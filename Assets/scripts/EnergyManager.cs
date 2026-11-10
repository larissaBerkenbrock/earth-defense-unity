using UnityEngine;
using TMPro;
using System.Collections;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance;
    public TextMeshProUGUI coreText;
    public int cores = 0;

    // Guarda a instancia global e atualiza o texto inicial de nucleos.
    void Awake() { Instance = this; UpdateUI(); }

    // Adiciona um nucleo recolhido, atualiza sistemas ligados e aplica beneficios ao jogador.
    public void AddCore()
    {
        cores++;
        UpdateUI();
        WeaponManager.Instance?.TryUnlock(cores);
        SpawnManager.Instance?.OnEnergyCoreCollected();

        // 1. Cura +5 HP
        PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
        if (ph != null && ph.currentHealth < ph.maxHealth)
        {
            ph.currentHealth = Mathf.Min(ph.currentHealth + 5, ph.maxHealth);
            Debug.Log("Player curado! HP: " + ph.currentHealth);
        }

        // 2. A cada 10 núcleos → buff de velocidade
        if (cores % 10 == 0)
        {
            PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
            if (pm != null) StartCoroutine(SpeedBoost(pm));
        }
    }

    // Aumenta temporariamente a velocidade do jogador e depois restaura o valor original.
    IEnumerator SpeedBoost(PlayerMovement pm)
    {
        float originalSpeed = pm.velocidade;
        pm.velocidade *= 1.3f;
        Debug.Log("⚡ Adrenalina ativada!");
        yield return new WaitForSeconds(8f);
        pm.velocidade = originalSpeed;
        Debug.Log("Adrenalina terminou");
    }

    // Atualiza o texto da UI com a quantidade atual de nucleos.
    void UpdateUI()
    {
        if (coreText != null) coreText.text = "Núcleos: " + cores;
    }
}
