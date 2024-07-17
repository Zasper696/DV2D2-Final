using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "ScriptableObjects/WaveConfig", order = 1)]
public class WaveConfigScriptableObject : ScriptableObject
{
    public int numberOfEnemies; // Número de enemigos en esta oleada
    public EnemyPrefab[] enemies; // Lista de tipos de enemigos que pueden aparecer
    public float spawnInterval; // Intervalo entre cada generación
    public float intervalBetweenWaves; // Intervalo entre oleadas
    public bool disableDuringWave; // Si el spawn está deshabilitado en esta oleada
}

[System.Serializable]
public struct EnemyPrefab
{
    public GameObject enemyPrefab; // El prefab del enemigo
    public float spawnProbability; // La probabilidad de que este enemigo se genere
}
