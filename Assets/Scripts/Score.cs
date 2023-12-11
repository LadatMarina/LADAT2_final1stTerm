using System;
using UnityEngine;

public static class Score
{
    public const string HIGH_SCORE = "highScore"; // Clave en PlayerPrefs
    public const int POINTS = 100; // Cantidad de puntos que ganamos al comer comida

    public static event EventHandler OnHighScoreChange;

    private static int score; // Puntuación del jugador

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE, 0);
    }

    public static bool TrySetNewHighScore()
    {
        int highScore = GetHighScore();
        if (score > highScore)
        {
            // Modificamos el High Score
            PlayerPrefs.SetInt(HIGH_SCORE, score);
            PlayerPrefs.Save();
            
            if (OnHighScoreChange != null)
            {
                OnHighScoreChange(null, EventArgs.Empty);
            }
            
            return true;
        }
        
        return false;
    }

    public static void InitializeStaticScore()
    {
        OnHighScoreChange = null;
        score = 0;
        AddScore(0);
        ScoreUI.Instance.UpdateHighScoreText();
    }
    
    public static int GetScore()
    {
        return score;
    }
    
    public static void AddScore(int pointsToAdd)
    {
        score += pointsToAdd;
        ScoreUI.Instance.UpdateScoreText(score);
    }
}
