using TMPro;
using UnityEngine;

public class UIDisplayRound : MonoBehaviour, IDependency<LevelStateTracker>
{
    private TextMeshProUGUI textMeshProUGUI;
    private LevelStateTracker levelStateTracker;
    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
    private void Awake() => textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    private void Start() => levelStateTracker.RoundActionStarted += OnRoundActionStarted;
    private void OnDestroy() => levelStateTracker.RoundActionStarted -= OnRoundActionStarted;
    private void OnRoundActionStarted() => textMeshProUGUI.text = $"Round {levelStateTracker.CurrentRound}";
}
