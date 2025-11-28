using System;

namespace CardGame
{
    public class AICalculator_Random : AICalculator, IAICalculator
    {
        public void CalculateMove(PlayerNum player)
        {
            controlPlayer = player;

            GetCards();
            CardDrag selectedHandCard = null;

            float randomPoint = UnityEngine.Random.Range(0f, 1f);
            if (levelStateTracker.CurrentRound != levelStateTracker.Rounds && levelManager.AIPassChance > randomPoint)
            {
                for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++)
                {
                    if ((PlayerNum)i == controlPlayer && selfPower > 0)
                    {
                        Pass(true); // Рандомно пассует
                        StartCoroutine(AIMoveNumerator(selectedHandCard));
                        return;
                    }
                }
            }
            for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++)
            {
                if ((PlayerNum)i != controlPlayer && levelStateTracker.GetPlayerPassState((PlayerNum)i) == true && selfPower > enemyPower)
                {
                    Pass(true);
                    return;
                }
            }
            if (selfDropPlaceTrackers == null)
            {
                StartCoroutine(AIMoveNumerator(selectedHandCard));
                return;
            }
            if (selfDropPlaceTrackers[FrontType.Common].transform.childCount > 0)
            {
                selectedHandCard = selfDropPlaceTrackers[FrontType.Common].transform.
                    GetChild(UnityEngine.Random.Range(0, selfDropPlaceTrackers[FrontType.Common].transform.childCount)).GetComponent<CardDrag>();
                if (selectedHandCard.Card.Info.Front == FrontType.Common)
                {
                    FrontType RandomDrop()
                    {
                        FrontType randomFront = (FrontType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(FrontType)).Length);
                        if (randomFront == FrontType.Common) RandomDrop();
                        return randomFront;
                    }
                    selectedHandCard.CurrentParent = selfDropPlaceTrackers[RandomDrop()].transform;
                }
                else
                    selectedHandCard.CurrentParent = selfDropPlaceTrackers[selectedHandCard.Card.Info.Front].transform;
                StartCoroutine(AIMoveNumerator(selectedHandCard));
            }
            else
                StartCoroutine(AIMoveNumerator(selectedHandCard));
        }
    }
}