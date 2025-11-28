using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardGame
{
    [RequireComponent(typeof(SoundHook))]
    public class DropPlace : MonoBehaviour, 
        IDropHandler, IPointerEnterHandler, IPointerExitHandler, 
        IDependency<MoveManager>, IDependency<LevelStateTracker>, IDependency<LevelManager>
    {
        [SerializeField] protected FrontType frontType;
        [SerializeField] private PlayerNum controlPlayer;

        [Header("CardSizing")]
        [SerializeField] protected float parentCardRatio = 0.9f;
        [SerializeField] protected GridLayoutGroup layoutGroup;

        protected LevelStateTracker levelStateTracker;
        protected LevelManager levelManager;
        protected DropPlaceTracker dropPlaceTracker;
        protected SoundHook soundHook;
        protected float cardHeight;
        protected float cardWidth;
        protected CardDrag[] childLineCards;
        protected MoveManager moveManager;
        protected string panelName => frontType.ToString();

        public void Construct(MoveManager obj) => moveManager = obj;
        public FrontType FrontType => frontType;
        public PlayerNum ControlPlayer => controlPlayer;
        public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
        public void Construct(LevelManager obj) => levelManager = obj;
        public DropPlaceTracker CardTracker => dropPlaceTracker;

        private void Awake() => soundHook = GetComponent<SoundHook>();
        protected virtual void Start()
        {
            moveManager.TurnSwitched.AddListener(OnTurnSwitched);
            levelStateTracker.LevelEnded += OnLevelEnded;
            CalculateCardSizes();
            dropPlaceTracker = GetComponent<DropPlaceTracker>();
        }
        protected virtual void OnDestroy()
        {
            moveManager.TurnSwitched.RemoveListener(OnTurnSwitched);
            levelStateTracker.LevelEnded -= OnLevelEnded;
        }

        #region MoveSystem
        public virtual void OnDrop(PointerEventData eventData) // Do when release mouse pointer within object area(moving mouse button up)
        {
            CardDrag card = eventData.pointerDrag.GetComponent<CardDrag>();

            if (card != null && CheckCardType(card.Card) == true)
            {
                card.CurrentParent = transform;

                if (frontType != FrontType.Common)
                    StartCoroutine(PlayerMoveNumerator());
            }
        }
        public IEnumerator PlayerMoveNumerator() // IEnumerator for coroutine of player move
        {
            yield return new WaitForEndOfFrame();
            dropPlaceTracker.CheckDropPlaceState();
            soundHook.Play();
            moveManager.Move(controlPlayer); 
            moveManager.SwitchTurn();
        }
        protected void OnTurnSwitched(PlayerNum move)
        {
            childLineCards = dropPlaceTracker.GetChildCards();
            if (childLineCards?.Length > 0)
                ChangeCardDropActivity(controlPlayer == move);
        }
        protected virtual void ChangeCardDropActivity(bool enabled) 
        {
            if (childLineCards == null) return;

            if (childLineCards.Length > 0)
                for (int i = 0; i < childLineCards.Length; i++)
                {
                    childLineCards[i].SetInteractivity(enabled);
                    if (frontType != FrontType.Common) childLineCards[i].SetInteractivity(false);
                    if (controlPlayer == PlayerNum.Player_2) childLineCards[i].SetInteractivity(false);
                }
        }
        protected void OnLevelEnded(PlayerNum playerWin) => ChangeCardDropActivity(false);
        #endregion

        public virtual void OnPointerEnter(PointerEventData eventData) // Do when mouse pointer enter object area
        {
            if (eventData.pointerDrag == null) return;

            CardDrag cardDrag = eventData.pointerDrag.GetComponent<CardDrag>();
            SetOnDragCardState(cardDrag);
        }
        public virtual void OnPointerExit(PointerEventData eventData) // Do when mouse pointer exit object area
        {
            if (eventData.pointerDrag == null) return;

            CardDrag cardDrag = eventData.pointerDrag.GetComponent<CardDrag>();
            SetOnDragCardState(cardDrag);
        }
        protected virtual bool CheckCardType(Card card)
        {
            if(card.ControlPlayer != controlPlayer) return false;
            if(card.Info.IsDisplacer) return false;

            if (card.Info.Front == frontType) return true;
            if (card.Info.Front == FrontType.Common) return true;
            if (frontType == FrontType.Common) return true;

            return false;
        }
        /// <summary>
        /// Calculate cellSize of cardPlace gridLayoutGroup
        /// </summary>
        protected void CalculateCardSizes()
        {
            if (transform.parent.GetComponent<LayoutElement>() == null) return;

            if (transform.parent.GetComponent<LayoutElement>().ignoreLayout == false)
                cardHeight = layoutGroup.cellSize.y * parentCardRatio;
            else
                cardHeight = (transform as RectTransform).rect.height * parentCardRatio;

            cardWidth = cardHeight * 0.578f;
            GetComponent<GridLayoutGroup>().cellSize = new Vector2(cardWidth, cardHeight);
        }
        /// <summary>
        /// Set temp card parent and color while drag
        /// </summary>
        /// <param name="cardDrag"></param>
        protected virtual void SetOnDragCardState(CardDrag cardDrag)
        {
            if (cardDrag == null) return;

            TempCard tCard = cardDrag.TempCard.GetComponent<TempCard>();
            Card card = cardDrag.GetComponent<Card>();

            if (cardDrag.DefaultTempCardParent == transform)
            {
                cardDrag.DefaultTempCardParent = cardDrag.CurrentParent;
                tCard.SetColor(tCard.DefaultColor);
            }
            else if (cardDrag.Card.Info.IsDisplacer == false)
            {
                cardDrag.DefaultTempCardParent = transform;
                if (card.Info.Front == frontType || card.Info.Front == FrontType.Common) tCard.SetColor(levelManager.MainGreenColor);
                if (card.Info.Front != frontType && frontType != FrontType.Common) tCard.SetColor(levelManager.MainRedColor);
                if (card.Info.Front == FrontType.Common && frontType != FrontType.Common) tCard.SetColor(levelManager.MainGreenColor);
                if (frontType == FrontType.Common) tCard.SetColor(tCard.DefaultColor);
                if (card.ControlPlayer != controlPlayer) tCard.SetColor(levelManager.MainRedColor);
            }
        }
    }
}