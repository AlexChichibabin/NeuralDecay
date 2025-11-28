using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    [RequireComponent(typeof(CardDisplace))]
    public class Card : MonoBehaviour, IScriptableObjectProperty, IDependency<TableCardTracker>, IDependency<LevelManager>
    {
        [SerializeField] private CardInfo info; //DebugOnly
        [SerializeField] private Image cardPicture;
        [SerializeField] private Image frontTypePicture;
        [SerializeField] private Image abilityPicture;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI cardPowerText;
        [SerializeField] private Image m_StrokeImage;

        private Color m_DefaultStrokeColor;
        private TableCardTracker tableCardTracker;
        private LevelManager levelManager;
        private PlayerNum controlPlayer;
        private DiscardPile discardPile;
        private CardDrag cardDrag;
        private int power;
        private bool abilityIsActive = true;

        public CardInfo Info => info;
        public PlayerNum ControlPlayer => controlPlayer;
        public int Power => power;
        public void Construct(TableCardTracker obj) => tableCardTracker = obj;
        public void Construct(LevelManager obj) => levelManager = obj;
        public Color DefaultStrokeColor => m_DefaultStrokeColor;
        public bool AbilityIsActive => abilityIsActive;
        public LevelManager LevelManager => levelManager;

        private void Awake()
        {
            if(info != null) SetPower(info.DefaultPower);
            if (levelManager != null)
                m_DefaultStrokeColor = levelManager.MainGreenColor;

            TryGetComponent<CardDrag>(out cardDrag);
        }
        public void ApplyProperty(ScriptableObject property)
        {
            if (property == null) return;

            if (property is CardInfo)
            {
                CardInfo cardInfo = property as CardInfo;

                gameObject.name = cardInfo.CardName;
                cardPicture.color = cardInfo.PictureColor;
                cardNameText.text = cardInfo.CardName;
                cardPowerText.text = cardInfo.DefaultPower.ToString();
                cardPicture.sprite = cardInfo.PictureSprite;
                cardPicture.GetComponent<AspectRatioFitter>().aspectRatio = cardInfo.AspectRatio;
                //factionPicture.sprite = cardInfo.FractionSprite;
                frontTypePicture.sprite = cardInfo.FrontTypeSprite;
                abilityPicture.sprite = cardInfo.AbilitySprite;
                if (cardInfo.CardTypeBase == CardTypeBase.EliteType) cardPowerText.color = cardInfo.ElitePowerColor;

                power = cardInfo.DefaultPower;
                
                info = cardInfo;
            }
        }
        public void Discard()
        {
            cardDrag.CurrentParent = tableCardTracker.GetDiscardPile(controlPlayer).Container;
            transform.SetParent(cardDrag.CurrentParent, false);
            transform.localPosition = new Vector3(0, 0, 0);
        }
        public void Restore()
        {
            cardDrag.CurrentParent = tableCardTracker.GetDropPlaceCardTracker(controlPlayer, FrontType.Common).transform;
            transform.SetParent(cardDrag.CurrentParent);
            SetPower(info.DefaultPower);
            SetAbilityActive(true);
        }
        public void SetDiscardPile(DiscardPile discardPile) => this.discardPile = discardPile;
        public void SetOwner(PlayerNum playerNumber) => this.controlPlayer = playerNumber;
        public void SetStrokeColor(Color color) => m_StrokeImage.color = color;
        public void SetAbilityActive(bool isActive) => abilityIsActive = isActive;
        public void SetPower(int power)
        {
            this.power = power;
            cardPowerText.text = power.ToString();
            if (info.CardTypeBase == CardTypeBase.EliteType) return;
            if (power < Info.DefaultPower) cardPowerText.color = levelManager.MainRedColor;
            if (power == Info.DefaultPower) cardPowerText.color = Color.white;
            if (power > Info.DefaultPower) cardPowerText.color = levelManager.MainGreenColor;
        }
    }
}