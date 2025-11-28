public static class ConverterInfoEnum
{
    public static CardInfo[] ConvertFromEnumToCardInfo(CardUnit[] cardUnit, CardInfoPool pool)
    {
        CardInfo[] cardSO = new CardInfo[cardUnit.Length];
        for (int i = 0; i < cardSO.Length; i++) cardSO[i] = pool.GetCardInfo(cardUnit[i]);
        return cardSO;
    }
    public static CardUnit[] ConvertFromCardInfoToEnum(CardInfo[] cardInfo)
    {
        CardUnit[] cardUnits = new CardUnit[cardInfo.Length];
        for (int i = 0; i < cardUnits.Length; i++) cardUnits[i] = cardInfo[i].CardUnit;
        return cardUnits;
    }
}
