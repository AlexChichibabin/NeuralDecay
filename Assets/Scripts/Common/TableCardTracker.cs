using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CardGame
{
    public class TableCardTracker : MonoBehaviour, IDependency<MoveManager>, IDependency<LevelStateTracker>
    {
        [Serializable]
        public class PlayerCards
        {
            public PlayerNum player;
            public int TotalScores;
            public Dictionary<FrontType, DropPlaceTracker> cardTrackers;
        }
        private LevelStateTracker levelStateTracker;
        private MoveManager moveManager;
        private Dictionary<PlayerNum, PlayerCards> playerCardsTable;
        private Dictionary<PlayerNum, DiscardPile> discardPiles;

        public UnityEvent<PlayerNum, int> ScoreUpdated;
        public Dictionary<PlayerNum, PlayerCards> PlayerCardsTable => playerCardsTable;
        public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
        public void Construct(MoveManager obj) => moveManager = obj;
        public int GetPlayerScores(PlayerNum playerNumber) => playerCardsTable[playerNumber].TotalScores;
        public DiscardPile GetDiscardPile(PlayerNum playerNumber) => discardPiles[playerNumber];
        public DropPlaceTracker GetDropPlaceCardTracker(PlayerNum player, FrontType front) => playerCardsTable[player].cardTrackers[front];

        private void Awake() // В ProjectSettings должен быть запущен раньше, чем DropPlaceCardTracker
        {
            InitDictionaries();

            levelStateTracker.RoundActionStarted += OnRoundActionStarted;
            levelStateTracker.RoundCompleted += OnRoundCompleted;
        }
        private void Start() => UpdateDropPlacesStates();
        private void OnDestroy()
        {
            levelStateTracker.RoundActionStarted -= OnRoundActionStarted;
            levelStateTracker.RoundCompleted -= OnRoundCompleted;
        }
        public void OnMoveUpdateCard()
        {
            UpdateDropPlacesStates();
            CheckHandCardAbsence();
        }
        /// <summary>
        /// В Awake каждый DropPlaceCardTracker закидывает себя в Словарь в TableCardTracker
        /// </summary>
        /// <param name="DropPlaceCardTracker"></param>
        public void SetDropPlaceTracker(DropPlaceTracker cardTracker)
        {
            playerCardsTable[cardTracker.GetComponent<DropPlace>().ControlPlayer].cardTrackers.
                Add(cardTracker.GetComponent<DropPlace>().FrontType, cardTracker);
            if (cardTracker.DropPlace.FrontType == FrontType.Common)
                discardPiles.Add(cardTracker.DropPlace.ControlPlayer,
                    cardTracker.transform.parent.GetComponentInChildren<DiscardPile>());
        }
        /// <summary>
        /// Обновляет DropPlaces пересчитывает их очки
        /// </summary>
        public void UpdateDropPlacesStates()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu") return;
            for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++)
            {
                playerCardsTable[(PlayerNum)i].TotalScores = 0;
                for (int j = 1; j < Enum.GetNames(typeof(FrontType)).Length; j++)
                    playerCardsTable[(PlayerNum)i].TotalScores +=
                        playerCardsTable[(PlayerNum)i].cardTrackers[(FrontType)j].PowerCount;
                ScoreUpdated?.Invoke((PlayerNum)i, playerCardsTable[(PlayerNum)i].TotalScores);
            }
        }
        private void OnRoundActionStarted() => CheckHandCardAbsence();
        /// <summary>
        /// Создает словарь очков для каждого игрока во время Awake (разово)
        /// </summary>
        private void InitDictionaries()
        {
            playerCardsTable = new Dictionary<PlayerNum, PlayerCards>();
            discardPiles = new Dictionary<PlayerNum, DiscardPile>();
            for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++)
            {
                playerCardsTable.Add((PlayerNum)i, new PlayerCards());
                playerCardsTable[(PlayerNum)i].player = (PlayerNum)i;
                playerCardsTable[(PlayerNum)i].cardTrackers = new Dictionary<FrontType, DropPlaceTracker>();
                //discardPiles.Add((PlayerNum)i, new DiscardPile());
            }
        }
        /// <summary>
        /// Проверяем руки обоих игроков. Если 0, то он пасует.
        /// </summary>
        private void CheckHandCardAbsence()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu") return;
            for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++)
                if (playerCardsTable[(PlayerNum)i].cardTrackers[FrontType.Common].transform.childCount == 0)
                    levelStateTracker.SetPassPlayerState((PlayerNum)i, true); // ставит isPassed true и если это у обоих игроков, то завершает раунд
        }
        /// <summary>
        /// Define who win the round
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool CheckWinner(out PlayerNum player)
        {
            if (playerCardsTable[PlayerNum.Player_1].TotalScores > playerCardsTable[PlayerNum.Player_2].TotalScores)
            {
                player = PlayerNum.Player_1;
                return true;
            } 
            if (playerCardsTable[PlayerNum.Player_1].TotalScores < playerCardsTable[PlayerNum.Player_2].TotalScores)
            {
                player = PlayerNum.Player_2;
                return true;
            }
            player = PlayerNum.Player_1;
            return false;
        }
        private void OnRoundCompleted()
        {
            PlayerNum playerWin;
            if (CheckWinner(out playerWin) == true)
            {
                levelStateTracker.AddRoundWinToPlayer(playerWin);
                levelStateTracker.SetLastRoundWinner(playerWin);
            }
                
            if(CheckWinner(out playerWin) == true) Debug.Log("Round win: " + playerWin);
            else Debug.Log("Round win: none");
        }
    }
}