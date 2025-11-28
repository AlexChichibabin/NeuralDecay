using CardGame;
using UnityEngine.EventSystems;

public class UIPassButton : UISelectableButton, IDependency<MoveManager>, IDependency<LevelStateTracker>
{
    private LevelStateTracker levelStateTracker;
    private MoveManager moveManager;

    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
    public void Construct(MoveManager obj) => moveManager = obj;

    protected new void Awake()
    {
        base.Awake();
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
    public void Pass() //MoveSystem ////////////////////////////
    {
        levelStateTracker.SetPassPlayerState(PlayerNum.Player_1, true);
        moveManager.SwitchTurn();
    }
    private void OnRoundActionStarted() => enabled = true;
    private void OnLevelEnded(PlayerNum playerWin) => enabled = false;
}
