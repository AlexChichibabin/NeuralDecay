using CardGame;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, IDependency<LevelManager>
{
    private string MainMenuSceneTitle;
    private LevelManager levelManager;

    public void Construct(LevelManager obj) => levelManager = obj;
    public void LoadMainMenu() => SceneManager.LoadScene(MainMenuSceneTitle);
    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
