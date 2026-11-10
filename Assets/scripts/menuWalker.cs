using UnityEngine;

public class MenuWalker : MonoBehaviour
{
    [Header("Movimento no Menu")]
    public float velocidade = 2f;
    public float limiteEsquerdo = -8f;
    public float limiteDireito = 8f;

    [Header("Animação")]
    public string parametroAndar = "isWalking";

    private Animator anim;
    private Rigidbody2D rb;

    private int direcao = 1;
    private float escalaOriginalX;
    private int parametroAndarHash;

    // Inicializa referencias, calcula a escala original e liga a animacao de andar.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        escalaOriginalX = Mathf.Abs(transform.localScale.x);
        parametroAndarHash = Animator.StringToHash(parametroAndar);

        AtivarAnimacaoAndar(true);
        VirarPersonagem();
    }

    // Atualiza o movimento do personagem do menu em tempo fixo de fisica.
    void FixedUpdate()
    {
        MoverNoMenu();
    }

    // Move o personagem entre os limites esquerdo e direito do menu.
    void MoverNoMenu()
    {
        Vector2 posicaoAtual = transform.position;

        posicaoAtual.x += direcao * velocidade * Time.fixedDeltaTime;

        if (posicaoAtual.x >= limiteDireito)
        {
            posicaoAtual.x = limiteDireito;
            direcao = -1;
            VirarPersonagem();
        }
        else if (posicaoAtual.x <= limiteEsquerdo)
        {
            posicaoAtual.x = limiteEsquerdo;
            direcao = 1;
            VirarPersonagem();
        }

        if (rb != null)
        {
            rb.MovePosition(posicaoAtual);
        }
        else
        {
            transform.position = posicaoAtual;
        }

        AtivarAnimacaoAndar(true);
    }

    // Vira o personagem para olhar na direcao em que esta andando.
    void VirarPersonagem()
    {
        Vector3 escala = transform.localScale;
        escala.x = escalaOriginalX * direcao;
        transform.localScale = escala;
    }

    // Liga ou desliga o parametro de animacao de caminhada.
    void AtivarAnimacaoAndar(bool estado)
    {
        if (anim != null)
        {
            anim.SetBool(parametroAndarHash, estado);
        }
    }
}
