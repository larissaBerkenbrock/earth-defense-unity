using UnityEngine;

[DisallowMultipleComponent]
public class FallingEnergyCore : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 2f;
    public float destroyY = -6f;

    [Header("Rotação")]
    public bool rotate = true;
    public float rotationSpeed = 120f;

    // Faz o nucleo cair, girar opcionalmente e se destruir ao sair da tela.
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        if (rotate)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
