using TMPro;
using UnityEngine;


public class UIEditorApplier : MonoBehaviour
{
    [SerializeField] private TextProperties m_TextProperties;

    [ContextMenu(nameof(ApplyTextProperties))]
    public void ApplyTextProperties()
    {
        TextMeshProUGUI[] texts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (TextMeshProUGUI text in texts)
        {
            text.color = m_TextProperties.MainTextColor;
            text.font = m_TextProperties.TextFont;
        }
    }
}
