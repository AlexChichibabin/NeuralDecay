using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    public class UIDropPlace : MonoBehaviour//, IDependency<MoveManager>
    {
        [SerializeField] private TextMeshProUGUI m_ScoresText;
        [SerializeField] private TextMeshProUGUI m_FrontText;
        [SerializeField] private Image m_FrontImage;
        [SerializeField] private Color stormColor;
        [SerializeField] private Color fireColor;
        [SerializeField] private Color cyberColor;
        [SerializeField] private Sprite stormSprite;
        [SerializeField] private Sprite fireSprite;
        [SerializeField] private Sprite cyberSprite;

        private DropPlace dropPlace;

        private void Awake()
        {
            dropPlace = transform.parent.GetComponentInChildren<DropPlace>();
        }
        public void UpdateUIInfo()
        {
            FrontType front = dropPlace.FrontType;
            if (front == FrontType.FireSupport)
                m_FrontText.text = "Fire";
            else m_FrontText.text = front.ToString();

            if (front == FrontType.Storm)
            {
                m_FrontText.color = stormColor;
                m_FrontImage.sprite = stormSprite;
            }
            if (front == FrontType.FireSupport)
            {
                m_FrontText.color = fireColor;
                m_FrontImage.sprite = fireSprite;
            }
            if (front == FrontType.Cyber)
            {
                m_FrontText.color = cyberColor;
                m_FrontImage.sprite = cyberSprite;
            }
            m_FrontImage.GetComponent<AspectRatioFitter>().aspectRatio = 1; //Hardcode
                
            m_ScoresText.text = dropPlace.GetComponent<DropPlaceTracker>().PowerCount.ToString();
        }
    }
}