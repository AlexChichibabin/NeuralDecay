using CardGame;
using TMPro;
using UnityEngine;

public class UIDisplayWinCount : MonoBehaviour, IDependency<LevelStateTracker>
{
    [SerializeField] PlayerNum controlPlayer;

    private TextMeshProUGUI roundWinsText;
    private LevelStateTracker levelStateTracker;

    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
    public PlayerNum ControlPlayer => controlPlayer;
    private void Awake()
    {
        roundWinsText = transform.GetComponentInChildren<TextMeshProUGUI>();
        levelStateTracker.RoundCompleted += UpdateUIInfo;
    }
    private void Start() => UpdateUIInfo();
    private void OnDestroy() => levelStateTracker.RoundCompleted -= UpdateUIInfo;
    private void UpdateUIInfo() => 
        roundWinsText.text = "Wins: " + levelStateTracker.GetPlayerWinsCount(controlPlayer).ToString();
}
