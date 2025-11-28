using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public class OpponentAI : MonoBehaviour, IDependency<MoveManager>, IDependency<LevelStateTracker>, IDependency<TableCardTracker>
    {
        [SerializeField] private bool aiIsActive;

        private IAICalculator m_AICalculator;
        private PlayerNum controlPlayer; 
        private LevelStateTracker levelStateTracker;
        private MoveManager moveManager;
        private TableCardTracker tableCardTracker;
        private Dictionary<FrontType, DropPlace> selfDropPlaces;

        public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
        public void Construct(MoveManager obj) => moveManager = obj;
        public void Construct(TableCardTracker obj) => tableCardTracker = obj;
        public PlayerNum ControlPlayer => controlPlayer;

        private void Awake()
        {
            aiIsActive = GetComponent<Player>().PlayerType == PlayerType.AI;
            enabled = aiIsActive;
            controlPlayer = GetComponent<Player>().PlayerNum;
            moveManager.TurnSwitched.AddListener(OnTurnSwitched);
        }
        private void Start()
        {
            selfDropPlaces = new Dictionary<FrontType, DropPlace>();
            for (int i = 0; i < Enum.GetNames(typeof(FrontType)).Length; i++)
            {
                DropPlace drop = tableCardTracker.GetDropPlaceCardTracker(controlPlayer, (FrontType)i).GetComponent<DropPlace>();
                selfDropPlaces.Add(drop.FrontType, drop);
            }
        }
        private void OnDestroy()
        {
            moveManager.TurnSwitched.RemoveListener(OnTurnSwitched);
        }
        private void OnTurnSwitched(PlayerNum move)
        {
            if (move != controlPlayer) return;
            StartCoroutine(AIMoveNumerator());
        }
        IEnumerator AIMoveNumerator()
        {
            yield return new WaitForEndOfFrame();
            AIMove();
        }
        private void AIMove()
        {
            if (levelStateTracker.State != LevelState.RoundAction) return;
            if (selfDropPlaces[FrontType.Common] == null) return;

            m_AICalculator.CalculateMove(controlPlayer);
        }
        public void InjectCalculator(IAICalculator calculator) => m_AICalculator = calculator;
    }
}
