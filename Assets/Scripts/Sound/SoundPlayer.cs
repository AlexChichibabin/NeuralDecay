using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : SingletonBase<SoundPlayer>, IDependency<LevelSequenceController>
{
    [SerializeField] private Sounds m_Sounds;
    [SerializeField][Range(0f, 1f)] private float sceneFadeSpeed;
    
    private Dictionary<AudioSourceType, AudioSource> m_AudioSources;
    private AudioSource m_AudioSourse;
    private LevelSequenceController levelSequenceController;

    public void Construct(LevelSequenceController obj) => levelSequenceController = obj;

    private new void Awake()
    {
        base.Awake();

        m_AudioSourse = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnLoadScene;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoadScene;
        levelSequenceController.OnEndScene.RemoveListener(EndScene);
    }
    public void PlayOnce(Sound sound)
    {
        m_AudioSourse.PlayOneShot(m_Sounds[sound]);
    }
    public void PlaySound(AudioSourceType type, Sound sound)
    {
        m_AudioSources[type].resource = m_Sounds[sound];

        if (m_AudioSources[type].isPlaying == true) return;
        StartCoroutine(PlaySoundNumerator(type, sceneFadeSpeed, true));
    }
    public void StopMusic(AudioSourceType type)
    {
        StartCoroutine(PlaySoundNumerator(type, sceneFadeSpeed, false));
    }
    IEnumerator PlaySoundNumerator(AudioSourceType type, float fadeSpeed, bool isFadeIn)
    {
        if (fadeSpeed == 0 || m_AudioSources[type] == null || m_AudioSources[type].isPlaying == true)
        {
            yield return null;
            StopCoroutine(PlaySoundNumerator(type, fadeSpeed, isFadeIn));
        }

        if (isFadeIn == true)
        {
            m_AudioSources[type].Play();
            while (m_AudioSources[type].volume < 1f)
            {
                m_AudioSources[type].volume += fadeSpeed;
                yield return new WaitForSeconds(0.001f);
            }
        }
        else
        {
            while (m_AudioSources[type].volume > 0f)
            {
                m_AudioSources[type].volume -= fadeSpeed;
                yield return new WaitForSeconds(0.001f);
            }
            m_AudioSources[type].Stop();
        }
    }
    public void AddAudioSource(AudioSourceType type, AudioSource audioSource)
    {
        if (m_AudioSources == null) m_AudioSources = new Dictionary<AudioSourceType, AudioSource>();

        if (m_AudioSources.ContainsKey(type))
        {
            Debug.Log($"AudioSource type of {type} is already exist");
            return;
        }
        m_AudioSources.Add(type, audioSource);
    }
    private void OnLoadScene(Scene arg0, LoadSceneMode arg1) =>
    levelSequenceController.OnEndScene.AddListener(EndScene);
    private void EndScene(string nextSceneName)
    {
        foreach (var type in m_AudioSources.Keys)
            if (SceneManager.GetActiveScene().name != nextSceneName)
                StopMusic(type);
    }
}
