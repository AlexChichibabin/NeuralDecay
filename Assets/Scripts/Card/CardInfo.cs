using UnityEngine;
public enum CardUnit
{
    CerebralModule, // Mercenary, Cyber, 
    DataBase, // Mercenary, Cyber, 
    Hacker, // Mercenary, Cyber, 
    Operator, // Mercenary, FireSupport, 
    Sniper, // Mercenary, FireSupport, 
    Clients, // Mercenary, Storm, 
    Landsknecht, // Mercenary, Storm, 
    Raider, // Mercenary, Storm, 
    Stimulants, // Specials, DoubleEffect
    DesperateMeasures, // Specials, FieldEffect, DiscardAllFieldEffects
    Statics, // Specials, FieldEffect, OnCyberField
    Smokescreen, // Specials, FieldEffect, OnFireSupField
    SuppressingFire, // Specials, FieldEffect, OnStormField
    Shadow, // Specials, Displacer
    Moira,
    Hecate,
    EmptyHeaded,
    Beacon,
    Lich,
    Marauder,
    Aberration,
    ProphetOfUpgrade
}
public enum FrontType
{
    Common,
    Storm,
    FireSupport,
    Cyber,
}
public enum CardTypeBase
{
    CommonType, // Обычные карты
    EliteType, // На них не действуют эффекты
    SpecialType // Без силы, но с эффектами
}
public enum CardTypeEffect
{
    None,
    FrontEffect, // Понижает все исходные силы карт на фронте до 1
    DesperateMeasures, // Снимает эффекты фронтов (погоды)
    Retranslation, // Прибаляет всем картам по 1 единице, кроме себя и героев
    MultiplyRepeats, // Умножает дефолтную силу карт на каличество карт в группе
    DoubleFront, // Удваивает силу всех карт фронта
    CriticalError, // Уничтожает самый сильный отряд в ряду, если сумма силы отрядов в ряду превышает 10
    TacticalRetreat, // Шанс остаться на поле при поражении в раунде
    Anastasis // Возвращает верхний отряд из сброса (кроме героев) на руку
}
public enum CardTypeGroup
{
    None,
    _First,
    _Second,
    _Third,
    _Fourth,
    _Fifth,
}
public enum CardFaction
{
    None,
    Citadel,
    Flesh
}

[CreateAssetMenu]
public class CardInfo : ScriptableObject
{
    [Header("Head")]
    [SerializeField] private CardUnit m_CardUnit;

    [Header("Card design")]
    [SerializeField] private string m_CardName;
    [SerializeField] private Color m_PictureColor;
    [SerializeField] private Color m_ElitePowerColor;
    [SerializeField] private Sprite m_PictureSprite;
    [SerializeField] private float m_AspectRatio;
    [SerializeField] private Sprite m_FractionSprite;
    [SerializeField] private Sprite m_FrontTypeSprite;
    [SerializeField] private Sprite m_AbilitySprite;


    [Header("Card atiribute values")]
    [SerializeField] private int m_DefaultPower;

    [Header("Card atiribute states")]
    [SerializeField] private CardFaction m_Faction;
    [SerializeField] private FrontType m_Front;
    [SerializeField] private CardTypeBase m_CardTypeBase = CardTypeBase.CommonType;
    [SerializeField] private CardTypeEffect m_CardTypeEffect = CardTypeEffect.None;
    [SerializeField] private CardTypeGroup m_CardTypeGroup = CardTypeGroup.None;
    [SerializeField] private bool m_IsDisplacer = false;
    [SerializeField] private int m_CritErrorNeededEnemyPower = 0;
    [SerializeField] private int m_RestoreCardCount = 1;
    [SerializeField][Range(0f, 1f)] private float m_TacticalRetreatChance = 0f;

    public CardUnit CardUnit => m_CardUnit;
    public string CardName => m_CardName;
    public CardFaction Faction => m_Faction;
    public FrontType Front => m_Front;
    public int DefaultPower => m_DefaultPower;
    public Color PictureColor => m_PictureColor;
    public Color ElitePowerColor => m_ElitePowerColor;
    public float AspectRatio => m_AspectRatio;
    public Sprite PictureSprite => m_PictureSprite;
    public Sprite FractionSprite => m_FractionSprite;
    public Sprite FrontTypeSprite => m_FrontTypeSprite;
    public Sprite AbilitySprite => m_AbilitySprite;
    public CardTypeBase CardTypeBase => m_CardTypeBase;
    public CardTypeEffect CardTypeEffect => m_CardTypeEffect;
    public CardTypeGroup CardTypeGroup => m_CardTypeGroup;
    public bool IsDisplacer => m_IsDisplacer;
    public int CritErrorNeededEnemyPower => m_CritErrorNeededEnemyPower;
    public int RestoreCardCount => m_RestoreCardCount;
    public float TacticalRetreatChance => m_TacticalRetreatChance;
}
