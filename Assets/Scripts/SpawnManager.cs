// SpawnManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; } // Instancia única de SpawnManager

    private WaveConfigScriptableObject currentWaveConfig; // Configuración de la oleada actual
    private bool isSpawning = false; // Indica si los enemigos están siendo generados
    private int currentWaveIndex = 0; // Índice de la oleada actual

    private Transform[] currentLevelSpawners; // Array de spawners en el nivel actual

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

        SceneManager.sceneLoaded += OnSceneLoaded; // Añade el evento de carga de escena
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Elimina el evento de carga de escena
    }

    // Evento de carga de escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentLevelSpawners = GetSpawnersForCurrentLevel(); // Obtiene los spawners para el nivel actual
    }

    // Devuelve el índice de la oleada actual
    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }

    // Inicia la generación de enemigos para la oleada especificada
    public void StartWave(int waveIndex, WaveConfigScriptableObject waveConfig)
    {
        currentWaveIndex = waveIndex;
        currentWaveConfig = waveConfig;

        if (currentWaveConfig == null)
        {
            Debug.LogError("WaveConfigScriptableObject is null.");
            return;
        }

        if (currentLevelSpawners == null || currentLevelSpawners.Length == 0)
        {
            Debug.LogError("No spawners found for the current level.");
            return;
        }

        isSpawning = !currentWaveConfig.disableDuringWave;

        if (isSpawning)
        {
            foreach (var spawner in currentLevelSpawners)
            {
                if (spawner != null)
                {
                    StartCoroutine(SpawnEnemiesFromSpawner(spawner)); // Inicia la corrutina para generar enemigos desde el spawner
                }
                else
                {
                    Debug.LogError("Spawner is null. Make sure all spawners are assigned.");
                }
            }
        }
    }

    // Devuelve los spawners para el nivel actual
    private Transform[] GetSpawnersForCurrentLevel()
    {
        GameObject[] spawnerObjects = GameObject.FindGameObjectsWithTag("Spawner");
        Transform[] spawners = new Transform[spawnerObjects.Length];
        for (int i = 0; i < spawnerObjects.Length; i++)
        {
            spawners[i] = spawnerObjects[i].transform;
        }
        return spawners;
    }

    // Detiene la generación de enemigos
    public void StopSpawning()
    {
        StopAllCoroutines();
        isSpawning = false;
    }

    // Corrutina para generar enemigos desde el spawner especificado
    private IEnumerator SpawnEnemiesFromSpawner(Transform spawner)
    {
        while (isSpawning)
        {
            float totalProbability = 0f;
            foreach (var enemy in currentWaveConfig.enemies)
            {
                totalProbability += enemy.spawnProbability;
            }

            float randomValue = Random.Range(0f, totalProbability);
            float cumulativeProbability = 0f;

            foreach (var enemy in currentWaveConfig.enemies)
            {
                cumulativeProbability += enemy.spawnProbability;
                if (randomValue <= cumulativeProbability)
                {
                    Instantiate(enemy.enemyPrefab, spawner.position, Quaternion.identity); // Instancia el prefab del enemigo
                    break;
                }
            }

            yield return new WaitForSeconds(currentWaveConfig.spawnInterval); // Espera antes de generar el siguiente enemigo
        }
    }
}
