using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    public class TempCard : Card, IDependency<LevelManager>
    {
        private Image m_Image;
        private Color m_DefaultColor;
        private DropSelectionDeck m_DropSelectionDeck;
        public Color DefaultColor => m_DefaultColor;
        public DropSelectionDeck DropSelectionDeck => m_DropSelectionDeck;

        private void Awake()
        {
            m_Image = GetComponent<Image>();
            m_DefaultColor = m_Image.color;
        }
        public void SetColor(Color color) => m_Image.color = color;
        public void SetDropSelectionDeck(DropSelectionDeck dropSelectionDeck) => m_DropSelectionDeck = dropSelectionDeck;
    }
}