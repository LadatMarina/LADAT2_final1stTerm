using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private LevelGrid levelGrid;
    private Snake snake;

    private bool isPaused;
    
    private void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Debug.LogError("There is more than one Instance");
        }

        Instance = this;
    }
    
    private void Start()
    {
        SoundManager.CreateSoundManagerGameObject();
        
        // Configuración de la cabeza de serpiente
        GameObject snakeHeadGameObject = new GameObject("Snake Head");
        SpriteRenderer snakeSpriteRenderer = snakeHeadGameObject.AddComponent<SpriteRenderer>();
        snakeSpriteRenderer.sprite = GameAssets.Instance.snakeHeadSprite;
        snake = snakeHeadGameObject.AddComponent<Snake>();
        
        // Configurar el LevelGrid
        levelGrid = new LevelGrid(20, 20);
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);

        // Inicializo tema score
        Score.InitializeStaticScore();

        //parent arrows to snakehead
        GameAssets.Instance.arrowParent.transform.SetParent(snakeHeadGameObject.transform);
        HidePossibledirections();
        isPaused = false;


    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     Loader.Load(Loader.Scene.Game);
        // }
        
        // Lógica de Pause con tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
                ResumeGame();
            }
            else
            {
                SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
                PauseGame();
            }
        }
        //shortcut for win
        if (Input.GetKeyDown(KeyCode.N))
        {
            snake.ChangeState(2);
        }
        //shortcut for loose
        if (Input.GetKeyDown(KeyCode.M))
        {
            snake.ChangeState(1);
        }

        if (Input.GetKeyDown(KeyCode.U)) {

            GameAssets.Instance.arrowsArray[3].gameObject.SetActive(true);
            Debug.Log("estic pitjant sa tecla que activa sa fletxa UP");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("estic pitjant sa tecla que activa sa fletxa DOWN");
            GameAssets.Instance.arrowsArray[2].gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameAssets.Instance.arrowsArray[0].gameObject.SetActive(true);
            Debug.Log("estic pitjant sa tecla que activa sa fletxa LEFT");

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("estic pitjant sa tecla que activa sa fletxa RIGHT");

            GameAssets.Instance.arrowsArray[1].gameObject.SetActive(true);
        }
    }

    public void SnakeDied()
    {
        GameOverUI.Instance.Show(Score.TrySetNewHighScore());
        SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        PauseUI.Instance.Show();
        isPaused = true;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        PauseUI.Instance.Hide();
        isPaused = false;
    }

    //if the index of my direction doesen't match, show all the arrow (this show all the arrows minus my direction)
    public void ShowPossibleDirections(Direction wrongDirection)
    {
        Time.timeScale *= 0.5f;

        Debug.Log("no pot anar cap a "+ wrongDirection + "  index:" +(int)wrongDirection);

        for (int i = 0; i < GameAssets.Instance.arrowsArray.Length; i++)
        {
            //Debug.Log(i);

            if (i != (int)wrongDirection)
            {
                GameAssets.Instance.arrowsArray[i].gameObject.SetActive(true); //NO ME FA ES SET ACTIVE, SA LÒGIA DELS INDEX SI QUE FUNCIONA
                Debug.Log("mostra index: " + i +"  que es sa direccio:"+ (Direction)i );

            }
        }
    }

    public void HidePossibledirections()
    {
        foreach (GameObject go in GameAssets.Instance.arrowsArray)
        {
            if (go != null)
            {
                go.SetActive(false);
            }
        }
        Time.timeScale = 1;
    }

    public void Winner()
    {
        GameAssets.Instance.winnerPanel.SetActive(true);
        SoundManager.PlaySound(SoundManager.Sound.WinnerSound);
    }
    
}
