using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardGame
{

    public class EffectCounter_Advanced : EffectCounter, IEffectCounter
    {
        protected override void Awake()
        {
            base.Awake();
            InitDictionaries();
        }

        public int CalculatePower(CardDrag[] cardDrags, TableCardTracker tableCardTracker)
        {
            this.tableCardTracker = tableCardTracker;
            ClearDictionaries();
            power = 0;

            GetCards(cardDrags); // Пытаемся получить карты

            if (cardsInfo == null || cards == null) return 0; // Провеяем наличие карт на поле

            for (int i = 0; i < Enum.GetNames(typeof(CardTypeEffect)).Length; i++) // Получаем количество карт по эффектам
                FindPowerEffectCard(cards, (CardTypeEffect)i);
            for (int i = 0; i < Enum.GetNames(typeof(CardTypeGroup)).Length; i++) // Получаем количество карт по группам
                FindGroupsCard(cards, (CardTypeGroup)i);

            DoDiscardFrontEffect();
            DoDiscardMostPowerfulCardsOnFront();
            DoRestoreTopFromPile();
            for (int i = 0; i < cards.Length; i++) //Проходимся по всем картам
            {
                if (dropPlaceTracker.DropPlace.FrontType == FrontType.Common) break; // Не применять усиления карт на руке
                cards[i].SetPower(cardsInfo[i].DefaultPower); // Перед усилением
                ApplyFrontEffect(i);
                if (cardsInfo[i].CardTypeBase == CardTypeBase.SpecialType) // Если спецкарта, выполняем
                    ApplyFrontEffect(i);
                if (cardsInfo[i].CardTypeBase != CardTypeBase.CommonType) continue; // Если карта героя или спецкарта, пропускаем ее
                ApplyFrontEffect(i);
                ApplyAddOneEffect(i);
                ApplyMultiplyRepeatsEffect(i);
                ApplyDoubleFrontEffect(i);
            }
            for (int i = 0; i < cardsInfo.Length; i++)
            {
                power += cards[i].Power;
            }
            return power;
        }

        #region ApplyEffects
        private void DoDiscardFrontEffect() // Отменяет ослабления в рядах
        {
            if (dropPlaceTracker.DropPlace.FrontType != FrontType.Common && CountByEffect[CardTypeEffect.DesperateMeasures] != 0)
            {
                for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++)
                {
                    for (int j = 0; j < Enum.GetNames(typeof(FrontType)).Length; j++)
                    {

                        DropPlaceTracker dropPlaceTracker = tableCardTracker.GetDropPlaceCardTracker((PlayerNum)i, (FrontType)j);
                        if (dropPlaceTracker.DropPlace.FrontType == FrontType.Common) continue;
                        dropPlaceTracker.SetFrontEffect(false);
                        if (dropPlaceTracker.transform.childCount == 0) continue;
                        CardDrag[] cardDrags = new CardDrag[dropPlaceTracker.transform.childCount];
                        for (int k = 0; k < cardDrags.Length; k++)
                        {
                            cardDrags[k] = dropPlaceTracker.transform.GetChild(k).GetComponent<CardDrag>();
                        }
                        for (int k = 0; k < cardDrags.Length; k++)
                            if (cardDrags[k].Card.Info.CardTypeEffect == CardTypeEffect.FrontEffect) cardDrags[k].Card.Discard(); ///

                        if (this.dropPlaceTracker == tableCardTracker.GetDropPlaceCardTracker((PlayerNum)i, (FrontType)j))
                        {
                            CountByEffect[CardTypeEffect.FrontEffect] = 0;
                            continue;
                        }
                        tableCardTracker.GetDropPlaceCardTracker((PlayerNum)i, (FrontType)j).CheckDropPlaceState();
                    }
                }
                if (CardsByEffect[CardTypeEffect.DesperateMeasures].Count() != 0) CardsByEffect[CardTypeEffect.DesperateMeasures][0].Discard();
            }  
        }
        private void DoDiscardMostPowerfulCardsOnFront() // Уничтожает вражеский(е) юнит(ы) с максимальной силой в ряду, если сила ряда больше значения
        {
            if (dropPlaceTracker.DropPlace.FrontType != FrontType.Common && CountByEffect[CardTypeEffect.CriticalError] != 0) // Проверяем количество CriticalError карт на 0
            {
                for (int j = 0; j < CardsByEffect[CardTypeEffect.CriticalError].Count; j++)
                {
                    if (CardsByEffect[CardTypeEffect.CriticalError][j].AbilityIsActive == false) continue;
                    for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++)
                    {
                        if (dropPlaceTracker.DropPlace.ControlPlayer == (PlayerNum)i) continue;
                        DropPlaceTracker drop = tableCardTracker.GetDropPlaceCardTracker((PlayerNum)i, dropPlaceTracker.DropPlace.FrontType);
                        if (drop.PowerCount < CardsByEffect[CardTypeEffect.CriticalError][j].Info.CritErrorNeededEnemyPower) continue;
                        if (drop.transform.childCount == 0) continue;
                        StartCoroutine(CritErrorNumerator(drop));
                    }
                    CardsByEffect[CardTypeEffect.CriticalError][j].SetAbilityActive(false);
                }
            }
        }
        IEnumerator CritErrorNumerator( DropPlaceTracker drop) // Delay before discarding by CritError
        {
            yield return new WaitForSeconds(0.5f);
            Card[] cards = new Card[drop.transform.childCount];
            int maxPower = 0;
            for (int k = 0; k < cards.Length; k++)
            {
                cards[k] = drop.transform.GetChild(k).GetComponent<Card>();
                if (cards[k].Info.CardTypeBase == CardTypeBase.CommonType && cards[k].Power > maxPower) maxPower = cards[k].Power;
            }
            bool isDiscarded = false;
            for (int k = 0; k < cards.Length; k++)
                if (cards[k].Info.CardTypeBase == CardTypeBase.CommonType && cards[k].Power == maxPower)
                {
                    isDiscarded = true;
                    cards[k].Discard();
                }
            if (isDiscarded == true)
            {
                soundHook.m_Sound = Sound.CritError;
                soundHook.Play();
            }
            drop.CheckDropPlaceState();
        }
        private void DoRestoreTopFromPile() // Restore to playerHand discarded cards in definite count
        {
            if (CardsByEffect[CardTypeEffect.Anastasis].Count == 0 || dropPlaceTracker.DropPlace.FrontType == FrontType.Common) return;
            for (int i = 0; i < CountByEffect[CardTypeEffect.Anastasis]; i++)
            {
                if (CardsByEffect[CardTypeEffect.Anastasis][i].AbilityIsActive == false) continue;
                for (int j = 0; j < Enum.GetNames(typeof(PlayerNum)).Length; j++)
                {
                    if (dropPlaceTracker.DropPlace.ControlPlayer != (PlayerNum)j) continue;
                    tableCardTracker.GetDiscardPile((PlayerNum)j).UpdateDiscardPileCards();
                    List<CardDrag> discardCards = tableCardTracker.GetDiscardPile((PlayerNum)j).DiscardCards;
                    int restoreCounter = 0;
                    for (int k = 0; k < discardCards.Count; k++)
                    {
                        if (restoreCounter >= CardsByEffect[CardTypeEffect.Anastasis][i].Info.RestoreCardCount || 
                            discardCards[k].Card.Info.CardTypeBase != CardTypeBase.CommonType) continue;
                        discardCards[k].Card.Restore();
                        restoreCounter++;
                    }
                    tableCardTracker.GetDropPlaceCardTracker((PlayerNum)j, FrontType.Common).CheckDropPlaceState();
                    CardsByEffect[CardTypeEffect.Anastasis][i].SetAbilityActive(false);
                }
            }
        }
        private void ApplyFrontEffect(int index) // Применяет FrontEffect 
        {
            bool effectIsSounded = false;
            if (CountByEffect[CardTypeEffect.FrontEffect] > 0 && dropPlaceTracker.IsFrontEffected == false) // Проверяем количество FrontEffect карт на 0
                for (int i = 0; i<Enum.GetNames(typeof(PlayerNum)).Length; i++)
                {
                    DropPlaceTracker tracker = tableCardTracker.GetDropPlaceCardTracker((PlayerNum)i, cardsInfo[index].Front);
                    if (tableCardTracker.GetDropPlaceCardTracker(GetOtherPlayer((PlayerNum)i), cardsInfo[index].Front).IsFrontEffected == true) effectIsSounded = true;
                    if (tracker.IsFrontEffected != true && effectIsSounded != true)
                    {
                        soundHook.m_Sound = Sound.FieldEffect;
                        soundHook.Play();
                        
                        effectIsSounded = true;
                    }
                    tracker.SetFrontEffect(true);
                    if (dropPlaceTracker.DropPlace.ControlPlayer == (PlayerNum)i) continue;
                    tableCardTracker.GetDropPlaceCardTracker((PlayerNum)i, cardsInfo[index].Front).CheckDropPlaceState();
                }
            if(dropPlaceTracker.IsFrontEffected == true)
            {
                if (cardsInfo[index].CardTypeBase == CardTypeBase.EliteType) return; // Не влияет на героев
                if (cardsInfo[index].CardTypeBase == CardTypeBase.SpecialType) // Снижаем начальную силу обычным картам до 1
                    cards[index].SetPower(0);
                else cards[index].SetPower(1);
            }
        }
        private void ApplyAddOneEffect(int index) // Применяет AddOnePower
        {
            if (CountByEffect[CardTypeEffect.Retranslation] > 0) // Проверяем количество AddOnePower карт на 0
            {
                if (cardsInfo[index].CardTypeEffect == CardTypeEffect.Retranslation) // Добавляем силу обычным картам по количеству AddOnePower карт и если это она и есть, то на 1 меньше
                    cards[index].SetPower(cards[index].Power + CountByEffect[CardTypeEffect.Retranslation] - 1);
                else cards[index].SetPower(cards[index].Power + CountByEffect[CardTypeEffect.Retranslation]);
            }
        }
        private void ApplyMultiplyRepeatsEffect(int index) // Применяет MultiplyRepeats
        {
            if (cardsInfo[index].CardTypeGroup == CardTypeGroup.None) return;
            if (CountByEffect[CardTypeEffect.MultiplyRepeats] > 0) // Проверяем количество MultiplyRepeats карт на 0
            {
                if (cardsInfo[index].CardTypeEffect == CardTypeEffect.MultiplyRepeats) // Умножаем силу MultiplyRepeats карт по их количеству
                    cards[index].SetPower(cards[index].Power * CountByGroup[cards[index].Info.CardTypeGroup]);
            }
        }
        private void ApplyDoubleFrontEffect(int index) // Применяет DoubleFront
        {
            if (CountByEffect[CardTypeEffect.DoubleFront] > 0) // Проверяем количество DoubleFront карт на 0
            {
                cards[index].SetPower(cards[index].Power * 2); // Удваиваем силу обычным картам
            }
        }
        #endregion
        private PlayerNum GetOtherPlayer(PlayerNum player) => player == PlayerNum.Player_1 ? PlayerNum.Player_2 : PlayerNum.Player_1;

    }
}