using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    // Singleton
    public static GameOverUI Instance { get; private set; }
    
    [SerializeField] private Button restartButton;
    
    [SerializeField] private TextMeshProUGUI messsageText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one Instance");
        }

        Instance = this;
        
        restartButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.Game);
            SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
        });

        Hide();
    }

    public void Show(bool hasNewHighScore)
    {
        UpdateScoreAndHighScore(hasNewHighScore);
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateScoreAndHighScore(bool hasNewHighScore)
    {
        scoreText.text = "YOUR SCORE: " + Score.GetScore().ToString();
        highScoreText.text = hasNewHighScore ? "NEW HIGH SCORE: " + Score.GetHighScore().ToString() : "LAST HIGH SCORE: " + Score.GetHighScore().ToString();
        messsageText.text = hasNewHighScore ? "CONGRATULATIONS" : "DON'T WORRY, NEXT TIME";
    }
}
