// Enemy.cs
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum DamageType
    {
        Physical,
        Magical,
        Fire,
        Ice,
        Poison
    }

    [SerializeField] protected int health; // Salud del enemigo
    [SerializeField] private float speed; // Velocidad del enemigo
    [SerializeField] private int damageToPlayer; // Daño al jugador
    [SerializeField] private float detectionRadius; // Radio de detección del jugador
    [SerializeField] private float stuckTimeThreshold = 3f; // Tiempo para considerarse atascado
    [SerializeField] private float minimumMovementThreshold = 0.1f; // Umbral mínimo de movimiento

    protected Vector2 movementDirection; // Dirección de movimiento del enemigo
    protected Transform player; // Referencia al jugador
    private Vector2 lastPosition; // Última posición del enemigo
    private float timeStuck = 0; // Tiempo atascado

    private void Awake()
    {
        SetDefaultValues(); // Establece valores por defecto
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Encuentra al jugador
        ChooseNewDirection(); // Elige una nueva dirección de movimiento
    }

    protected virtual void Update()
    {
        Move(); // Mueve al enemigo
        CheckIfStuck(); // Comprueba si está atascado
        FollowPlayer(); // Sigue al jugador
    }

    private void Move()
    {
        transform.Translate(movementDirection * speed * Time.deltaTime); // Traduce la posición del enemigo
    }

    private void FollowPlayer()
    {
        if (player && Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            movementDirection = (player.position - transform.position).normalized; // Normaliza la dirección hacia el jugador
        }
    }

    private void CheckIfStuck()
    {
        if (Vector2.Distance(transform.position, lastPosition) < minimumMovementThreshold)
        {
            timeStuck += Time.deltaTime;
            if (timeStuck >= stuckTimeThreshold)
            {
                ChooseNewDirection(); // Elige una nueva dirección si está atascado
                timeStuck = 0;
            }
        }
        else
        {
            timeStuck = 0;
        }
        lastPosition = transform.position;
    }

    protected void ChooseNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        movementDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)); // Elige una dirección aleatoria
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            ChooseNewDirection(); // Elige una nueva dirección al chocar con una pared
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer, Player.DamageType.Physical); // Añadir DamageType
                Die(); // Matar al enemigo
            }
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            PlayerBullet bullet = collision.gameObject.GetComponent<PlayerBullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage, bullet.damageType); // Añadir DamageType
                Destroy(bullet.gameObject); // Destruir la bala
            }
        }
    }

    public virtual void TakeDamage(int damage, DamageType damageType)
    {
        health -= damage;
        if (health <= 0)
        {
            Die(); // Matar al enemigo
        }
    }

    protected virtual void Die()
    {
        Debug.Log("Enemy died.");
        GameManager.Instance.EnemyDefeated(); // Notificar al GameManager que el enemigo ha sido derrotado
        Destroy(gameObject); // Destruir el objeto enemigo
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Dibuja el radio de detección en el editor
    }

    private void SetDefaultValues()
    {
        health = 3;
        speed = 2f;
        damageToPlayer = 1;
        detectionRadius = 5f;
    }
}
