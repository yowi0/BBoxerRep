using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static string PlayerName { get; set; }
    public static int Score { get; set; }
    public static int HighScore { get; set; }

    public static void ResetScore()
    {
        Score = 0;
    }

    public static void UpdateHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
        }
    }
}
