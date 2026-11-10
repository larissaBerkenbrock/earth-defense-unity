using UnityEngine;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    [Header("UI")]
    public TextMeshProUGUI weaponText;

    [Header("Visual")]
    public SpriteRenderer weaponSpriteRenderer;

    [Header("Armas (dano e cooldown)")]
    public int[] damages = { 1, 2, 4, 10 };
    public float[] cooldowns = { 0.5f, 0.3f, 0.6f, 0.2f };
    public string[] names = { "Pistola Basica", "Rifle de Plasma", "Lanca-Granadas", "Raio Laser" };
    public int[] unlockCost = { 0, 25, 60, 999 };

    public int currentWeapon = 0;

    private int activeDamage;
    private float activeCooldown;
    private string activeWeaponName;
    private bool levelWeaponActive;

    // Cria a instancia global, garante referencias e seleciona a arma inicial.
    void Awake()
    {
        Instance = this;
        EnsureReferences();
        SetActiveWeaponFromIndex(currentWeapon);
    }

    // Aplica a arma definida pelo nivel, incluindo nome, dano, cooldown e sprite.
    public void SetLevelWeapon(string weaponName, int damage, float cooldown, Sprite weaponSprite)
    {
        EnsureReferences();

        levelWeaponActive = true;
        activeWeaponName = string.IsNullOrWhiteSpace(weaponName) ? "Arma" : weaponName;
        activeDamage = Mathf.Max(1, damage);
        activeCooldown = Mathf.Max(0.05f, cooldown);

        if (weaponSpriteRenderer != null && weaponSprite != null)
            weaponSpriteRenderer.sprite = weaponSprite;
        else if (weaponSpriteRenderer == null)
            Debug.LogWarning("WeaponManager sem Weapon Sprite Renderer atribuido.");
        else if (weaponSprite == null)
            Debug.LogWarning("A arma do nivel '" + activeWeaponName + "' nao tem sprite atribuido.");

        UpdateUI();
        Debug.Log("Arma do nivel aplicada: " + activeWeaponName + " | Dano: " + activeDamage + " | Cooldown: " + activeCooldown);
    }

    // Tenta desbloquear uma arma melhor com base na quantidade de cores coletados.
    public void TryUnlock(int cores)
    {
        if (levelWeaponActive) return;

        int weaponCount = Mathf.Min(damages.Length, cooldowns.Length, names.Length, unlockCost.Length);

        for (int i = weaponCount - 1; i > currentWeapon; i--)
        {
            if (cores >= unlockCost[i])
            {
                currentWeapon = i;
                SetActiveWeaponFromIndex(i);
                Debug.Log("Arma desbloqueada: " + names[i]);
                break;
            }
        }
    }

    // Devolve o dano da arma ativa para os tiros.
    public int GetDamage() => activeDamage;
    // Devolve o cooldown da arma ativa para controlar a velocidade de tiro.
    public float GetCooldown() => activeCooldown;

    // Seleciona uma arma pelos arrays configurados no Inspector.
    void SetActiveWeaponFromIndex(int index)
    {
        int weaponCount = Mathf.Min(damages.Length, cooldowns.Length, names.Length);
        if (weaponCount == 0) return;

        currentWeapon = Mathf.Clamp(index, 0, weaponCount - 1);
        activeDamage = damages[currentWeapon];
        activeCooldown = cooldowns[currentWeapon];
        activeWeaponName = names[currentWeapon];
        UpdateUI();
    }

    // Atualiza o texto da UI com o nome da arma ativa.
    void UpdateUI()
    {
        if (weaponText != null)
            weaponText.text = "Arma: " + activeWeaponName;
    }

    // Procura automaticamente o SpriteRenderer da arma caso ele nao tenha sido atribuido.
    void EnsureReferences()
    {
        if (weaponSpriteRenderer == null)
        {
            SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer.name.ToLower().Contains("pistola") || renderer.name.ToLower().Contains("arma") || renderer.name.ToLower().Contains("weapon"))
                {
                    weaponSpriteRenderer = renderer;
                    break;
                }
            }
        }
    }
}
