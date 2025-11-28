using UnityEngine;
using UnityEngine.Events;

namespace CardGame
{
    public class MoveManager : MonoBehaviour, IDependency<TableCardTracker>, IDependency<LevelStateTracker>
    {
        [SerializeField] private Player[] players;

        private PlayerNum playerTurn;
        private LevelStateTracker levelStateTracker;
        private TableCardTracker tableCardTracker;

        public PlayerNum PlayerTurn => playerTurn;
        public UnityEvent<PlayerNum> TurnSwitched;
        public UnityEvent Moved;

        public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
        public void Construct(TableCardTracker obj) => tableCardTracker = obj;

        private void Awake() => levelStateTracker.RoundActionStarted += OnRoundStarted;
        private void Start() => RamdomInitiative();
        private void OnDestroy() => levelStateTracker.RoundActionStarted -= OnRoundStarted;

        public void Move(PlayerNum movedPlayer) => Moved?.Invoke(); // Isn't used
        public void SwitchTurn()
        {
            if (levelStateTracker.GetPlayerPassState(PlayerNum.Player_1) == false &&
                levelStateTracker.GetPlayerPassState(PlayerNum.Player_2) == false)
            {
                playerTurn = playerTurn == PlayerNum.Player_1 ? PlayerNum.Player_2 : PlayerNum.Player_1;
                TurnSwitched.Invoke(playerTurn);
            }
            if (levelStateTracker.GetPlayerPassState(PlayerNum.Player_1) == true &&
                levelStateTracker.GetPlayerPassState(PlayerNum.Player_2) == true) return;
            if (levelStateTracker.GetPlayerPassState(PlayerNum.Player_1) == true ||
                levelStateTracker.GetPlayerPassState(PlayerNum.Player_2) == true)
            {
                playerTurn = levelStateTracker.GetPlayerPassState(PlayerNum.Player_1) == true ? PlayerNum.Player_2 : PlayerNum.Player_1;
                TurnSwitched.Invoke(playerTurn);
            }
        }
        private PlayerNum RamdomInitiative() => playerTurn = (PlayerNum)(Random.Range(0, 2));
        private void OnRoundStarted()
        {
            TurnSwitched?.Invoke(playerTurn);
        }
    }
}