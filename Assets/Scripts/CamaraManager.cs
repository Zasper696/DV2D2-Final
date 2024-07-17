using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CamaraConfig
{
    public string sceneName; // Nombre de la escena
    public float limiteXMinimo; // L�mite m�nimo en X
    public float limiteXMaximo; // L�mite m�ximo en X
    public float limiteYMinimo; // L�mite m�nimo en Y
    public float limiteYMaximo; // L�mite m�ximo en Y
}

public class CamaraManager : MonoBehaviour
{
    public static CamaraManager Instance { get; private set; } // Instancia �nica de CamaraManager

    public List<CamaraConfig> camaraConfigs; // Lista de configuraciones de c�mara

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

    // Obtiene la configuraci�n de c�mara para la escena actual
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

