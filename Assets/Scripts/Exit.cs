using UnityEngine;

public class Exit : MonoBehaviour
{
    // Sale del juego
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit(); // Cierra la aplicación

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Para salir del modo Play en el editor de Unity
#endif
    }
}
