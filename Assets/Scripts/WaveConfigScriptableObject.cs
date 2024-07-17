using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "ScriptableObjects/WaveConfig", order = 1)]
public class WaveConfigScriptableObject : ScriptableObject
{
    public int numberOfEnemies; // N�mero de enemigos en esta oleada
    public EnemyPrefab[] enemies; // Lista de tipos de enemigos que pueden aparecer
    public float spawnInterval; // Intervalo entre cada generaci�n
    public float intervalBetweenWaves; // Intervalo entre oleadas
    public bool disableDuringWave; // Si el spawn est� deshabilitado en esta oleada
}

[System.Serializable]
public struct EnemyPrefab
{
    public GameObject enemyPrefab; // El prefab del enemigo
    public float spawnProbability; // La probabilidad de que este enemigo se genere
}
