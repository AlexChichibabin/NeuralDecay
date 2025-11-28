using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsLoader : MonoBehaviour
{
    [SerializeField] private Setting[] allSettings;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnLoadScene;
        for (int i = 0; i < allSettings.Length; i++)
        {
            allSettings[i].Load();
            allSettings[i].Apply();
        }
    }
    private void OnDestroy() => SceneManager.sceneLoaded -= OnLoadScene;
    private void OnLoadScene(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < allSettings.Length; i++)
        {
            allSettings[i].Load();
            allSettings[i].Apply();
        }
    }
}