using UnityEngine;
using UnityEngine.Events;
using CardGame;
using System.Collections.Generic;
using System;
using System.Collections;
using UniRx;

public enum LevelState
{
    LevelStarted,
    RoundPrepare,
    RoundAction,
    RoundCompleted,
    LevelEnded
}

public class LevelStateTracker : MonoBehaviour
{
    class PlayerOnGame // isn't serializable
    {
        public bool IsPassedRound;
        public int WinCount;
    }

    [SerializeField] private int rounds;
    [SerializeField] private Timer countDownTimer;

    private Dictionary<PlayerNum, PlayerOnGame> players;
    private PlayerNum lastRoundWinner;
    private int currentRound;
    private LevelState state;

    public event UnityAction LevelStarted;
    public event UnityAction RoundPreparationStarted;
    public event UnityAction RoundActionStarted;
    public event UnityAction RoundCompleted;
    public event UnityAction<PlayerNum> LevelEnded;
    public event UnityAction<PlayerNum, bool> PassStateChanged;

    public int CurrentRound => currentRound;
    public PlayerNum LastRoundWinner => lastRoundWinner;
    public Timer CountDownTimer => countDownTimer;
    public int Rounds => rounds;
    public LevelState State => state;

    private void Awake()
    {
        players = new Dictionary<PlayerNum, PlayerOnGame>();
        for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++) // Создаем словарь состояния для каждого игрока
        {
            players.Add((PlayerNum)i, new PlayerOnGame());
            players[(PlayerNum)i].IsPassedRound = false;
            players[(PlayerNum)i].WinCount = 0;
        }
    }
    private void Start() => StartCoroutine(StartStateNmerator());
    IEnumerator StartStateNmerator()
    {
        yield return new WaitForEndOfFrame();

        StartState(LevelState.LevelStarted);
        currentRound = 1;
        LevelStarted?.Invoke();
        LaunchPreparationStart();
    }
    private void StartState(LevelState state) => this.state = state;
    public bool GetPlayerPassState(PlayerNum player) => players[player].IsPassedRound;
    public int GetPlayerWinsCount(PlayerNum player) => players[player].WinCount;
    public PlayerNum SetLastRoundWinner(PlayerNum player) => lastRoundWinner = player;
    public void AddRoundWinToPlayer(PlayerNum player) => players[player].WinCount++;
    public void LaunchPreparationStart()
    {
        if (state != LevelState.LevelStarted && state != LevelState.RoundCompleted) return;
        StartState(LevelState.RoundPrepare);
        RoundPreparationStarted?.Invoke(); // Здесь начинаются все приготовления перед игрой
        StartLevelAction(); // Debug only
    }
    public void SetPassPlayerState(PlayerNum player, bool isPassed) //MoveSystem ////////////////////////////
    {
        players[player].IsPassedRound = isPassed;
        PassStateChanged?.Invoke(player, isPassed);

        if (players[PlayerNum.Player_1].IsPassedRound == true &&
            players[PlayerNum.Player_2].IsPassedRound == true)
            if (state == LevelState.RoundAction)
                StartCoroutine(RoundCompletionNum());
    }
    IEnumerator RoundCompletionNum()
    {
        yield return new WaitForEndOfFrame();
        CompleteRound();
    }
    private void StartLevelAction()
    {
        if (state != LevelState.RoundPrepare) return;
        StartState(LevelState.RoundAction);
        for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++) // Обнуляем IsPassed в начале раунда
            SetPassPlayerState((PlayerNum)i, false); //MoveSystem ////////////////////////////
        RoundActionStarted?.Invoke();
    }
    private void CompleteRound()
    {
        if (state != LevelState.RoundAction) return;

        StartState(LevelState.RoundCompleted);
        StartCoroutine(WaitAtEndRound());
    }
    IEnumerator WaitAtEndRound()
    {
        yield return new WaitForSeconds(1f);
        RoundCompleted?.Invoke();

        currentRound++;
        yield return new WaitForSeconds(1f);
        if (currentRound <= rounds)
        {
            LaunchPreparationStart();
        }
        else
            CompleteLevel();
    }
    private bool GetLevelWinner(out PlayerNum winner)
    {
        if (players[PlayerNum.Player_1].WinCount > players[PlayerNum.Player_2].WinCount)
        {
            winner = PlayerNum.Player_1;
            return true;
        }
        if (players[PlayerNum.Player_2].WinCount > players[PlayerNum.Player_1].WinCount)
        {
            winner = PlayerNum.Player_2;
            return true;
        }
        if (players[PlayerNum.Player_1].WinCount == players[PlayerNum.Player_2].WinCount)
        {
            winner = PlayerNum.Player_1;
            return false;
        }
        winner = PlayerNum.Player_1;
        return false;
    }
    private void CompleteLevel()
    {
        if (state != LevelState.RoundCompleted) return;
        StartState(LevelState.LevelEnded);
        Debug.Log("GameEnded");
        PlayerNum winner;
        if (GetLevelWinner(out winner) == true)
            Debug.Log("Winner is: " + winner);
        else
            Debug.Log("Winner is: none");
        LevelEnded?.Invoke(winner);
    }
}
