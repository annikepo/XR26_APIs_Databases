using UnityEngine;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System;

namespace Databases
{
    /// <summary>
    /// Game Data Manager for handling SQLite database operations
    /// </summary>
    public class GameDataManager : MonoBehaviour
    {
        [Header("Database Configuration")]
        [SerializeField] private string databaseName = "GameData.db";

        private SQLiteConnection _database;
        private string _databasePath;

        // Singleton pattern
        public static GameDataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                _databasePath = Path.Combine(Application.persistentDataPath, databaseName);
                _database = new SQLiteConnection(_databasePath);
                _database.CreateTable<HighScore>();
                Debug.Log($"✅ Database initialized at: {_databasePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to initialize database: {ex.Message}");
            }
        }

        #region High Score Operations

        public void AddHighScore(string playerName, int score, string levelName = "Default")
        {
            try
            {
                var newScore = new HighScore
                {
                    PlayerName = playerName,
                    Score = score,
                    LevelName = levelName,
                    AchievedAt = DateTime.Now
                };

                _database.Insert(newScore);
                Debug.Log($"🏆 High score added: {playerName} - {score} pts (Level: {levelName})");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to add high score: {ex.Message}");
            }
        }

        public List<HighScore> GetTopHighScores(int limit = 10)
        {
            try
            {
                return _database.Table<HighScore>()
                                .OrderByDescending(h => h.Score)
                                .Take(limit)
                                .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to get high scores: {ex.Message}");
                return new List<HighScore>();
            }
        }

        public List<HighScore> GetHighScoresForLevel(string levelName, int limit = 10)
        {
            try
            {
                return _database.Table<HighScore>()
                                .Where(h => h.LevelName == levelName)
                                .OrderByDescending(h => h.Score)
                                .Take(limit)
                                .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to get level high scores: {ex.Message}");
                return new List<HighScore>();
            }
        }

        #endregion

        #region Utility

        public int GetHighScoreCount()
        {
            try
            {
                return _database.Table<HighScore>().Count();
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to count high scores: {ex.Message}");
                return 0;
            }
        }

        public void ClearAllHighScores()
        {
            try
            {
                _database.DeleteAll<HighScore>();
                Debug.Log("🧹 All high scores cleared.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to clear high scores: {ex.Message}");
            }
        }

        private void OnApplicationQuit()
        {
            _database?.Close();
        }

        #endregion
    }
}
