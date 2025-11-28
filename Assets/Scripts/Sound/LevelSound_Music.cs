using UnityEngine.SceneManagement;

public class LevelSound_Music : LevelSound
{
    protected override AudioSourceType sourceType => AudioSourceType.Music;
    protected override void PlayOnLoadScene(Scene scene, LoadSceneMode mode)
    {
        base.PlayOnLoadScene(scene, mode);
        if (scene.name == levelManager.MainMenuSceneName)
            soundPlayer.PlaySound(sourceType, Sound.Music1);
        else if (scene.name == "Scene 1")
            soundPlayer.PlaySound(sourceType, Sound.Music2);
        if (scene.name == "Scene 2")
            soundPlayer.PlaySound(sourceType, Sound.Music3);
    }
}
