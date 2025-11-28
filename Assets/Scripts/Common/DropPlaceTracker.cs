using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardGame
{
    [RequireComponent(typeof(SoundHook))]
    public class DropPlaceTracker : MonoBehaviour, IDependency<MoveManager>, IDependency<TableCardTracker>, IDependency<LevelStateTracker>
    {
        [SerializeField] private UIDropPlace dropPlaceUI;

        private DropPlace dropPlace;
        private IEffectCounter powerCalculator;
        private LevelStateTracker levelStateTracker;
        private MoveManager moveManager;
        private TableCardTracker tableCardTracker;
        private CardDrag[] childLineCards;
        private int powerCount;
        private bool isFrontEffected;
        private bool isMain = false;
        private SoundHook soundHook;

        public bool IsMain => isMain;
        public DropPlace DropPlace => dropPlace;
        public int PowerCount => powerCount;
        public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
        public void Construct(MoveManager obj) => moveManager = obj;
        public void Construct(TableCardTracker obj) => tableCardTracker = obj;
        public bool IsFrontEffected => isFrontEffected;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu") isMain = true;
            if (isMain == false)
            {
                dropPlace = GetComponent<DropPlace>();
                tableCardTracker.SetDropPlaceTracker(this);
                levelStateTracker.RoundActionStarted += OnRoundActionStarted;
                levelStateTracker.LevelEnded += OnLevelEnded;
            }
            soundHook = GetComponent<SoundHook>();
        }
        private void Start() => UpdateText();
        private void OnDestroy()
        {
            if (isMain == false)
            {
                levelStateTracker.RoundActionStarted += OnRoundActionStarted;
                levelStateTracker.LevelEnded -= OnLevelEnded;
            }
        }
        public void CheckDropPlaceState() //MoveSystem ////////////////////////////
        {
            GetChildCards(); //Получает CardDrag(карты) и их суммарную силу по полю
            if (tableCardTracker != null) tableCardTracker.OnMoveUpdateCard(); //Считает TotalScores и отправляет на UI, затем проверяет на 0 карт
            UpdateText(); // Поскольку должен обновиться сначала powerCount, а потом уже отображаться на UI
        }
        public CardDrag[] GetChildCards()
        {
            powerCount = 0;
            if (transform.childCount == 0) return null;

            childLineCards = new CardDrag[transform.childCount];

            for (int i = 0; i < childLineCards.Length; i++)
            {
                childLineCards[i] = transform.GetChild(i).GetComponent<CardDrag>();
            }
            if(isMain == false) powerCount = powerCalculator.CalculatePower(childLineCards, tableCardTracker);
            return childLineCards;
        }
        public void InjectCalculator(IEffectCounter calculator) => powerCalculator = calculator;
        public void SetFrontEffect(bool isEffected)
        {
            isFrontEffected = isEffected;
        }

        private void OnRoundActionStarted()
        {
            SetFrontEffect(false);
            CheckDropPlaceState();
        } 
        private void OnLevelEnded(PlayerNum playerWin) => CheckDropPlaceState();
        private void UpdateText()
        {
            if (dropPlace != null && dropPlace.FrontType != FrontType.Common) dropPlaceUI.UpdateUIInfo();
        }
    }
}