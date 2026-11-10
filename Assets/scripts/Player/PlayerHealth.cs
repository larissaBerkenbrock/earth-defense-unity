using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar;

    [Header("Game Over")]
    public GameOverManager gameOverManager; // arrasta o GameOverManager aqui

    [Header("Animacao de morte")]
    public string deathStateName = "playerDeath";
    public float deathRotationAngle = 90f;
    public float deathRotationDuration = 0.15f;
    public float gameOverDelayAfterDeath = 0.35f;

    private PlayerMovement playerMovement;
    private Rigidbody2D playerRB;
    private Animator anim;
    private Collider2D[] playerColliders;
    private bool isDead;

    // Permite que outros scripts saibam se o jogador ja morreu.
    public bool IsDead => isDead;

    // Inicializa referencias do jogador, define a vida cheia e atualiza a barra de vida.
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
        playerColliders = GetComponents<Collider2D>();

        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Recebe dano, atualiza a vida e chama a morte quando a vida chega a zero.
    public void TakeDamage(int damage)
    {
        if (playerMovement != null && playerMovement.IsDashing) return; // invulneravel no dash
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log("Player levou " + damage + " de dano! Vida atual: " + currentHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    // Sincroniza a barra de vida da UI com a vida atual do jogador.
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // Marca o jogador como morto, para movimento/colisoes e inicia a sequencia de morte.
    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player morreu!");

        if (playerMovement != null)
            playerMovement.SetDead(true);

        if (playerRB != null)
        {
            playerRB.linearVelocity = Vector2.zero;
            playerRB.angularVelocity = 0f;
            playerRB.simulated = false;
        }

        foreach (Collider2D playerCollider in playerColliders)
        {
            if (playerCollider != null)
                playerCollider.enabled = false;
        }

        StartCoroutine(DeathSequence());
    }

    // Executa a animacao/rotacao de morte e depois mostra o game over.
    IEnumerator DeathSequence()
    {
        float animationLength = 0f;
        Quaternion startRotation = transform.rotation;
        float fallDirection = transform.localScale.x >= 0f ? -1f : 1f;
        Quaternion deathRotation = Quaternion.Euler(0f, 0f, startRotation.eulerAngles.z + deathRotationAngle * fallDirection);

        if (deathRotationDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < deathRotationDuration)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / deathRotationDuration);
                transform.rotation = Quaternion.Lerp(startRotation, deathRotation, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        transform.rotation = deathRotation;

        if (anim != null)
        {
            anim.enabled = true;
            anim.SetBool("isWalking", false);

            int deathStateHash = Animator.StringToHash(deathStateName);
            if (anim.HasState(0, deathStateHash))
            {
                anim.Play(deathStateHash, 0, 0f);
                animationLength = GetAnimationLength(deathStateName);
            }
            else
            {
                Debug.LogWarning("Estado de morte nao encontrado no Animator: " + deathStateName);
            }
        }

        yield return new WaitForSeconds(animationLength + gameOverDelayAfterDeath);

        if (gameOverManager != null)
            gameOverManager.ShowGameOver();
    }

    // Procura no Animator a duracao de uma animacao pelo nome do clip.
    float GetAnimationLength(string clipName)
    {
        if (anim == null || anim.runtimeAnimatorController == null)
            return 0f;

        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        return 0f;
    }
}
