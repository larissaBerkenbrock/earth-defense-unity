using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelSettings
    {
        public string levelName = "Nivel";
        [HideInInspector] public int enemiesToDefeat = 10;
        public int scoutsToDefeatToAdvance = 10;
        public int coresToCollectToAdvance = 5;
        public float spawnInterval = 3f;

        [Header("Visual")]
        public Sprite backgroundSprite;
        public Sprite scoutSprite;
        public Sprite weaponSprite;
        public float scoutScaleMultiplier = 1f;
        public float scoutHitboxScaleMultiplier = 1f;
        public bool scoutSpriteFacesRight = false;

        [Header("Arma")]
        public string weaponName = "Pistola Basica";
        public int weaponDamage = 1;
        public float weaponCooldown = 0.5f;

        [Header("Scout")]
        public int scoutHealth = 2;
        public float scoutSpeed = 3f;
        public int scoutDamage = 20;
    }

    public GameObject scoutPrefab;
    public Transform spawnPointLeft;
    public Transform spawnPointRight;

    [Header("Visual da Cena")]
    public SpriteRenderer backgroundRenderer;

    [Header("Sistema de Niveis")]
    public LevelSettings[] levels =
    {
        new LevelSettings
        {
            levelName = "Nivel 1",
            scoutsToDefeatToAdvance = 10,
            coresToCollectToAdvance = 5,
            spawnInterval = 3f,
            weaponName = "Pistola Basica",
            weaponDamage = 1,
            weaponCooldown = 0.5f,
            scoutScaleMultiplier = 1f,
            scoutHitboxScaleMultiplier = 1f,
            scoutSpriteFacesRight = false,
            scoutHealth = 2,
            scoutSpeed = 3f,
            scoutDamage = 20
        },
        new LevelSettings
        {
            levelName = "Nivel 2",
            scoutsToDefeatToAdvance = 10,
            coresToCollectToAdvance = 5,
            spawnInterval = 2.2f,
            weaponName = "Rifle de Plasma",
            weaponDamage = 2,
            weaponCooldown = 0.3f,
            scoutScaleMultiplier = 4f,
            scoutHitboxScaleMultiplier = 1f,
            scoutSpriteFacesRight = true,
            scoutHealth = 3,
            scoutSpeed = 4f,
            scoutDamage = 25
        },
        new LevelSettings
        {
            levelName = "Nivel 3",
            scoutsToDefeatToAdvance = 10,
            coresToCollectToAdvance = 5,
            spawnInterval = 1.6f,
            weaponName = "Raio Laser",
            weaponDamage = 4,
            weaponCooldown = 0.2f,
            scoutScaleMultiplier = 4f,
            scoutHitboxScaleMultiplier = 1f,
            scoutSpriteFacesRight = true,
            scoutHealth = 4,
            scoutSpeed = 5f,
            scoutDamage = 30
        }
    };

    [Header("UI")]
    public TextMeshProUGUI levelText;

    private int currentLevelIndex;
    private int scoutsSpawnedInLevel;
    private int scoutsDefeatedInLevel;
    private int coresCollectedInLevel;
    private float spawnTimer;
    private bool levelActive;
    private bool gameComplete;

    public static SpawnManager Instance;

    // Informa se ainda existe outro nivel depois do nivel atual.
    public bool HasNextLevel => currentLevelIndex < levels.Length - 1;

    // Guarda a instancia global do SpawnManager para outros scripts acessarem.
    void Awake()
    {
        Instance = this;
    }

    // Inicia o primeiro nivel quando a cena comeca.
    void Start()
    {
        StartLevel(0);
    }

    // Controla o tempo de spawn e cria Scouts enquanto o nivel esta ativo.
    void Update()
    {
        if (gameComplete || !levelActive) return;

        LevelSettings level = CurrentLevel();
        spawnTimer += Time.deltaTime;

        int scoutSpawnLimit = Mathf.Max(ScoutsGoal(level), CoresGoal(level));
        if (scoutsSpawnedInLevel < scoutSpawnLimit && spawnTimer >= level.spawnInterval)
        {
            SpawnScout(level);
            scoutsSpawnedInLevel++;
            spawnTimer = 0f;
        }
    }

    // Tenta iniciar o proximo nivel e informa se conseguiu avancar.
    public bool StartNextLevel()
    {
        if (!HasNextLevel)
            return false;

        StartLevel(currentLevelIndex + 1);
        return true;
    }

    // Reseta os contadores, aplica visual/dificuldade e prepara um nivel.
    void StartLevel(int levelIndex)
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("SpawnManager nao tem niveis configurados.");
            return;
        }

        currentLevelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
        scoutsSpawnedInLevel = 0;
        scoutsDefeatedInLevel = 0;
        coresCollectedInLevel = 0;
        spawnTimer = CurrentLevel().spawnInterval;
        levelActive = true;
        gameComplete = false;

        ApplyLevelPresentation(CurrentLevel());
        ClearRemainingScouts();
        ClearRemainingEnergyCores();
        UpdateUI();
        Debug.Log(CurrentLevel().levelName + " iniciado!");
    }

    // Cria um Scout em um dos pontos de spawn e aplica as configuracoes do nivel.
    void SpawnScout(LevelSettings level)
    {
        if (scoutPrefab == null || spawnPointLeft == null || spawnPointRight == null)
        {
            Debug.LogError("SpawnManager precisa do scoutPrefab e dos dois spawn points.");
            return;
        }

        Transform sp = Random.value < 0.5f ? spawnPointLeft : spawnPointRight;
        GameObject scout = Instantiate(scoutPrefab, sp.position, Quaternion.identity);
        ApplyLevelDifficulty(scout, level);
    }

    // Aplica velocidade, dano, vida, sprite, escala e hitbox do nivel ao Scout.
    void ApplyLevelDifficulty(GameObject scout, LevelSettings level)
    {
        ScoutMovement movement = scout.GetComponent<ScoutMovement>();
        if (movement != null)
        {
            movement.speed = level.scoutSpeed;
            movement.damage = level.scoutDamage;
            movement.spriteFacesRight = level.scoutSpriteFacesRight;
        }

        ScoutHealth health = scout.GetComponent<ScoutHealth>();
        if (health != null)
        {
            health.maxHealth = level.scoutHealth;
            health.currentHealth = level.scoutHealth;
        }

        SpriteRenderer spriteRenderer = scout.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && level.scoutSprite != null)
            spriteRenderer.sprite = level.scoutSprite;

        float visualScale = Mathf.Max(0.1f, level.scoutScaleMultiplier);
        Transform visualRoot = FindVisualRoot(scout);
        if (visualRoot != null)
            visualRoot.localScale = Vector3.one * visualScale;
        else
            scout.transform.localScale *= visualScale;

        ResizeHitbox(scout, level.scoutHitboxScaleMultiplier);
    }

    // Encontra o objeto visual do Scout para ajustar escala sem mexer na raiz.
    Transform FindVisualRoot(GameObject scout)
    {
        Transform visualRoot = scout.transform.Find("VisualRoot");
        if (visualRoot != null)
            return visualRoot;

        SpriteRenderer spriteRenderer = scout.GetComponentInChildren<SpriteRenderer>();
        return spriteRenderer != null ? spriteRenderer.transform : null;
    }

    // Redimensiona as hitboxes do Scout conforme o multiplicador do nivel.
    void ResizeHitbox(GameObject scout, float hitboxScale)
    {
        float safeHitboxScale = Mathf.Max(0.1f, hitboxScale);

        BoxCollider2D[] boxColliders = scout.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D boxCollider in boxColliders)
        {
            boxCollider.size *= safeHitboxScale;
            boxCollider.offset *= safeHitboxScale;
        }
    }

    // Conta um Scout derrotado e verifica se o objetivo do nivel foi concluido.
    public void OnScoutKilled()
    {
        if (gameComplete || !levelActive) return;

        LevelSettings level = CurrentLevel();
        scoutsDefeatedInLevel = Mathf.Min(scoutsDefeatedInLevel + 1, ScoutsGoal(level));
        UpdateUI();
        CheckLevelObjectives();
    }

    // Conta um core coletado e verifica se o objetivo do nivel foi concluido.
    public void OnEnergyCoreCollected()
    {
        if (gameComplete || !levelActive) return;

        LevelSettings level = CurrentLevel();
        coresCollectedInLevel = Mathf.Min(coresCollectedInLevel + 1, CoresGoal(level));
        UpdateUI();
        CheckLevelObjectives();
    }

    // Verifica se a quantidade necessaria de Scouts ou cores ja foi atingida.
    void CheckLevelObjectives()
    {
        LevelSettings level = CurrentLevel();
        if (scoutsDefeatedInLevel >= ScoutsGoal(level) || coresCollectedInLevel >= CoresGoal(level))
            CompleteCurrentLevel();
    }

    // Finaliza o nivel atual, atualiza a UI e chama a tela de vitoria/progresso.
    void CompleteCurrentLevel()
    {
        if (!levelActive) return;

        levelActive = false;
        gameComplete = !HasNextLevel;
        UpdateUI();

        int completedLevel = currentLevelIndex + 1;
        Debug.Log("Nivel " + completedLevel + " concluido!");

        if (VictoryManager.Instance != null)
            VictoryManager.Instance.ShowLevelComplete(completedLevel, levels.Length, HasNextLevel);
        else
            Debug.LogError("VictoryManager nao encontrado na cena.");
    }

    // Retorna as configuracoes do nivel atual com indice protegido.
    LevelSettings CurrentLevel()
    {
        return levels[Mathf.Clamp(currentLevelIndex, 0, levels.Length - 1)];
    }

    // Retorna a meta minima de Scouts que precisam ser derrotados.
    int ScoutsGoal(LevelSettings level)
    {
        return Mathf.Max(1, level.scoutsToDefeatToAdvance);
    }

    // Retorna a meta minima de cores que precisam ser coletados.
    int CoresGoal(LevelSettings level)
    {
        return Mathf.Max(1, level.coresToCollectToAdvance);
    }

    // Aplica o fundo e a arma visual/funcional definidos para o nivel.
    void ApplyLevelPresentation(LevelSettings level)
    {
        if (backgroundRenderer != null && level.backgroundSprite != null)
            backgroundRenderer.sprite = level.backgroundSprite;

        WeaponManager weaponManager = WeaponManager.Instance != null ? WeaponManager.Instance : FindFirstObjectByType<WeaponManager>();
        if (weaponManager != null)
            weaponManager.SetLevelWeapon(level.weaponName, level.weaponDamage, level.weaponCooldown, level.weaponSprite);
        else
            Debug.LogWarning("SpawnManager nao encontrou WeaponManager para aplicar a arma do nivel.");
    }

    // Remove Scouts que sobraram antes de iniciar outro nivel.
    void ClearRemainingScouts()
    {
        ScoutHealth[] scouts = FindObjectsByType<ScoutHealth>(FindObjectsSortMode.None);
        foreach (ScoutHealth scout in scouts)
            Destroy(scout.gameObject);
    }

    // Remove cores de energia que sobraram antes de iniciar outro nivel.
    void ClearRemainingEnergyCores()
    {
        EnergyCore[] cores = FindObjectsByType<EnergyCore>(FindObjectsSortMode.None);
        foreach (EnergyCore core in cores)
            Destroy(core.gameObject);
    }

    // Atualiza o texto da UI com o progresso do nivel atual.
    void UpdateUI()
    {
        if (levelText == null || levels == null || levels.Length == 0) return;

        int levelNumber = currentLevelIndex + 1;
        if (levelActive)
        {
            LevelSettings level = CurrentLevel();
            levelText.text = "Nivel " + levelNumber + "/" + levels.Length + " - Nucleos: " + coresCollectedInLevel + "/" + CoresGoal(level) + " | Scouts: " + scoutsDefeatedInLevel + "/" + ScoutsGoal(level);
        }
        else if (gameComplete)
            levelText.text = "";
        else
            levelText.text = "";
    }
}
