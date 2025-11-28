using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardGame
{
    public enum DealCardType { UpToMax, Discrete }
    public enum Faction { Citadel, Flesh }

    public class LevelManager : MonoBehaviour
    {
        [Header(("PowerCalculator"))]
        [SerializeField] private PowerCalculateType m_PowerCalculateType;

        [Header(("AI"))]
        [SerializeField] private AIType m_AIType;
        [SerializeField][Range(0f, 1f)] private float m_AIPassChance;

        [Header(("DealCards"))]
        [SerializeField] private DealCardType m_DealCardType;
        [SerializeField] private int cardCountToMax;
        [SerializeField] private int cardCountDiscrete;
        [SerializeField] private int maxHandCards;

        [Header("Design")]
        [SerializeField] private Color m_MainGreenColor;
        [SerializeField] private Color m_MainRedColor;
        [SerializeField] private Color m_MainBlueColor;

        [Header("PlayerSetting")]
        [SerializeField] private string m_DeckSaveFileName;
        [SerializeField] private Faction m_Faction;
        private CardUnit[] playerDeck;

        [Header("LevelData")]
        [SerializeField] private string m_MainMenuSceneName = "MainMenu";
        private string m_SceneName => SceneManager.GetActiveScene().name;

        [Header("DisplayDataText")]
        [SerializeField] private string m_IsPassedText;
        [SerializeField] private string m_IsNotPassedText;

        public AIType AIType => m_AIType;
        public DealCardType DealCardType => m_DealCardType;
        public PowerCalculateType PowerCalculateType => m_PowerCalculateType;
        public int CardCountToMax => cardCountToMax;
        public int CardCountDiscrete => cardCountDiscrete;
        public int MaxHandCards => maxHandCards;
        public float AIPassChance => m_AIPassChance;
        public Color MainGreenColor => m_MainGreenColor;
        public Color MainRedColor => m_MainRedColor;
        public Color MainBlueColor => m_MainBlueColor;

        public void SetDeck(CardUnit[] playerDeck) => this.playerDeck = playerDeck;
        public CardUnit[] GetDeck() => playerDeck;
        public string DeckSaveFileName => m_DeckSaveFileName;
        public Faction Faction => m_Faction;
        public string SceneName => m_SceneName;
        public string MainMenuSceneName => m_MainMenuSceneName;
        public string IsPassedText => m_IsPassedText;
        public string IsNotPassedText => m_IsNotPassedText;
    }
}