using CardGame;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AudioSourceType
{
    Music,
    SFX,
    Ambience
}

public abstract class LevelSound : MonoBehaviour, IDependency<SoundPlayer>, IDependency<LevelManager>
{
    protected AudioSource audioSource;
    protected SoundPlayer soundPlayer;
    protected LevelManager levelManager;
    protected virtual AudioSourceType sourceType => AudioSourceType.Music;
    public AudioSourceType SourceType => sourceType;
    public void Construct(SoundPlayer obj) => soundPlayer = obj;
    public void Construct(LevelManager obj) => levelManager = obj;

    protected void Awake()
    {
        SceneManager.sceneLoaded += PlayOnLoadScene;
        audioSource = GetComponent<AudioSource>();
    }
    protected void OnDestroy() => SceneManager.sceneLoaded -= PlayOnLoadScene;
    protected void Start() { }
    protected virtual void PlayOnLoadScene(Scene scene, LoadSceneMode mode) { soundPlayer.AddAudioSource(sourceType, audioSource); }
}
