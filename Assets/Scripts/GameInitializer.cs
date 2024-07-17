// GameInitializer.cs
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject playerDataPrefab;
    public GameObject uiPrefab;
    public GameObject gameManagerPrefab;
    public GameObject damageManagerPrefab;
    public GameObject healthManagerPrefab;
    public GameObject levelManagerPrefab;
    public GameObject spawnManagerPrefab;
    public GameObject waveManagerPrefab;
    public GameObject canvasPrefab;
    public GameObject camaraManagerPrefab;

    private void Awake()
    {
        // Instanciar PlayerData
        if (PlayerData.Instance == null)
        {
            Instantiate(playerDataPrefab);
        }

        // Instanciar el jugador
        if (Player.Instance == null)
        {
            GameObject player = Instantiate(playerPrefab);
            PositionPlayerAtSpawnPoint(player);
        }
        else
        {
            PositionPlayerAtSpawnPoint(Player.Instance.gameObject);
        }

        // Comprobar y cargar el GameManager
        if (GameManager.Instance == null)
        {
            Instantiate(gameManagerPrefab);
        }

        // Comprobar y cargar el UIManager
        if (UIManager.Instance == null)
        {
            Instantiate(uiPrefab);
        }

        // Comprobar y cargar el DamageManager
        if (DamageManager.Instance == null)
        {
            Instantiate(damageManagerPrefab);
        }

        // Comprobar y cargar el HealthManager
        if (HealthManager.Instance == null)
        {
            Instantiate(healthManagerPrefab);
        }

        // Comprobar y cargar el LevelManager
        if (LevelManager.Instance == null)
        {
            Instantiate(levelManagerPrefab);
        }

        // Comprobar y cargar el SpawnManager
        if (SpawnManager.Instance == null)
        {
            Instantiate(spawnManagerPrefab);
        }

        // Comprobar y cargar el WaveManager
        if (WaveManager.Instance == null)
        {
            Instantiate(waveManagerPrefab);
        }

        // Comprobar y cargar el Canvas
        if (GameObject.FindWithTag("Canvas") == null)
        {
            Instantiate(canvasPrefab);
        }

        // Comprobar y cargar el CamaraManager
        if (CamaraManager.Instance == null)
        {
            Instantiate(camaraManagerPrefab);
        }
    }

    // Posiciona al jugador en el punto de spawn
    private void PositionPlayerAtSpawnPoint(GameObject player)
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
        }
        else
        {
            Debug.LogWarning("No PlayerSpawn point found. Player will be instantiated at default position.");
        }
    }
}
