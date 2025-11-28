using CardGame;
using TMPro;
using UnityEngine;

public class UIDisplayTotalScore : MonoBehaviour, IDependency<TableCardTracker>
{
    [SerializeField] PlayerNum controlPlayer;
    private TextMeshProUGUI totalScoreText;
    private TableCardTracker tableScoreTracker;
    public void Construct(TableCardTracker obj) => tableScoreTracker = obj;
    public PlayerNum ControlPlayer => controlPlayer;

    private void Awake()
    {
        totalScoreText = transform.GetComponentInChildren<TextMeshProUGUI>();
        tableScoreTracker.ScoreUpdated.AddListener(UpdateUIInfo);
    }
    private void OnDestroy() => tableScoreTracker.ScoreUpdated.RemoveListener(UpdateUIInfo);
    private void UpdateUIInfo(PlayerNum player, int totalScore)
    {
        if (player == controlPlayer)
            totalScoreText.text = totalScore.ToString();
    } 
}
