using UnityEngine;

public class ScoutMovement : MonoBehaviour
{
    public float speed = 3f;
    public int damage = 20;
    public float contactDamageCooldown = 1f;
    public Transform player;
    public bool spriteFacesRight = false;

    Rigidbody2D rb;
    Vector3 originalScale;
    float nextContactDamageTime;

    // Inicializa a fisica, guarda a escala original e encontra o jogador se necessario.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        if (player == null)
        {
            PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
            if (playerHealth != null)
                player = playerHealth.transform;
        }
    }

    // Move o Scout na direcao do jogador e ajusta o lado visual do sprite.
    void FixedUpdate()
    {
        if (player == null) return;

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        float facingDirection = spriteFacesRight ? direction : -direction;

        transform.localScale = new Vector3(
            Mathf.Abs(originalScale.x) * facingDirection,
            originalScale.y,
            originalScale.z
        );
    }

    // Tenta causar dano assim que algo entra no trigger do Scout.
    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    // Continua tentando causar dano enquanto o jogador permanece no trigger.
    void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    // Verifica se o alvo e o jogador e aplica dano com cooldown de contato.
    void TryDamagePlayer(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null || playerHealth.IsDead) return;
        if (Time.time < nextContactDamageTime) return;

        playerHealth.TakeDamage(damage);
        nextContactDamageTime = Time.time + contactDamageCooldown;
    }
}
