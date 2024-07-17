using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; } // Instancia �nica de WaveManager

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

    // Inicia una nueva oleada llamando al GameManager
    public void StartWave()
    {
        GameManager.Instance.StartNextWave(); // Llama al m�todo StartNextWave del GameManager
    }

    // Termina la oleada actual llamando al GameManager
    public void EndWave()
    {
        GameManager.Instance.EndCurrentWave(); // Llama al m�todo EndCurrentWave del GameManager
    }
}
