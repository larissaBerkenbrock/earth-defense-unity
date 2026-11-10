using UnityEngine;

public class EnergyCore : MonoBehaviour
{
    public AudioClip somRecolha;
    public float lifeTime = 10f;

    // Destroi o nucleo se o jogador nao recolher dentro do tempo.
    void Start() { Destroy(gameObject, lifeTime); }

    // Quando o jogador toca no nucleo, adiciona energia, toca som e remove o objeto.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerHealth>() != null)
        {
            EnergyManager.Instance?.AddCore();
            Destroy(gameObject);
            if (somRecolha != null)
                AudioSource.PlayClipAtPoint(somRecolha, transform.position);
        }
    }
}
