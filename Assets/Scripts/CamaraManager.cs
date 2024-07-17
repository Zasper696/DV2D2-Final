using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CamaraConfig
{
    public string sceneName; // Nombre de la escena
    public float limiteXMinimo; // Límite mínimo en X
    public float limiteXMaximo; // Límite máximo en X
    public float limiteYMinimo; // Límite mínimo en Y
    public float limiteYMaximo; // Límite máximo en Y
}

public class CamaraManager : MonoBehaviour
{
    public static CamaraManager Instance { get; private set; } // Instancia única de CamaraManager

    public List<CamaraConfig> camaraConfigs; // Lista de configuraciones de cámara

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

    // Obtiene la configuración de cámara para la escena actual
    public CamaraConfig GetConfigForCurrentScene()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        foreach (CamaraConfig config in camaraConfigs)
        {
            if (config.sceneName == currentSceneName)
            {
                return config;
            }
        }

        Debug.LogWarning("No camera configuration found for the current scene: " + currentSceneName);
        return null;
    }
}

