using TMPro;
using UnityEngine;
using CardGame;
using System.Collections;

public class UIEndLevelPanel : MonoBehaviour, IDependency<LevelStateTracker>, IDependency<LevelSequenceController>
{
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_ResultSpeakText;
    [SerializeField] private string[] m_TitleString;
    [SerializeField] private string[] m_RelultString;
    [SerializeField] private GameObject m_NextButton;

    private LevelStateTracker levelStateTracker;
    private LevelSequenceController levelSequenceController;

    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
    public void Construct(LevelSequenceController obj) => levelSequenceController = obj;
    private void Awake()
    {
        levelStateTracker.LevelEnded += OnLevelEnded;
    }
    private void Start()
    {
        //StartCoroutine(SetUnactiveNumerator());
    }
    IEnumerator SetUnactiveNumerator()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        levelStateTracker.LevelEnded -= OnLevelEnded;
    }
    public void OnLevelEnded(PlayerNum playerWin)
    {
        //Application.Quit();
        bool isWinner = playerWin == PlayerNum.Player_1 ? true : false;
        DefineResult(isWinner);
        gameObject.SetActive(true);
        if (levelSequenceController.CurrentEpisode == null)
        {
            Debug.Log("levelSequenceController.CurrentEpisode is null");
            m_NextButton.SetActive(false);
            return;
        }
        if (levelSequenceController.CurrentEpisode.Levels.Length <= levelSequenceController.CurrentLevel + 1)
            m_NextButton.SetActive(false);
    }
    private void DefineResult(bool isWinner)
    {
        if (isWinner == true)
        {
            m_TitleText.text = m_TitleString[0];
            m_ResultSpeakText.text = m_RelultString[0];
        }
        else
        {
            m_TitleText.text = m_TitleString[1];
            m_ResultSpeakText.text = m_RelultString[1];
        }
    }
}
