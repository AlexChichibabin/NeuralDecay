namespace CardGame
{
    public interface IEffectCounter
    {
        int CalculatePower(CardDrag[] cards, TableCardTracker tableCardTracker);
    }
}