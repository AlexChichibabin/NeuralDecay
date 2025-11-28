using CardGame;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SoundHook))]
public class DropSelectionDeck : DropPlace
{
    [Header("Player choises")]
    [SerializeField] protected CardFaction faction;
    [SerializeField] protected Transform draggablePlane;
    [SerializeField] private DeckSelectionTracker deckSelectionTracker;
    [SerializeField] private bool isPool = false;
    [SerializeField] private Canvas canvas;

    private SpawnObjectByPropertiesList spawnObjectByPropertiesList;
    private ScriptableObject[] defaultCardInfo;
    private ScriptableObject[] spawnObject;
    private GameObject tempCard;
    private Vector3 defaultTempCardPosition;
    private bool isGotDefaultDeck = false;

    public bool IsPool => isPool;
    public Transform DraggablePlane => draggablePlane;
    public void SetDeckSelectionTracker(DeckSelectionTracker deckSelectionTracker) => this.deckSelectionTracker = deckSelectionTracker;
    public void UpdateDeck() => StartCoroutine(ApplyDataNumerator());

    private void Awake()
    {
        tempCard = canvas.GetComponentInChildren<CardDrag>().gameObject;
        defaultTempCardPosition = tempCard.transform.localPosition;
        soundHook = GetComponent<SoundHook>();
        dropPlaceTracker = GetComponent<DropPlaceTracker>();
        if(draggablePlane != null) draggablePlane.TryGetComponent(out spawnObjectByPropertiesList);

        if (isPool == true) // Было в старте
        {
            if (spawnObjectByPropertiesList != null)
            {
                spawnObject = spawnObjectByPropertiesList.GetPropreties();
                spawnObjectByPropertiesList.SpawnBy(spawnObject);
                ApplyCardDragsData();
            }
        }
    }
    protected override void Start() { } // Block parent's Start
    protected override void OnDestroy() { } // Block parent's OnDestroy
    public override void OnDrop(PointerEventData eventData) // Do when release mouse pointer within object area(moving mouse button up)
    {
        CardDrag card = eventData.pointerDrag.GetComponent<CardDrag>();

        if (card != null && CheckCardType(card.Card) == true)
        {
            DropSelectionDeck anotherDrop;
            card.DefaultParent.TryGetComponent(out anotherDrop);
            if (anotherDrop != null && anotherDrop.isPool == true) anotherDrop.UpdateDeck();
            card.CurrentParent = transform;
            card.DefaultParent = transform;
            soundHook.Play();
        }
        if(deckSelectionTracker != null) deckSelectionTracker.ApplyDeck();
        UpdateDeck();
    }
    public override void OnPointerEnter(PointerEventData eventData) // Do when mouse pointer enter object area
    {
        if (eventData.pointerDrag == null) return;

        CardDrag cardDrag = eventData.pointerDrag.GetComponent<CardDrag>();
        SetOnDragCardState(cardDrag);
    }
    public override void OnPointerExit(PointerEventData eventData) // Do when mouse pointer exit object area
    {
        if (eventData.pointerDrag == null) return;

        CardDrag cardDrag = eventData.pointerDrag.GetComponent<CardDrag>();

        SetOnDragCardState(cardDrag);
    }
    /// <summary>
    /// Set nesessary data to all spawned cards
    /// </summary>
    public void ApplyCardDragsData()
    {
        CardDrag[] drags = transform.GetComponentsInChildren<CardDrag>();
        for (int i = 0; i < drags.Length; i++)
        {
            if (drags[i] != null)
                drags[i].ThrowProperties(canvas, tempCard, defaultTempCardPosition, levelManager);
        }
    }
    /// <summary>
    /// Update card pool
    /// </summary>
    /// <returns></returns>
    IEnumerator ApplyDataNumerator()
    {
        yield return new WaitForEndOfFrame();

        if (isPool == true)
        {
            if (spawnObjectByPropertiesList != null)
            {
                spawnObject = spawnObjectByPropertiesList.GetPropreties();
                spawnObjectByPropertiesList.SpawnBy(spawnObject);
                ApplyCardDragsData();
            }
        }
    }
    /// <summary>
    /// Destroy all player cards. Clear deck. Without save.
    /// </summary>
    public void DestroyChilds()
    {
        for (int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
    }
    /// <summary>
    /// Spawn player deck from defaultDeck array
    /// </summary>
    public void SpawnDefaultDeck()
    {
        if (spawnObjectByPropertiesList == null) return;
        spawnObjectByPropertiesList.SpawnBy(defaultCardInfo);
        ApplyCardDragsData();
    }
    /// <summary>
    /// Set defaultCardInfo ScriptableObject array in awake from DeckSelectionTracker
    /// </summary>
    public void SetDefaultDeck()
    {
        if (isGotDefaultDeck == true) return;
        defaultCardInfo = spawnObjectByPropertiesList.GetPropreties();
        isGotDefaultDeck = true; // for once using
    }
    /// <summary>
    /// Check if player can drop this card on this place (dropPlace)
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    protected override bool CheckCardType(Card card) // 
    {
        //if (card.Info.Faction == faction || card.Info.Faction == CardFaction.None) return true;
        return true;
    }
    /// <summary>
    /// Set temp card parent and color while drag
    /// </summary>
    /// <param name="cardDrag"></param>
    protected override void SetOnDragCardState(CardDrag cardDrag) // 
    {
        if (cardDrag == null) return;

        TempCard tCard = cardDrag.TempCard.GetComponent<TempCard>();

        if (cardDrag.DefaultTempCardParent == transform)
        {
            cardDrag.DefaultTempCardParent = cardDrag.DefaultParent;
            tCard.SetColor(tCard.DefaultColor);
        }
        else
            cardDrag.DefaultTempCardParent = transform;

        tCard.SetDropSelectionDeck(this);
    }
}
