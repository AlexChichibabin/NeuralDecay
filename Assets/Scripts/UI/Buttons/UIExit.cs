using TMPro;
using UnityEngine;

public class UIExit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GameObject exitConfirmPanel;
    [SerializeField]
    private string[] questionString = new string[]
    {
            "Are you sure?",
            "Really?"
    };
    private int yesCount = 0;
    private void Start()
    {
        questionText.text = questionString[yesCount];
    }
    public void No()
    {
        ResetQuestion();
        gameObject.SetActive(false);
    }
    public void Yes()
    {
        if (yesCount == questionString.Length - 1) ExitGame();
        else NextText();
    }
    private void NextText()
    {
        yesCount++;
        questionText.text = questionString[yesCount];
    }
    private void ExitGame()
    {
        Application.Quit();
    }
    private void ResetQuestion()
    {
        yesCount = 0;
        questionText.text = questionString[yesCount];
    }
}