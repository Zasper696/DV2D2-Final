using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class LevelConfig
{
    public string levelName; // Nombre del nivel
    public WaveConfigScriptableObject[] waves; // Configuraci�n de las oleadas en el nivel
    public float spawnerStartDelay = 3f; // Retraso antes de iniciar los spawners
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; } // Instancia �nica de LevelManager

    [SerializeField] private LevelConfig[] levels; // Configuraci�n de todos los niveles
    private int currentLevelIndex = 0; // �ndice del nivel actual
    private int currentWaveIndex = 0; // �ndice de la oleada actual

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeLevel(); // Inicializa el nivel actual
    }

    // Inicializa el nivel
    public void InitializeLevel()
    {
        currentWaveIndex = 0;
        StartCoroutine(StartSpawnersWithDelay(levels[currentLevelIndex].spawnerStartDelay)); // Inicia los spawners con un retraso
    }

    // Devuelve la configuraci�n del nivel actual
    public LevelConfig GetCurrentLevelConfig()
    {
        return levels[currentLevelIndex];
    }

    // Devuelve el �ndice del nivel actual
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    // Devuelve el �ndice de la oleada actual
    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }

    // Establece el �ndice de la oleada actual
    public void SetCurrentWaveIndex(int waveIndex)
    {
        currentWaveIndex = waveIndex;
    }

    // Establece el �ndice del nivel actual
    public void SetCurrentLevelIndex(int levelIndex)
    {
        currentLevelIndex = levelIndex;
    }

    // Termina el nivel actual
    public void EndCurrentLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levels.Length)
        {
            SceneManager.LoadScene(levels[currentLevelIndex].levelName); // Carga la escena del siguiente nivel
            SceneManager.sceneLoaded += OnLevelLoaded; // A�ade el evento de carga de escena
        }
        else
        {
            SceneManager.LoadScene("Win"); // Carga la escena de victoria
        }
    }

    // Evento de carga de escena
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnLevelLoaded; // Elimina el evento de carga de escena
        InitializeLevel(); // Inicializa el nivel
    }

    // Inicia los spawners con un retraso
    private IEnumerator StartSpawnersWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.StartNextWave(); // Inicia la siguiente oleada
    }

    // Reinicia el nivel
    public void ResetLevel()
    {
        currentLevelIndex = 0;
        currentWaveIndex = 0;
    }

    // Devuelve el n�mero total de niveles
    public int GetLevelsCount()
    {
        return levels.Length;
    }
}
