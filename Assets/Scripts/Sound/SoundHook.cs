using UnityEngine;
public class SoundHook : MonoBehaviour
{
    public Sound m_Sound;
    public void Play() { m_Sound.PlaySound(); }
}
