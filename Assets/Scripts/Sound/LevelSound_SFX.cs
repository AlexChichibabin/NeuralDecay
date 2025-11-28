using UnityEngine.SceneManagement;

public class LevelSound_SFX : LevelSound
{
    protected override AudioSourceType sourceType => AudioSourceType.SFX;
    protected override void PlayOnLoadScene(Scene scene, LoadSceneMode mode)
    {
        base.PlayOnLoadScene(scene, mode);
    }
}