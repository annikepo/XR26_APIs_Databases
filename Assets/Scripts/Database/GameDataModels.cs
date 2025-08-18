using System;
using SQLite;

namespace Databases
{
    [Table("HighScores")]
    public class HighScore
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string PlayerName { get; set; }

        public int Score { get; set; }

        [Indexed]
        public string LevelName { get; set; }

        public DateTime AchievedAt { get; set; }

        public float CompletionTime { get; set; }

        // Empty constructor for SQLite
        public HighScore()
        {
            AchievedAt = DateTime.UtcNow;
        }

        // Convenience constructor
        public HighScore(string playerName, int score, string levelName, float completionTime = 0f)
        {
            PlayerName = playerName;
            Score = score;
            LevelName = levelName;
            CompletionTime = completionTime;
            AchievedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{PlayerName}: {Score} points on {LevelName} ({CompletionTime:F2}s)";
        }
    }
}
