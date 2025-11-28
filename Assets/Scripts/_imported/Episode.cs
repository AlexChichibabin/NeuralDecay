using UnityEngine;

[CreateAssetMenu]
public class Episode : ScriptableObject
{
    [SerializeField] private string m_EpisodeName;
    public string EpisodeName => m_EpisodeName;

    [SerializeField] private string[] m_Levels;
    public string[] Levels => m_Levels;

    [SerializeField] private Sprite m_PreviewImage;
    public Sprite PreviewImage => m_PreviewImage;
}
