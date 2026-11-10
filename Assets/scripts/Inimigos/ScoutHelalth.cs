using UnityEngine;
using System.Collections;

public class ScoutHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 2;
    public int currentHealth;

    [Header("Flash de dano")]
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;

    [Header("Drop")]
    public GameObject corePrefab;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private bool isDead;

    // Define a vida inicial e guarda as cores originais dos sprites.
    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;
    }

    // Reduz a vida do Scout, faz flash de dano ou mata quando chega a zero.
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Scout levou " + damage + " de dano! Vida atual: " + currentHealth);

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(FlashRed());
    }

    // Muda os sprites para vermelho por um curto tempo e depois restaura as cores.
    IEnumerator FlashRed()
    {
        foreach (SpriteRenderer sr in spriteRenderers)
            sr.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        if (isDead) yield break;

        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].color = originalColors[i];
    }

    // Registra a morte do Scout e remove ele da onda atual.
    void Die()
    {
        Debug.Log("Scout morreu!");
        RemoveFromWave(true);
    }

    // Remove o Scout do jogo, opcionalmente cria um core e avisa o SpawnManager.
    public void RemoveFromWave(bool dropCore)
    {
        if (isDead) return;

        isDead = true;

        if (dropCore && corePrefab != null)
            Instantiate(corePrefab, transform.position, Quaternion.identity);

        SpawnManager.Instance?.OnScoutKilled();
        Destroy(gameObject);
    }
}
