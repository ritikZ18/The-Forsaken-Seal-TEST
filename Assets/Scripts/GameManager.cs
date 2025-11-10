using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public bool gameStarted = false;
    public bool gameOver = false;
    
    [Header("UI References")]
    public GameObject startMenu;
    public GameObject gameUI;
    public GameObject gameOverMenu;
    public Text scoreText;
    public Text waveText;
    
    [Header("Game References")]
    public PlayerHealth playerHealth;
    // public MobSpawner mobSpawner; // Removed - no longer needed
    
    [Header("Game Stats")]
    public int currentScore = 0;
    public int currentWave = 1;
    public int enemiesKilled = 0;
    
    public static GameManager Instance { get; private set; }
    
    public System.Action OnGameStart;
    public System.Action OnGameOver;
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnWaveChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        Time.timeScale = 0f;
        ShowStartMenu();
        
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += HandlePlayerDeath;
        }
    }
    
    void Update()
    {
        if (!gameStarted && Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
        
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    
    public void StartGame()
    {
        gameStarted = true;
        gameOver = false;
        Time.timeScale = 1f;
        
        if (startMenu != null) startMenu.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
        
        // if (mobSpawner != null)
        //     mobSpawner.enabled = true;
        
        OnGameStart?.Invoke();
        
        Debug.Log("Game Started!");
    }
    
    void HandlePlayerDeath()
    {
        if (!gameOver)
        {
            GameOver();
        }
    }
    
    public void GameOver()
    {
        gameOver = true;
        Time.timeScale = 0f;
        
        if (gameUI != null) gameUI.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(true);
        
        // if (mobSpawner != null)
        //     mobSpawner.enabled = false;
        
        OnGameOver?.Invoke();
        
        Debug.Log("Game Over!");
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        UpdateScoreUI();
    }
    
    public void EnemyKilled()
    {
        enemiesKilled++;
        AddScore(10);
        
        if (enemiesKilled % 5 == 0)
        {
            NextWave();
        }
    }
    
    void NextWave()
    {
        currentWave++;
        OnWaveChanged?.Invoke(currentWave);
        UpdateWaveUI();
        
        // if (mobSpawner != null)
        // {
        //     mobSpawner.mobsToSpawn += 2;
        // }
        
        Debug.Log($"Wave {currentWave}!");
    }
    
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }
    
    void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {currentWave}";
        }
    }
    
    void ShowStartMenu()
    {
        if (startMenu != null) startMenu.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
    }
    
    public bool IsGameActive => gameStarted && !gameOver;
}
