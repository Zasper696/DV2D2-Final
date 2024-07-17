// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Instancia única de GameManager

    [Header("Player Health Settings")]
    public int playerMaxHealth = 100; // Salud máxima del jugador
    private int playerCurrentHealth; // Salud actual del jugador

    private enum GameState { Start, WaveActive, Transition, GameOver } // Estados del juego
    private GameState currentState; // Estado actual del juego

    private int enemiesDefeatedInWave = 0; // Enemigos derrotados en la oleada actual
    private SpawnManager[] spawners; // Array de SpawnManagers
    private float nextRoundCountdown; // Cuenta atrás para la siguiente ronda

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame(); // Inicializa el juego
    }

    public void InitializeGame()
    {
        spawners = FindObjectsOfType<SpawnManager>(); // Encuentra todos los SpawnManagers
        playerCurrentHealth = playerMaxHealth; // Establece la salud del jugador al máximo
        UIManager.Instance.UpdateUI(); // Actualiza la UI con todos los valores actuales
        StartNextLevel(); // Inicia el siguiente nivel
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Start:
                StartNextWave(); // Inicia la siguiente oleada
                break;
            case GameState.WaveActive:
                // Lógica de la oleada activa
                break;
            case GameState.Transition:
                nextRoundCountdown -= Time.deltaTime;
                if (nextRoundCountdown <= 0)
                {
                    StartNextWave(); // Inicia la siguiente oleada
                }
                UIManager.Instance.UpdateNextRoundCountdownUI(nextRoundCountdown); // Actualiza la cuenta atrás en la UI
                break;
            case GameState.GameOver:
                // Lógica del fin del juego
                break;
        }
    }

    // Cambia el estado del juego
    private void ChangeState(GameState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GameState.Start:
                StartNextWave(); // Inicia la siguiente oleada
                break;
            case GameState.WaveActive:
                break;
            case GameState.Transition:
                nextRoundCountdown = 5f;
                UIManager.Instance.UpdateNextRoundCountdownUI(nextRoundCountdown); // Actualiza la cuenta atrás en la UI
                break;
            case GameState.GameOver:
                break;
        }
    }

    // Aplica daño al jugador
    public void PlayerTakeDamage(int damage)
    {
        playerCurrentHealth -= damage;
        UIManager.Instance.UpdatePlayerHealthUI(playerCurrentHealth); // Actualiza la UI de salud del jugador

        if (playerCurrentHealth <= 0)
        {
            Debug.Log("Player has been defeated!");
            ChangeState(GameState.GameOver); // Cambia el estado a GameOver
            SceneManager.LoadScene("Lose"); // Carga la escena de derrota
        }
    }

    // Devuelve la salud actual del jugador
    public int GetPlayerCurrentHealth()
    {
        return playerCurrentHealth;
    }

    // Notifica que un enemigo ha sido derrotado
    public void EnemyDefeated()
    {
        enemiesDefeatedInWave++;
        UIManager.Instance.UpdateEnemyCounterUI(GetRemainingEnemies()); // Actualiza la UI de conteo de enemigos

        if (enemiesDefeatedInWave >= LevelManager.Instance.GetCurrentLevelConfig().waves[LevelManager.Instance.GetCurrentWaveIndex()].numberOfEnemies)
        {
            EndCurrentWave(); // Termina la oleada actual
        }
    }

    // Inicia la siguiente oleada
    public void StartNextWave()
    {
        if (LevelManager.Instance.GetCurrentWaveIndex() < LevelManager.Instance.GetCurrentLevelConfig().waves.Length)
        {
            enemiesDefeatedInWave = 0;
            UIManager.Instance.UpdateWaveText(LevelManager.Instance.GetCurrentWaveIndex() + 1); // Actualiza la UI de oleada
            UIManager.Instance.UpdateEnemyCounterUI(LevelManager.Instance.GetCurrentLevelConfig().waves[LevelManager.Instance.GetCurrentWaveIndex()].numberOfEnemies); // Actualiza la UI de conteo de enemigos

            ChangeState(GameState.WaveActive); // Cambia el estado a WaveActive

            foreach (SpawnManager spawner in spawners)
            {
                spawner.StartWave(LevelManager.Instance.GetCurrentWaveIndex(), LevelManager.Instance.GetCurrentLevelConfig().waves[LevelManager.Instance.GetCurrentWaveIndex()]); // Inicia la oleada en cada spawner
            }
        }
        else
        {
            EndCurrentLevel(); // Termina el nivel actual
        }
    }

    // Termina la oleada actual
    public void EndCurrentWave()
    {
        foreach (SpawnManager spawner in spawners)
        {
            spawner.StopSpawning(); // Detiene el spawning en cada spawner
        }

        DestroyAllEnemies(); // Destruye todos los enemigos

        LevelManager.Instance.SetCurrentWaveIndex(LevelManager.Instance.GetCurrentWaveIndex() + 1); // Incrementa el índice de la oleada actual
        if (LevelManager.Instance.GetCurrentWaveIndex() < LevelManager.Instance.GetCurrentLevelConfig().waves.Length)
        {
            ChangeState(GameState.Transition); // Cambia el estado a Transition
        }
        else
        {
            EndCurrentLevel(); // Termina el nivel actual
        }
    }

    // Destruye todos los enemigos en la escena
    private void DestroyAllEnemies()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy); // Destruye el objeto enemigo
        }
    }

    // Termina el nivel actual
    public void EndCurrentLevel()
    {
        LevelManager.Instance.SetCurrentLevelIndex(LevelManager.Instance.GetCurrentLevelIndex() + 1); // Incrementa el índice del nivel actual
        if (LevelManager.Instance.GetCurrentLevelIndex() < LevelManager.Instance.GetLevelsCount())
        {
            int currentHealth = Player.Instance.GetCurrentHealth(); // Guarda la salud actual del jugador
            SceneManager.LoadScene(LevelManager.Instance.GetCurrentLevelConfig().levelName); // Carga la escena del siguiente nivel
            Player.Instance.SetCurrentHealth(currentHealth); // Restaura la salud del jugador
            SceneManager.sceneLoaded += OnLevelLoaded; // Añade el evento de carga de escena
        }
        else
        {
            ChangeState(GameState.GameOver); // Cambia el estado a GameOver
            SceneManager.LoadScene("Win"); // Carga la escena de victoria
        }
    }

    // Evento de carga de escena
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnLevelLoaded; // Elimina el evento de carga de escena
        if (scene.name != "Win" && scene.name != "Lose")
        {
            UIManager.Instance.ResetUIReferences(); // Restablece las referencias de la UI
            UIManager.Instance.UpdateUI(); // Actualiza la UI con todos los valores actuales
            InitializeGame(); // Inicializa el juego
        }
    }

    // Inicia el siguiente nivel
    private void StartNextLevel()
    {
        LevelManager.Instance.SetCurrentWaveIndex(0); // Reinicia el índice de la oleada
        StartNextWave(); // Inicia la siguiente oleada
    }

    // Reinicia el juego
    public void RestartGame()
    {
        LevelManager.Instance.SetCurrentLevelIndex(0); // Reinicia el índice del nivel
        LevelManager.Instance.SetCurrentWaveIndex(0); // Reinicia el índice de la oleada
        PlayerData.Instance.InitializeHealth(); // Reinicia la salud del jugador
        SceneManager.LoadScene(LevelManager.Instance.GetCurrentLevelConfig().levelName); // Carga la escena del primer nivel
        SceneManager.sceneLoaded += OnLevelLoaded; // Añade el evento de carga de escena
    }

    // Devuelve el índice de la oleada actual
    public int GetCurrentWave()
    {
        return LevelManager.Instance.GetCurrentWaveIndex() + 1;
    }

    // Devuelve el número de enemigos restantes
    public int GetRemainingEnemies()
    {
        if (LevelManager.Instance.GetCurrentWaveIndex() >= LevelManager.Instance.GetCurrentLevelConfig().waves.Length || LevelManager.Instance.GetCurrentWaveIndex() < 0)
        {
            return 0;
        }
        return LevelManager.Instance.GetCurrentLevelConfig().waves[LevelManager.Instance.GetCurrentWaveIndex()].numberOfEnemies - enemiesDefeatedInWave;
    }

    // Devuelve la cuenta atrás para la siguiente ronda
    public float GetNextRoundCountdown()
    {
        return nextRoundCountdown;
    }

    // Carga el siguiente nivel
    public void LoadNextLevel(string sceneName)
    {
        int currentHealth = Player.Instance.GetCurrentHealth(); // Guarda la salud actual del jugador
        SceneManager.LoadScene(sceneName); // Carga la escena especificada
        Player.Instance.SetCurrentHealth(currentHealth); // Restaura la salud del jugador
        SceneManager.sceneLoaded += OnSceneLoaded; // Añade el evento de carga de escena
    }

    // Evento de carga de escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Elimina el evento de carga de escena

        if (scene.name != "Win" && scene.name != "Lose")
        {
            UIManager.Instance.ResetUIReferences(); // Restablece las referencias de la UI
            UIManager.Instance.UpdateUI(); // Actualiza la UI con todos los valores actuales
            InitializeGame(); // Inicializa el juego
        }
    }
}
