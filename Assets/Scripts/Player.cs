using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } // Instancia �nica de Player

    public enum DamageType
    {
        Physical,
        Magical,
        Fire,
        Ice,
        Poison
    }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5; // Salud m�xima del jugador
    private int currentHealth; // Salud actual del jugador

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento del jugador
    private float lastFireTime; // �ltima vez que el jugador dispar�

    [Header("Attack Settings")]
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala del jugador
    [SerializeField] private float bulletSpawnRadius = 1f; // Radio de spawn de la bala
    [SerializeField] private float fireCooldown = 0.5f; // Tiempo de enfriamiento entre disparos

    [Header("Animation Settings")]
    [SerializeField] private float deathAnimationTime = 1f; // Tiempo de la animaci�n de muerte
    [SerializeField] private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer del jugador

    [Header("Damage Colors")]
    [SerializeField] private Color lowHealthColor = Color.red; // Color cuando la salud es baja
    [SerializeField] private int lowHealthThreshold = 2; // Umbral para considerar la salud baja
    [SerializeField] private Color physicalDamageColor = Color.gray; // Color para da�o f�sico
    [SerializeField] private Color magicalDamageColor = Color.blue; // Color para da�o m�gico
    [SerializeField] private Color fireDamageColor = Color.red; // Color para da�o de fuego
    [SerializeField] private Color iceDamageColor = Color.cyan; // Color para da�o de hielo
    [SerializeField] private Color poisonDamageColor = Color.green; // Color para da�o de veneno
    [SerializeField] private float damageColorDuration = 1f; // Duraci�n del cambio de color por da�o

    [Header("Fire Damage Settings")]
    [SerializeField] private float fireDamageCooldownIncrease = 2f; // Incremento en el tiempo de enfriamiento por da�o de fuego
    [SerializeField] private float fireDamageDuration = 5f; // Duraci�n del da�o de fuego

    [Header("Ice Damage Settings")]
    [SerializeField] private float iceDamageSpeedReduction = 2f; // Reducci�n de velocidad por da�o de hielo
    [SerializeField] private float iceDamageDuration = 5f; // Duraci�n del da�o de hielo

    [Header("Poison Damage Settings")]
    [SerializeField] private int poisonDamagePerSecond = 1; // Da�o por segundo del veneno
    [SerializeField] private float poisonDuration = 5f; // Duraci�n del da�o de veneno

    private Coroutine poisonCoroutine; // Corrutina para aplicar da�o de veneno
    private Animator animator; // Referencia al Animator del jugador

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
        // Usar PlayerData para inicializar la salud del jugador
        currentHealth = PlayerData.Instance.health <= 0 ? maxHealth : PlayerData.Instance.health;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        animator = GetComponent<Animator>();
        UIManager.Instance.UpdatePlayerHealthUI(currentHealth); // Actualiza la UI de salud del jugador
    }

    private void Update()
    {
        Move(); // Maneja el movimiento del jugador
        Attack(); // Maneja el ataque del jugador
    }

    private void Move()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
            spriteRenderer.flipX = true; // Voltea el sprite en el eje X
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
            spriteRenderer.flipX = false; // No voltea el sprite en el eje X
        }

        Vector2 moveDir = new Vector2(moveX, moveY).normalized; // Normaliza la direcci�n de movimiento
        transform.Translate(moveDir * moveSpeed * Time.deltaTime); // Traduce la posici�n del jugador

        if (moveDir.magnitude > 0.1f)
        {
            if (moveY > 0)
                animator.SetInteger("Player", 1);
            else if (moveY < 0)
                animator.SetInteger("Player", 2);
            else if (moveX != 0)
                animator.SetInteger("Player", 3);
        }
        else
        {
            animator.SetInteger("Player", 0);
        }
    }

    private void Attack()
    {
        if (Time.time <= lastFireTime + fireCooldown)
            return;

        Vector2 direction = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector2.up;
            animator.SetInteger("Player", 4);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector2.down;
            animator.SetInteger("Player", 5);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector2.left;
            animator.SetInteger("Player", 6);
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2.right;
            animator.SetInteger("Player", 6);
            spriteRenderer.flipX = false;
        }

        if (direction != Vector2.zero)
        {
            Shoot(direction); // Dispara en la direcci�n especificada
        }
    }

    private void Shoot(Vector2 direction)
    {
        lastFireTime = Time.time; // Actualiza el tiempo del �ltimo disparo
        Vector3 bulletSpawnPosition = transform.position + (Vector3)(direction * bulletSpawnRadius); // Calcula la posici�n de spawn de la bala
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity); // Instancia la bala
        bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * 10f; // Establece la velocidad de la bala
    }

    public void TakeDamage(int damage, DamageType damageType)
    {
        PlayerData.Instance.TakeDamage(damage); // Aplica da�o al jugador
        currentHealth = PlayerData.Instance.health;
        UIManager.Instance.UpdatePlayerHealthUI(currentHealth); // Actualiza la UI de salud del jugador

        if (currentHealth <= 0)
        {
            Die(); // Llama al m�todo Die si la salud es menor o igual a 0
        }
        else
        {
            StartCoroutine(ShowDamageEffect(damageType)); // Muestra el efecto de da�o
            if (damageType == DamageType.Poison)
            {
                if (poisonCoroutine != null)
                {
                    StopCoroutine(poisonCoroutine);
                }
                poisonCoroutine = StartCoroutine(ApplyPoisonDamage()); // Aplica da�o de veneno
            }
            else if (damageType == DamageType.Fire)
            {
                StartCoroutine(ApplyFireDamage()); // Aplica da�o de fuego
            }
            else if (damageType == DamageType.Ice)
            {
                StartCoroutine(ApplyIceDamage()); // Aplica da�o de hielo
            }
        }

        if (currentHealth <= lowHealthThreshold)
        {
            spriteRenderer.color = lowHealthColor; // Cambia el color si la salud es baja
        }
    }

    private IEnumerator ShowDamageEffect(DamageType damageType)
    {
        Color damageColor = GetDamageColor(damageType); // Obtiene el color correspondiente al tipo de da�o
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageColorDuration);
        spriteRenderer.color = Color.white;
    }

    private Color GetDamageColor(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Physical:
                return physicalDamageColor;
            case DamageType.Magical:
                return magicalDamageColor;
            case DamageType.Fire:
                return fireDamageColor;
            case DamageType.Ice:
                return iceDamageColor;
            case DamageType.Poison:
                return poisonDamageColor;
            default:
                return Color.white;
        }
    }

    private IEnumerator ApplyPoisonDamage()
    {
        float elapsedTime = 0f;
        while (elapsedTime < poisonDuration)
        {
            PlayerData.Instance.TakeDamage(poisonDamagePerSecond); // Aplica da�o por veneno
            currentHealth = PlayerData.Instance.health;
            UIManager.Instance.UpdatePlayerHealthUI(currentHealth); // Actualiza la UI de salud del jugador

            if (currentHealth <= 0)
            {
                Die(); // Llama al m�todo Die si la salud es menor o igual a 0
                yield break;
            }
            elapsedTime += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator ApplyFireDamage()
    {
        float elapsedTime = 0f;
        float originalCooldown = fireCooldown;
        fireCooldown += fireDamageCooldownIncrease; // Incrementa el tiempo de enfriamiento por da�o de fuego

        while (elapsedTime < fireDamageDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fireCooldown = originalCooldown; // Restaura el tiempo de enfriamiento original
    }

    private IEnumerator ApplyIceDamage()
    {
        float elapsedTime = 0f;
        float originalMoveSpeed = moveSpeed;
        moveSpeed -= iceDamageSpeedReduction; // Reduce la velocidad de movimiento por da�o de hielo

        while (elapsedTime < iceDamageDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        moveSpeed = originalMoveSpeed; // Restaura la velocidad de movimiento original
    }

    private void Die()
    {
        animator.SetInteger("Player", 7); // Player_Death
        StartCoroutine(DieAnimationCoroutine()); // Inicia la corrutina de animaci�n de muerte
    }

    private IEnumerator DieAnimationCoroutine()
    {
        yield return new WaitForSeconds(deathAnimationTime);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lose"); // Carga la escena de derrota
    }

    // M�todos Get y Set para la salud actual del jugador
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(int health)
    {
        currentHealth = health;
        PlayerData.Instance.health = health;
        UIManager.Instance.UpdatePlayerHealthUI(currentHealth); // Actualiza la UI de salud del jugador
    }
}
