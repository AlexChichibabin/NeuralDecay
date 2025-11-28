using System;
using UnityEngine;

public class MapCompletion : MonoBehaviour
{
    [Serializable]
    private class SeasonScore
    {
        [Serializable]
        public class LevelScore
        {
            public Episode episode;
            public int levelScore;
        }
        public LevelScore[] levelsInSeason;
        public int seasonScore;
    }

    [SerializeField] private SeasonScore[] m_CompletionDataPerSeason;

    private LevelSequenceController levelSequenceController;

    public const string m_FileName = "completion.dat";
    public string FileName => m_FileName;
    public void Construct(LevelSequenceController obj) => levelSequenceController = obj;
    public int TotalScores { private set; get; }

    private void Awake()
    {
        Saver<SeasonScore[]>.TryLoad(m_FileName, ref m_CompletionDataPerSeason);
        TotalScores = 0;
        if (m_CompletionDataPerSeason != null)
        {
            foreach (var season in m_CompletionDataPerSeason)
            {
                season.seasonScore = 0;
                foreach (var episode in season.levelsInSeason)
                {
                    TotalScores += episode.levelScore;
                    season.seasonScore += episode.levelScore;
                }
            }
        }
    }
    public void SaveEpisodeResult(int levelScore)
    {
        foreach (var season in m_CompletionDataPerSeason)
        {
            foreach (var episode in season.levelsInSeason)
            {   // Сохранение новых очков прохожения 
                if (episode.levelScore < levelScore)
                {
                    TotalScores += levelScore - episode.levelScore;
                    season.seasonScore += levelScore - episode.levelScore;
                    episode.levelScore = levelScore;

                    Saver<SeasonScore[]>.Save(m_FileName, m_CompletionDataPerSeason);
                }
            }
            print($"Episode complete with score {levelScore}");
        }
    }
    public int GetEpisodeScore(Episode episode)
    {
        foreach (var season in m_CompletionDataPerSeason)
        {
            foreach (var data in season.levelsInSeason)
            {
                if (data.episode == episode)
                {
                    return data.levelScore;
                }
            }
        }
        return 0;
    }
    public void SeasonCompletionInitialize(int SeasonsLength)
    {
        m_CompletionDataPerSeason = new SeasonScore[SeasonsLength];
        for (int i = 0; i < SeasonsLength; i++)
        {
            m_CompletionDataPerSeason[i] = new SeasonScore();
        }
    }
    public void RaceCompletionInitialize(int SeasonsIndex, int RacesLength)
    {
        m_CompletionDataPerSeason[SeasonsIndex].levelsInSeason = new SeasonScore.LevelScore[RacesLength];
    }
    public void ResetProgress()
    {
        foreach (var season in m_CompletionDataPerSeason)
        {
            foreach (var race in season.levelsInSeason)
            {
                
            }
        }
        FileHandler.Reset(m_FileName);

        TotalScores = 0;
        foreach (var season in m_CompletionDataPerSeason)
        {
            season.seasonScore = 0;
            foreach (var race in season.levelsInSeason)
            {
                race.levelScore = 0;
            }
        }

    }
}
