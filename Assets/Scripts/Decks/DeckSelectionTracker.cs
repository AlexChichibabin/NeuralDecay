using CardGame;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelectionTracker : MonoBehaviour, IDependency<LevelManager>, IDependency<DeckManager>
{
    private CardInfoPool cardInfoPool;
    private SpawnObjectByPropertiesList spawnObjectByPropertiesList;
    private CardUnit[] deckInfo;
    private CardInfo[] deckProperties;
    private DropPlaceTracker dropPlaceTracker;
    private LevelManager levelManager;
    private bool isMain = false;
    
    public bool IsMain => isMain;
    public DropPlaceTracker DropPlaceTracker => dropPlaceTracker;
    public void Construct(LevelManager obj) => levelManager = obj;
    public void Construct(DeckManager obj)
    {
        cardInfoPool = obj.Pool;
        cardInfoPool.InitDictionary();
    }
    private void Awake()
    {
        TryGetComponent(out spawnObjectByPropertiesList);
        spawnObjectByPropertiesList.Parent.TryGetComponent(out dropPlaceTracker);
        dropPlaceTracker.GetComponent<DropSelectionDeck>().SetDeckSelectionTracker(this);
        if(SceneManager.GetActiveScene().name == "MainMenu") isMain = true;
        deckProperties = spawnObjectByPropertiesList.GetPropreties() as CardInfo[];
    }
    private void Start()
    {
        dropPlaceTracker.GetComponent<DropSelectionDeck>().SetDefaultDeck();
        Saver<CardUnit[]>.TryLoad(levelManager.DeckSaveFileName, ref deckInfo);
        if (deckInfo != null)
        {
            spawnObjectByPropertiesList.SpawnBy(ConverterInfoEnum.ConvertFromEnumToCardInfo(deckInfo, cardInfoPool));
            dropPlaceTracker.GetComponent<DropSelectionDeck>().ApplyCardDragsData();
        } 
    }
    [ContextMenu(nameof(ApplyDeck))]
    public void ApplyDeck() => StartCoroutine(ApplyNumerator());
    IEnumerator ApplyNumerator() 
    {
        yield return new WaitForEndOfFrame();

        CardDrag[] drags = dropPlaceTracker.GetChildCards();
        deckProperties = new CardInfo[drags.Length];
        deckInfo = new CardUnit[drags.Length];

        for (int i = 0; i < drags.Length; i++)
        {
            deckProperties[i] = drags[i].Card.Info;
            deckInfo[i] = deckProperties[i].CardUnit;
        }
            
        FileHandler.Reset(levelManager.DeckSaveFileName);
        for (int i = 0; i < deckInfo.Length; i++)
        {
            Debug.Log(deckInfo[i]);
        }

        Saver<CardUnit[]>.Save(levelManager.DeckSaveFileName, deckInfo);
        levelManager.SetDeck(deckInfo);
    }
}
