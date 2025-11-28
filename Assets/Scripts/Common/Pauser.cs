using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Pauser : MonoBehaviour
{
    private bool isPause;

    public event UnityAction<bool> PauseStateChange;
    public bool IsPause => isPause;

    private void Awake() => SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    private void OnDestroy() => SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode) => UnPause();

    public void ChangePauseState()
    {
        if (isPause == true) UnPause();
        else Pause();
    }
    public void Pause()
    {
        if (isPause == true) return;

        isPause = true;
        PauseStateChange?.Invoke(isPause);
    }
    public void UnPause()
    {
        if (isPause == false) return;

        isPause = false;
        PauseStateChange?.Invoke(isPause);
    }
}