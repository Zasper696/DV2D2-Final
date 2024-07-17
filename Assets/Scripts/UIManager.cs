// UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Instancia única de UIManager

    [Header("Player Health UI")]
    public Slider playerHealthSlider; // Slider para la salud del jugador
    public Text playerHealthText; // Texto para la salud del jugador

    [Header("Enemy Counter UI")]
    public Text enemyCounterText; // Texto para el contador de enemigos

    [Header("Wave Counter UI")]
    public Text waveText; // Texto para la oleada actual

    [Header("Next Round Countdown UI")]
    public Text nextRoundText; // Texto para la cuenta atrás de la siguiente ronda

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AssignUIElements(); // Asigna los elementos de la UI
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeUI(); // Inicializa la UI
    }

    // Asigna los elementos de la UI usando etiquetas
    private void AssignUIElements()
    {
        playerHealthSlider = GameObject.FindWithTag("UI_HealthSlider")?.GetComponent<Slider>();
        playerHealthText = GameObject.FindWithTag("UI_HealthText")?.GetComponent<Text>();
        enemyCounterText = GameObject.FindWithTag("UI_EnemyCounter")?.GetComponent<Text>();
        waveText = GameObject.FindWithTag("UI_WaveCounter")?.GetComponent<Text>();
        nextRoundText = GameObject.FindWithTag("UI_NextRoundCounter")?.GetComponent<Text>();

        if (playerHealthSlider == null) Debug.LogError("Player Health Slider not found! Please ensure the UI element is tagged with 'UI_HealthSlider'.");
        if (playerHealthText == null) Debug.LogError("Player Health Text not found! Please ensure the UI element is tagged with 'UI_HealthText'.");
        if (enemyCounterText == null) Debug.LogError("Enemy Counter Text not found! Please ensure the UI element is tagged with 'UI_EnemyCounter'.");
        if (waveText == null) Debug.LogError("Wave Text not found! Please ensure the UI element is tagged with 'UI_WaveCounter'.");
        if (nextRoundText == null) Debug.LogError("Next Round Text not found! Please ensure the UI element is tagged with 'UI_NextRoundCounter'.");
    }

    // Inicializa la UI
    private void InitializeUI()
    {
        if (playerHealthSlider == null || playerHealthText == null ||
            enemyCounterText == null || waveText == null || nextRoundText == null)
        {
            Debug.LogError("UI elements not assigned in the inspector.");
            return;
        }

        playerHealthSlider.maxValue = PlayerData.Instance.maxHealth; // Establece el valor máximo del slider
        UpdatePlayerHealthUI(PlayerData.Instance.health); // Actualiza la salud del jugador en la UI
    }

    // Actualiza la salud del jugador en la UI
    public void UpdatePlayerHealthUI(int currentHealth)
    {
        if (playerHealthSlider != null && playerHealthText != null)
        {
            playerHealthSlider.value = currentHealth;
            playerHealthText.text = "Player: " + currentHealth;
        }
    }

    // Actualiza el contador de enemigos en la UI
    public void UpdateEnemyCounterUI(int remainingEnemies)
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = "Enemies Left: " + remainingEnemies;
        }
    }

    // Actualiza el texto de la oleada en la UI
    public void UpdateWaveText(int currentWave)
    {
        if (waveText != null)
        {
            waveText.text = "Actual Wave: " + currentWave;
        }
    }

    // Actualiza la cuenta atrás de la siguiente ronda en la UI
    public void UpdateNextRoundCountdownUI(float countdown)
    {
        if (nextRoundText != null)
        {
            nextRoundText.text = "Next Round In: " + countdown.ToString("F1") + "s";
        }
    }

    // Actualiza todos los elementos de la UI
    public void UpdateUI()
    {
        UpdatePlayerHealthUI(PlayerData.Instance.health);
        UpdateEnemyCounterUI(GameManager.Instance.GetRemainingEnemies());
        UpdateWaveText(LevelManager.Instance.GetCurrentWaveIndex() + 1);
        // Asegurarse de que nextRoundText se actualiza solo cuando está en estado de transición
        if (GameManager.Instance.GetNextRoundCountdown() > 0)
        {
            UpdateNextRoundCountdownUI(GameManager.Instance.GetNextRoundCountdown());
        }
    }

    // Restablece las referencias de la UI
    public void ResetUIReferences()
    {
        AssignUIElements();
        InitializeUI(); // Inicializa la UI
    }
}
