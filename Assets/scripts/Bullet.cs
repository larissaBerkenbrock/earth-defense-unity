using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public int damage = 1;

    private float direction = 1f;

    // Destroi a bala automaticamente depois do tempo de vida configurado.
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Move a bala na direcao definida a cada frame.
    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime, Space.World);
    }

    // Define para que lado a bala vai se mover.
    public void SetDirection(float dir)
    {
        direction = dir;
        // Não mexemos mais na escala — apenas na direção do movimento
    }

    // Detecta colisao com inimigos, aplica dano e destroi a bala.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            ScoutHealth sh = other.GetComponent<ScoutHealth>();
            if (sh != null) sh.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
