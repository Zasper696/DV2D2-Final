using UnityEngine;

public class Camara : MonoBehaviour
{
    [SerializeField] public Transform objetivo; // El objetivo a seguir (el jugador)
    [SerializeField] public float smoothSpeed = 0.125f; // Velocidad de suavizado
    [SerializeField] private Vector3 velocity = Vector3.zero; // Velocidad actual de la c�mara

    private float limiteXMinimo;
    private float limiteXMaximo;
    private float limiteYMinimo;
    private float limiteYMaximo;

    void Start()
    {
        objetivo = GameObject.FindGameObjectWithTag("Player").transform; // Encuentra el jugador
        InitializeCameraLimits(); // Inicializa los l�mites de la c�mara
    }

    void FixedUpdate()
    {
        if (objetivo != null)
        {
            Vector3 posicionObjetivo = objetivo.position;
            posicionObjetivo.x = Mathf.Clamp(posicionObjetivo.x, limiteXMinimo, limiteXMaximo);
            posicionObjetivo.y = Mathf.Clamp(posicionObjetivo.y, limiteYMinimo, limiteYMaximo);
            posicionObjetivo.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, posicionObjetivo, ref velocity, smoothSpeed); // Mueve la c�mara suavemente
        }
    }

    // Inicializa los l�mites de la c�mara seg�n la escena actual
    private void InitializeCameraLimits()
    {
        CamaraConfig config = CamaraManager.Instance.GetConfigForCurrentScene();
        if (config != null)
        {
            limiteXMinimo = config.limiteXMinimo;
            limiteXMaximo = config.limiteXMaximo;
            limiteYMinimo = config.limiteYMinimo;
            limiteYMaximo = config.limiteYMaximo;
        }
        else
        {
            Debug.LogWarning("Camera limits not set for the current scene.");
        }
    }
}
