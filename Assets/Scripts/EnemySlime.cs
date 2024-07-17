// EnemySlime.cs
using UnityEngine;

public class EnemySlime : Enemy
{
    [SerializeField] private Color lowHealthColor = Color.red; // Color al tener poca salud
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Obtener el componente SpriteRenderer
    }

    protected override void Update()
    {
        base.Update();
    }

    // Aplica daño al enemigo
    public override void TakeDamage(int damage, DamageType damageType)
    {
        base.TakeDamage(damage, damageType);
        if (health == 1)
        {
            ChangeColor(lowHealthColor); // Cambia el color si la salud es baja
        }
    }

    // Cambia el color del enemigo
    private void ChangeColor(Color newColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = newColor;
        }
    }

    protected override void Die()
    {
        base.Die();
    }
}
