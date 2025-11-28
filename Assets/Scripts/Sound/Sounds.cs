using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu()]
[Serializable]
public class Sounds : ScriptableObject
{
    [SerializeField] private AudioClip[] m_Sounds = new AudioClip[Enum.GetValues(typeof(Sound)).Length]; // Не должно быть null, иначе ошибки

    public AudioClip this[Sound s] => m_Sounds[(int)s];

#if UNITY_EDITOR
    [CustomEditor(typeof(Sounds))]
    [Serializable]
    public class SoundInspector : Editor
    {
        private static readonly int soundCount = Enum.GetValues(typeof(Sound)).Length;
        private new Sounds target => base.target? base.target as Sounds : null;

        public override void OnInspectorGUI()
        {
            if (target == null || target.m_Sounds == null) return;
            if (target.m_Sounds.Length < soundCount) { Array.Resize(ref target.m_Sounds, soundCount); }

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < target.m_Sounds.Length; i++)
            {
                target.m_Sounds[i] = EditorGUILayout.ObjectField($"{(Sound)i}",
                target.m_Sounds[i], typeof(AudioClip), false) as AudioClip;
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                Debug.Log("Sounds_SetDyrty()");
            }
        }
    }
#endif
}