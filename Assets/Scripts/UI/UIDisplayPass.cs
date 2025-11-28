using CardGame;
using TMPro;
using UnityEngine;

public class UIDisplayPass : MonoBehaviour, IDependency<LevelStateTracker>, IDependency<LevelManager>
{
    [SerializeField] private PlayerNum controlPlayer;

    private TextMeshProUGUI textMeshProUGUI;
    private LevelStateTracker levelStateTracker;
    private LevelManager levelManager;

    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
    public void Construct(LevelManager obj) => levelManager = obj;

    private void Awake()
    {
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        levelStateTracker.PassStateChanged += OnPassStateChanged;
    }
    private void OnDestroy() => levelStateTracker.PassStateChanged -= OnPassStateChanged;
    private void OnPassStateChanged(PlayerNum player, bool isPassed)
    {
        if (player == controlPlayer) textMeshProUGUI.text = isPassed ? levelManager.IsPassedText : levelManager.IsNotPassedText;
    }
}
