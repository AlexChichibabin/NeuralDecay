using CardGame;
using UnityEngine;

public class SceneDependenciesContainer : Dependency
{
    [SerializeField] private LevelStateTracker levelStateTracker;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private MoveManager movesManager;
    [SerializeField] private TableCardTracker tableScoreTracker;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private SceneRestarter sceneRestarter;

    protected override void BindAll(MonoBehaviour monoBehaviourInScene)
    {
        Bind<LevelStateTracker>(levelStateTracker, monoBehaviourInScene);
        Bind<LevelManager>(levelManager, monoBehaviourInScene);
        Bind<MoveManager>(movesManager, monoBehaviourInScene);
        Bind<TableCardTracker>(tableScoreTracker, monoBehaviourInScene);
        Bind<SceneLoader>(sceneLoader, monoBehaviourInScene);
        Bind<SceneRestarter>(sceneRestarter, monoBehaviourInScene);
    }
    private void Awake()
    {
        FindAllObjectsToBind();
    }
}