using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TextProperties", menuName = "Scriptable Objects/TextProperties")]
public class TextProperties : ScriptableObject
{
    [SerializeField] private Color m_MainTextColor;
    [SerializeField] private TMP_FontAsset m_TextFont;

    public Color MainTextColor => m_MainTextColor;
    public TMP_FontAsset TextFont => m_TextFont;
}
