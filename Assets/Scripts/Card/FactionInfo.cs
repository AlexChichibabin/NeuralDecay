using UnityEngine;

[CreateAssetMenu(fileName = "FactionInfo", menuName = "Scriptable Objects/FactionInfo")]
public class FactionInfo : ScriptableObject
{
    [SerializeField] private CardInfo[] m_CardInfo;

    public CardInfo[] CardInfo => m_CardInfo;
}
