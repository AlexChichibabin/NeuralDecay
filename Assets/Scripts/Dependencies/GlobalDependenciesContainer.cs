using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalDependenciesContainer : Dependency
{
    [SerializeField] private Pauser pauser;
    [SerializeField] private MapCompletion mapCompletion;
    [SerializeField] private LevelSequenceController levelSequenceController;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private DeckManager deckManager;

    private static GlobalDependenciesContainer instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnLoadScene;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoadScene;
    }
    protected override void BindAll(MonoBehaviour monoBehaviourInScene)
    {
        Bind<Pauser>(pauser, monoBehaviourInScene);
        Bind<MapCompletion>(mapCompletion, monoBehaviourInScene);
        Bind<LevelSequenceController>(levelSequenceController, monoBehaviourInScene);
        Bind<SoundPlayer>(soundPlayer, monoBehaviourInScene);
        Bind<DeckManager>(deckManager, monoBehaviourInScene);
    }
    private void OnLoadScene(Scene arg0, LoadSceneMode arg1)
    {
        FindAllObjectsToBind();
    }
}