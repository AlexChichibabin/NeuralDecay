using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoadScene : MonoBehaviour
{
    private void Awake() => SceneManager.sceneLoaded += OnLoadScene;
    private void OnDestroy() => SceneManager.sceneLoaded -= OnLoadScene;
    private void OnLoadScene(Scene scene, LoadSceneMode mode) => StartCoroutine(LoadNextSceneNumerator());
    IEnumerator LoadNextSceneNumerator()
    {
        yield return new WaitForSeconds(1);
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
