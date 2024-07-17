// DamageManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set; } // Instancia �nica de DamageManager

    [System.Serializable]
    public class DamageConfig
    {
        public GameObject enemyPrefab; // Prefab del enemigo
        public Enemy.DamageType damageType; // Tipo de da�o del enemigo
    }

    [System.Serializable]
    public class PlayerDamageConfig : DamageConfig
    {
        public int damageAmount; // Cantidad de da�o que el jugador puede causar
    }

    [System.Serializable]
    public class EnemyDamageConfig : DamageConfig
    {
        public GameObject bulletPrefab; // Prefab de la bala del enemigo
        public int damageAmount; // Cantidad de da�o que el enemigo puede causar
    }

    [SerializeField] private List<PlayerDamageConfig> playerDamageConfigs; // Configuraciones de da�o del jugador
    [SerializeField] private List<EnemyDamageConfig> enemyDamageConfigs; // Configuraciones de da�o del enemigo

    private List<DamageInfo> playerDamageQueue = new List<DamageInfo>(); // Cola de da�os al jugador
    private List<DamageInfo> enemyDamageQueue = new List<DamageInfo>(); // Cola de da�os al enemigo

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
        StartCoroutine(ProcessDamageQueue()); // Inicia la rutina para procesar la cola de da�os
    }

    private IEnumerator ProcessDamageQueue()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            ApplyDamage(); // Aplica los da�os en cola
        }
    }

    // Registra el da�o al jugador
    public void RegisterPlayerDamage(int damage, Player.DamageType damageType)
    {
        playerDamageQueue.Add(new DamageInfo(damage, damageType));
    }

    // Registra el da�o al enemigo
    public void RegisterEnemyDamage(GameObject enemy, int damage, Enemy.DamageType damageType)
    {
        enemyDamageQueue.Add(new DamageInfo(enemy, damage, damageType));
    }

    // Aplica los da�os registrados
    private void ApplyDamage()
    {
        foreach (var damage in playerDamageQueue)
        {
            FindObjectOfType<Player>().TakeDamage(damage.Damage, damage.PlayerDamageType);
        }
        playerDamageQueue.Clear();

        foreach (var damage in enemyDamageQueue)
        {
            if (damage.Target != null)
            {
                Enemy enemy = damage.Target.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage.Damage, damage.EnemyDamageType);
                }
            }
        }
        enemyDamageQueue.Clear();
    }

    private class DamageInfo
    {
        public GameObject Target { get; }
        public int Damage { get; }
        public Player.DamageType PlayerDamageType { get; }
        public Enemy.DamageType EnemyDamageType { get; }

        public DamageInfo(int damage, Player.DamageType damageType)
        {
            Damage = damage;
            PlayerDamageType = damageType;
        }

        public DamageInfo(GameObject target, int damage, Enemy.DamageType damageType)
        {
            Target = target;
            Damage = damage;
            EnemyDamageType = damageType;
        }
    }
}
