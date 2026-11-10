using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public AudioClip somTiro;
    private AudioSource audioSource;
    [Header("Movimento")]
    public float velocidade = 10.0f;

    [Header("Tiro")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.3f;

    [Header("Dash")]
    public float dashSpeed = 30f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;

    Rigidbody2D playerRB;
    Animator anim;
    float lastDirection = 1f;
    float fireTimer = 0f;
    float firePointAbsX;

    bool isDashing = false;
    bool isDead = false;
    float dashTimer = 0f;
    float dashCooldownTimer = 0f;
    float dashDirection = 1f;

    // Inicializa audio, fisica, animacao e guarda a distancia do ponto de tiro.
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        playerRB = GetComponent<Rigidbody2D>();
        if (playerRB != null)
            playerRB.simulated = true;

        // Procura o Animator no próprio objeto ou em filhos
        anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (firePoint != null)
            firePointAbsX = Mathf.Abs(firePoint.localPosition.x);
    }

    // Le os inputs de tiro e dash, respeitando cooldowns e estado de morte.
    void Update()
    {
        if (isDead) return;

        fireTimer -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && fireTimer <= 0f && !isDashing)
        {
            Shoot();
            fireCooldown = WeaponManager.Instance != null ? WeaponManager.Instance.GetCooldown() : 0.5f;
            fireTimer = fireCooldown;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash();
        }
    }

    // Aplica o movimento horizontal, dash e animacao usando o ciclo de fisica.
    void FixedUpdate()
    {
        if (isDead)
        {
            StopMovement();
            return;
        }

        if (isDashing)
        {
            playerRB.linearVelocity = new Vector2(dashDirection * dashSpeed, playerRB.linearVelocity.y);
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f) isDashing = false;

            // Durante o dash considera sempre walking
            if (anim != null)
                anim.SetBool("isWalking", true);

            return;
        }

        float input = Input.GetAxis("Horizontal");
        playerRB.linearVelocity = new Vector2(velocidade * input, playerRB.linearVelocity.y);

        // Atualiza isWalking consoante se há input horizontal
        if (anim != null)
            anim.SetBool("isWalking", Mathf.Abs(input) > 0.1f);

        if (input > 0.1f)
        {
            lastDirection = 1f;
            FlipPlayer(1f);
        }
        else if (input < -0.1f)
        {
            lastDirection = -1f;
            FlipPlayer(-1f);
        }
    }

    // Vira o jogador para esquerda ou direita sem mudar o ponto de tiro.
    void FlipPlayer(float dir)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;

        if (firePoint != null)
        {
            Vector3 fp = firePoint.localPosition;
            fp.x = firePointAbsX;
            firePoint.localPosition = fp;
        }
    }

    // Cria uma bala, toca o som de tiro e aplica dano/direcao da arma atual.
    void Shoot()
    {
        if (isDead) return;

        if (somTiro != null) audioSource.PlayOneShot(somTiro, 0.5f);
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = WeaponManager.Instance != null ? WeaponManager.Instance.GetDamage() : bulletScript.damage;
            bulletScript.SetDirection(lastDirection);
        }
    }

    // Permite que outros scripts saibam se o jogador esta no dash.
    public bool IsDashing => isDashing;
    // Permite que outros scripts saibam se o jogador esta morto.
    public bool IsDead => isDead;

    // Inicia o dash na ultima direcao em que o jogador estava virado.
    void StartDash()
    {
        if (isDead) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = lastDirection;
    }

    // Atualiza o estado de morte do jogador e para qualquer movimento ativo.
    public void SetDead(bool dead)
    {
        isDead = dead;
        isDashing = false;
        StopMovement();
    }

    // Zera a velocidade do Rigidbody e desliga a animacao de caminhada.
    void StopMovement()
    {
        if (playerRB != null)
            playerRB.linearVelocity = Vector2.zero;

        if (anim != null)
            anim.SetBool("isWalking", false);
    }
}
