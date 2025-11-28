using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardInfoPool", menuName = "Scriptable Objects/CardInfoPool")]
public class CardInfoPool : ScriptableObject
{
    public CardInfo[] Pool;
    public Dictionary<CardUnit, CardInfo> Cards;

    public void InitDictionary()
    {
        if (Cards != null) return;
        Cards = new Dictionary<CardUnit, CardInfo>();
        for (int i = 0; i < Pool.Length; i++)
        {
            if (Cards.ContainsKey(Pool[i].CardUnit) == true)
            {
                Debug.Log($"CardUnit named of {Pool[i].CardUnit} is already exist");
                return;
            }
            Cards.Add(Pool[i].CardUnit, Pool[i]);
        }
    }
    public CardInfo GetCardInfo(CardUnit unit) => Cards[unit];
}
