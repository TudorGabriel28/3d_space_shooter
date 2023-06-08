using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public event Action<int> ScoreChanged = delegate(int i) {  };
    public event Action<int> HighScoreChanged = delegate(int i) {  };

    public int Score { get; private set; }
    public int HighScore { get; private set; }

    const string HighScoreKey = "HighScore";

    private List<PlayerScore> playerScores = new();
    private const string leaderboardKey = "LeaderboardData"; // Key to save and load leaderboard data in PlayerPrefs

    public void ResetScore()
    {
        Score = 0;
        ScoreChanged(Score);
    }

    public void AddPoints(int points)
    {
        if (GameManager.Instance.GameState == GameState.GameOver) return;
        Score += points;
        ScoreChanged(Score);
        if (Score <= HighScore) return;
        HighScore = Score;
        HighScoreChanged(HighScore);
    }

    private void AddPlayerScoreToLeaderboard()
    {
        PlayerScore newScore = new(PlayerPrefs.GetString("CurrentPlayer"), Score);
        playerScores.Add(newScore);

        // Sort the scores in descending order
        playerScores.Sort((a, b) => b.score.CompareTo(a.score));

        // Truncate the list to keep only the top 5 scores
        if (playerScores.Count > 5)
            playerScores = playerScores.GetRange(0, 5);

        SaveLeaderboardData();

    }

    // Method to save the leaderboard data to PlayerPrefs
    private void SaveLeaderboardData()
    {
        string data = "";

        foreach (PlayerScore score in playerScores)
        {
            data += $"{score.playerName}: {score.score};";
        }

        PlayerPrefs.SetString(leaderboardKey, data);
        PlayerPrefs.Save();
    }

    // Method to load the leaderboard data from PlayerPrefs
    private void LoadLeaderboardData()
    {
        if (PlayerPrefs.HasKey(leaderboardKey))
        {
            string data = PlayerPrefs.GetString(leaderboardKey);
            string[] scoreStrings = data.Split(';');

            playerScores.Clear();

            foreach (string scoreString in scoreStrings)
            {
                if (!string.IsNullOrEmpty(scoreString))
                {
                    string[] scoreData = scoreString.Split(':');
                    if (scoreData.Length == 2)
                    {
                        string playerName = scoreData[0];
                        int score = int.Parse(scoreData[1]);
                        PlayerScore scoreObj = new PlayerScore(playerName, score);
                        playerScores.Add(scoreObj);
                    }
                }
            }
        }
    }

    private void ClearLeaderboard()
    {
        playerScores.Clear();
        PlayerPrefs.DeleteKey(leaderboardKey);
        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        LoadLeaderboardData();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        HighScoreChanged(HighScore);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
        AddPlayerScoreToLeaderboard();
    }
}
