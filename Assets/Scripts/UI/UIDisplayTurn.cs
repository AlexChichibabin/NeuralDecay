using CardGame;
using TMPro;
using UnityEngine;

public class UIDisplayTurn : MonoBehaviour, IDependency<MoveManager>
{
    private TextMeshProUGUI textMeshProUGUI;
    private MoveManager moveManager;
    public void Construct(MoveManager obj) => moveManager = obj;
    private void Awake() => textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    private void Start() => moveManager.TurnSwitched.AddListener(OnTurnSwitched);
    private void OnDestroy() => moveManager.TurnSwitched.RemoveListener(OnTurnSwitched);
    private void OnTurnSwitched(PlayerNum player) => textMeshProUGUI.text = player == PlayerNum.Player_1? "Your turn" : "Enemy turn";
}
