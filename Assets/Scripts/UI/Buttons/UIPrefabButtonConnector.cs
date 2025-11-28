using UnityEngine;

public class UIPrefabButtonConnector : MonoBehaviour, IDependency<LevelSequenceController>, IDependency<Pauser>
{
    [SerializeField] private Episode episode;
    [SerializeField] private DropSelectionDeck dropSelectionDeck;

    private LevelSequenceController levelSequenceController;
    private Pauser pauser;

    public void Construct(LevelSequenceController obj) => levelSequenceController = obj;
    public void Construct(Pauser obj) => pauser = obj;
    public void StartBattle() => levelSequenceController.StartEpisode(episode);
    public void RestartBattle() => levelSequenceController.RestartLevel();
    public void LoadMenu() => levelSequenceController.LoadMenuScene();
    public void Advance() => levelSequenceController.AdvanceLevel();
    public void Pause() => pauser.Pause();
    public void ClearDeck() => dropSelectionDeck.DestroyChilds();
    public void SpawnDefaultDeck() => dropSelectionDeck.SpawnDefaultDeck();
}
