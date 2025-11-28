using CardGame;
using UnityEngine.EventSystems;

public class UIUnitSelectableButton : UISelectableButton, IDependency<LevelStateTracker>
{
    private LevelStateTracker levelStateTracker;
    private UIPrefabButtonConnector connector;

    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;

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
    private void OnRoundActionStarted()
    {
        enabled = true;
    }
    private void OnLevelEnded(PlayerNum playerWin)
    {
        enabled = false;
    }
}
