using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelSequenceController : SingletonBase<LevelSequenceController>
{
    public static string MainMenuSceneNickname = "MainMenu";
    public static string MapSceneNickname = "LevelMap";

    public UnityEvent<string> OnEndScene;
    public Episode CurrentEpisode { get; private set; }
    public int CurrentLevel { get; private set; }
    public bool LastLevelResult { get; private set; }
    public void LoadMenuScene() => StartCoroutine(LoadSceneNumerator(MainMenuSceneNickname, true));
    public void StartEpisode(Episode episode) // Load named episode
    {
        CurrentEpisode = episode;
        CurrentLevel = 0;

        if (CurrentEpisode.Levels.Length > 0 && CurrentEpisode.Levels[CurrentLevel] != null)
            StartCoroutine(LoadSceneNumerator(episode.Levels[CurrentLevel], true));
    }
    public void RestartLevel() // Load current level
    {
        if (CurrentEpisode == null)
        {
            StartCoroutine(LoadSceneNumerator(SceneManager.GetActiveScene().name, false));
            Debug.Log("LevelSequenceController.CurrentLevel is null");
        }
        else StartCoroutine(LoadSceneNumerator(CurrentEpisode.Levels[CurrentLevel], false));
    }
    public void FinishCurrentLevel(bool success)
    {
        LastLevelResult = success;
    }
    public void AdvanceLevel() // Load next level in episode. Load main menu if there is no
    {
        if (CurrentEpisode)
        {
            CurrentLevel++;

            if (CurrentEpisode.Levels.Length <= CurrentLevel)
                StartCoroutine(LoadSceneNumerator(MainMenuSceneNickname, true));
            else
                StartCoroutine(LoadSceneNumerator(CurrentEpisode.Levels[CurrentLevel], true));
        }
        else
            StartCoroutine(LoadSceneNumerator(MainMenuSceneNickname, true));
    }
    IEnumerator LoadSceneNumerator(string sceneName, bool hasToInvokeEvent)
    {
        if (hasToInvokeEvent == true) OnEndScene.Invoke(sceneName);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }
}