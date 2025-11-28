using System.Collections.Generic;
using System;
using UnityEngine;

namespace CardGame
{
    public enum PowerCalculateType { Basic, Advance }
    [RequireComponent(typeof(SoundHook))]
    public abstract class EffectCounter : MonoBehaviour
    {
        [SerializeField] protected PowerCalculateType powerCalculateType;
        protected int power;
        protected DropPlaceTracker dropPlaceTracker;
        protected TableCardTracker tableCardTracker;
        protected Card[] cards;
        protected CardInfo[] cardsInfo;
        protected SoundHook soundHook;

        protected Dictionary<CardTypeEffect, List<Card>> CardsByEffect;
        protected Dictionary<CardTypeEffect, int> CountByEffect;
        protected Dictionary<CardTypeGroup, List<Card>> CardsByGroup;
        protected Dictionary<CardTypeGroup, int> CountByGroup;

        public PowerCalculateType PowerCalculateType => powerCalculateType;
        public void SetDropPlaceTracker(DropPlaceTracker dropPlaceTracker) => this.dropPlaceTracker = dropPlaceTracker;

        protected virtual void Awake() => soundHook = GetComponent<SoundHook>();
        protected void GetCards(CardDrag[] cards)
        {
            if (cards == null) return;
            this.cards = new Card[cards.Length];
            for (int i = 0; i < this.cards.Length; i++)
            {
                this.cards[i] = cards[i].GetComponent<Card>();
            }
            GetCardsInfoFromCards();
        }
        private void GetCardsInfoFromCards()
        {
            cardsInfo = new CardInfo[cards.Length];
            for (int i = 0; i < cardsInfo.Length; i++)
            {
                cardsInfo[i] = cards[i].Info;
            }
        }
        #region Dictionaries
        protected void InitDictionaries()
        {
            CardsByEffect = new Dictionary<CardTypeEffect, List<Card>>();
            CountByEffect = new Dictionary<CardTypeEffect, int>();
            CardsByGroup = new Dictionary<CardTypeGroup, List<Card>>();
            CountByGroup = new Dictionary<CardTypeGroup, int>();
            for (int i = 0; i < Enum.GetNames(typeof(CardTypeEffect)).Length; i++)
            {
                CardsByEffect.Add((CardTypeEffect)i, new List<Card>());
                CountByEffect.Add((CardTypeEffect)i, 0);
            }
            for (int i = 0; i < Enum.GetNames(typeof(CardTypeGroup)).Length; i++)
            {
                CardsByGroup.Add((CardTypeGroup)i, new List<Card>());
                CountByGroup.Add((CardTypeGroup)i, 0);
            }
        }
        protected void ClearDictionaries()
        {
            if (CardsByEffect != null && CountByEffect != null)
            {
                for (int i = 0; i < Enum.GetNames(typeof(CardTypeEffect)).Length; i++) // Обнуляем списки карт
                {
                    CardsByEffect[(CardTypeEffect)i].Clear();
                    CountByEffect[(CardTypeEffect)i] = 0;
                }
            }
            if (CardsByGroup != null && CountByGroup != null)
            {
                for (int i = 0; i < Enum.GetNames(typeof(CardTypeGroup)).Length; i++) // Обнуляем списки карт
                {
                    CardsByGroup[(CardTypeGroup)i].Clear();
                    CountByGroup[(CardTypeGroup)i] = 0;
                }
            }
        }
        #endregion
        #region FindingCardsByEnums
        protected void FindPowerEffectCard(Card[] cards, CardTypeEffect effect) // Находит все карты с их эффектами и считает их количество
        {
            if (cards == null || cards.Length == 0) return;
            for (int i = 0; i < cards.Length; i++)
                if (cards[i].Info)
                    if (cards[i].Info.CardTypeEffect == effect)
                    {
                        CardsByEffect[effect].Add(cards[i]);
                        CountByEffect[effect]++;
                    }
        }
        protected void FindGroupsCard(Card[] cards, CardTypeGroup group) // Находит все карты с их эффектами и считает их количество
        {
            if (cards == null || cards.Length == 0) return;
            for (int i = 0; i < cards.Length; i++)
                if (cards[i].Info)
                    if (cards[i].Info.CardTypeGroup == group)
                    {
                        CardsByGroup[group].Add(cards[i]);
                        CountByGroup[group]++;
                    }
        }
        #endregion
    }
}