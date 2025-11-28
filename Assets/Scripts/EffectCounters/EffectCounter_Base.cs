namespace CardGame
{
    public class EffectCounter_Base : EffectCounter, IEffectCounter
    {
        public int CalculatePower(CardDrag[] cards, TableCardTracker tableCardTracker)
        {
            GetCards(cards);
            power = 0;
            if (cardsInfo == null) return 0;
            for (int i = 0; i < cardsInfo.Length; i++)
            {
                power += cardsInfo[i].DefaultPower;
            }
            return power;
        }
    }
}