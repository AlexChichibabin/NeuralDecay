using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public class DiscardPile : a_Deck, IDependency<TableCardTracker>
    {
        private Dictionary<FrontType, DropPlaceTracker> dropPlaceTrackers;
        private TableCardTracker tableCardTracker;
        private List<CardDrag> discardCards; 

        public void Construct(TableCardTracker obj) => tableCardTracker = obj;
        public Transform Container => container;
        public List<CardDrag> DiscardCards => discardCards;

        protected new void Awake()
        {
            base.Awake();
            levelStateTracker.RoundCompleted += OnRoundCompleted;
            discardCards = new List<CardDrag>();
            soundHook.m_Sound = Sound.CardDeal;
        }
        public void UpdateDiscardPileCards()
        {
            discardCards.Clear();
            for (int i = 0; i < container.childCount; i++)
                discardCards.Add(container.GetChild(i).GetComponent<CardDrag>());
            soundHook.Play();
        }
        private void Start() => GetDropPlaceTrackers();
        private void OnDestroy() => levelStateTracker.RoundCompleted -= OnRoundCompleted;
        private void OnRoundCompleted()
        {
            bool discarded = false;
            for (int frontType = 0; frontType < Enum.GetNames(typeof(FrontType)).Length; frontType++)
            {
                if (dropPlaceTrackers[(FrontType)frontType].GetChildCards() == null || dropPlaceTrackers[(FrontType)frontType].transform.childCount == 0) continue;
                for (int trackerIndex = 0; trackerIndex < dropPlaceTrackers[(FrontType)frontType].GetChildCards().Length; trackerIndex++)
                {
                    if((FrontType)frontType == FrontType.Common) continue;
                    discardCards.Add(dropPlaceTrackers[(FrontType)frontType].GetChildCards()[trackerIndex]);
                }
            }
            for (int cardIndex = 0; cardIndex < discardCards.Count; cardIndex++)
            {
                if (discardCards[cardIndex].CurrentParent != container && discardCards[cardIndex].GetComponentInParent<DropPlace>().FrontType == FrontType.Common) continue;
                if (discardCards[cardIndex].Card.Info.CardTypeEffect == CardTypeEffect.TacticalRetreat && 
                    levelStateTracker.LastRoundWinner != discardCards[cardIndex].Card.ControlPlayer) // Если есть TacticalRetreat и этот игрок проиграл, то проверка такая, иначе простой Discard
                {
                    //if (levelStateTracker.LastRoundWinner == discardCards[cardIndex].Card.ControlPlayer) continue;
                    if (discardCards[cardIndex].Card.AbilityIsActive == true && discardCards[cardIndex].Card.Info.TacticalRetreatChance > UnityEngine.Random.Range(0f, 1f))
                    {
                        discardCards[cardIndex].CurrentParent = tableCardTracker.GetDropPlaceCardTracker(discardCards[cardIndex].Card.ControlPlayer, FrontType.Common).transform;
                        discardCards[cardIndex].transform.SetParent(discardCards[cardIndex].CurrentParent);
                        discardCards[cardIndex].Card.SetPower(discardCards[cardIndex].Card.Info.DefaultPower);
                        discardCards[cardIndex].Card.SetAbilityActive(false);
                        continue;
                    }
                    else
                    {
                        discardCards[cardIndex].Card.Discard();
                        discarded = true;
                    }
                    discardCards[cardIndex].Card.SetAbilityActive(false);
                }
                else
                {
                    discardCards[cardIndex].Card.Discard();
                    discarded = true;
                }
            }
            if (discarded == true) soundHook.Play();
        }
        private void GetDropPlaceTrackers()
        {
            dropPlaceTrackers = new Dictionary<FrontType, DropPlaceTracker>(); // HandPlace на случай если придется сбросить с руки

            for (int i = 0; i < Enum.GetNames(typeof(FrontType)).Length; i++)
            {
                dropPlaceTrackers.Add((FrontType)i, tableCardTracker.GetDropPlaceCardTracker(controlPlayer, (FrontType)i));
            }
        }
    }
}