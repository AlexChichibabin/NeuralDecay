using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardGame
{
    public class CardDeck : a_Deck, IPointerClickHandler, IDependency<LevelManager>, IDependency<TableCardTracker>, IDependency<CardInfoPool>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CardInfoPool cardInfoPool;
        private int dealCardsCount;
        private LevelManager levelManager;
        private CardInfo[] cardInfo;
        private CardUnit[] cardUnits;
        private GameObject tempCard;
        private Vector3 defaultTempCardPosition;
        private TableCardTracker tableCardTracker;
        private bool isPlayer => PlayerNum.Player_1 == controlPlayer ? true : false;

        public void Construct(LevelManager obj) => levelManager = obj;
        public void Construct(TableCardTracker obj) => tableCardTracker = obj;
        public void Construct(CardInfoPool obj) => cardInfoPool = obj;

        protected new void Awake()
        {
            base.Awake();
            tempCard = canvas.GetComponentInChildren<CardDrag>().gameObject;
            defaultTempCardPosition = tempCard.transform.localPosition;
            if (isPlayer == true) TryLoadDeck();
            if (levelManager.DealCardType == DealCardType.UpToMax) dealCardsCount = levelManager.CardCountToMax;
            if (levelManager.DealCardType == DealCardType.Discrete) dealCardsCount = levelManager.CardCountDiscrete;
            levelStateTracker.RoundPreparationStarted += OnRoundPreparationStarted;
            ShuffleCards(2);
        }
        private void OnDestroy() => levelStateTracker.RoundPreparationStarted -= OnRoundPreparationStarted;

        [ContextMenu(nameof(ShuffleCards))]
        public void DealCards(int count)
        {
            if (levelManager.MaxHandCards == handPlace.childCount) return;
            if (levelManager.DealCardType == DealCardType.UpToMax) count = levelManager.MaxHandCards - handPlace.childCount;
            count = Mathf.Clamp(count, 0, cards.Length);

            for (int i = cards.Length - 1; i >= cards.Length - count; i--)
            {
                cards[i].transform.SetParent(handPlace, false);
                cards[i].GetComponent<Card>().SetOwner(handPlace.GetComponent<DropPlace>().ControlPlayer);
            }
            Card[] tempCards = new Card[cards.Length];
            Array.Resize(ref cards, container.childCount);

            soundHook.m_Sound = Sound.CardDeal;
            soundHook.Play();
        }
        public void OnPointerClick(PointerEventData eventData) =>
            Debug.Log($"OnClick {controlPlayer} has {cards.Length} cards");

        private void TryLoadDeck()
        {
            cardInfoPool.InitDictionary();
            Saver<CardUnit[]>.TryLoad(levelManager.DeckSaveFileName, ref cardUnits);
            if (cardUnits == null)
            {
                ApplyCardDragsData();
                return;
            }
            cardInfo = ConverterInfoEnum.ConvertFromEnumToCardInfo(cardUnits, cardInfoPool);
            if (cardInfo != null)
            {
                GetComponent<SpawnObjectByPropertiesList>().SpawnBy(cardInfo);
                ApplyCardDragsData();

                if (cards.Length != cardInfo.Length)
                    Array.Resize(ref cards, cardInfo.Length);
            }
        }
        private void ApplyCardDragsData()
        {
            foreach (var card in transform.GetComponentsInChildren<CardDrag>())
            {
                card.ThrowProperties(canvas, tempCard, defaultTempCardPosition, levelManager);
                card.Card.Construct(tableCardTracker);
            }
        }
        private void OnRoundPreparationStarted()
        {
            Debug.Log($"OnRoundPreparationStarted {controlPlayer} has {cards.Length} cards");
            if (levelManager.DealCardType == DealCardType.Discrete)
            {
                if (levelStateTracker.CurrentRound == 1)
                    for (int i = 0; i < levelManager.MaxHandCards; i += levelManager.CardCountDiscrete) DealCards(levelManager.CardCountToMax); // Ìב ןמÿגטעסÿ באד
                else
                    DealCards(dealCardsCount);
            }
            else
                DealCards(dealCardsCount);
        }
    }
}