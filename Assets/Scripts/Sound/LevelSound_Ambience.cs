using UnityEngine.SceneManagement;

public class LevelSound_Ambience : LevelSound
{
    protected override AudioSourceType sourceType => AudioSourceType.Ambience;
    protected override void PlayOnLoadScene(Scene scene, LoadSceneMode mode)
    {
        base.PlayOnLoadScene(scene, mode);
        if (scene.name == levelManager.MainMenuSceneName)
            soundPlayer.PlaySound(sourceType, Sound.Ambience);
        else
            soundPlayer.PlaySound(sourceType, Sound.Ambience);
    }
}
