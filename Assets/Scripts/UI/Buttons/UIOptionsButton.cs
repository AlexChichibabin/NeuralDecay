using CardGame;
using UnityEngine.EventSystems;

public class UIOptionsButton : UISelectableButton, IDependency<MoveManager>, IDependency<LevelStateTracker>
{
    private LevelStateTracker levelStateTracker;
    private MoveManager moveManager;
    private UIPrefabButtonConnector connector;

    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
    public void Construct(MoveManager obj) => moveManager = obj;

    protected new void Awake()
    {
        base.Awake();
        connector = GetComponent<UIPrefabButtonConnector>();
        levelStateTracker.LevelEnded += OnLevelEnded;
        levelStateTracker.RoundActionStarted += OnRoundActionStarted;
    }
    private void OnDestroy()
    {
        levelStateTracker.LevelEnded -= OnLevelEnded;
        levelStateTracker.RoundActionStarted += OnRoundActionStarted;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        SetFocus();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        SetUnfocus();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        connector.Pause();
    }
    private void OnRoundActionStarted() => enabled = true;
    private void OnLevelEnded(PlayerNum playerWin) => enabled = false;
}
