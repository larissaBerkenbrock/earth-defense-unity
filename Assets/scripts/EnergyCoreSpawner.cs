using UnityEngine;

public class EnergyCoreSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject energyCorePrefab;

    [Header("Spawn")]
    public float spawnInterval = 0.5f;
    public float spawnY = 6f;
    public float minX = -8f;
    public float maxX = 8f;

    [Header("Variação")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 4f;
    public float minScale = 0.5f;
    public float maxScale = 1.2f;

    private float timer;

    // Conta o tempo e cria novos nucleos em intervalos regulares.
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEnergyCore();
            timer = spawnInterval;
        }
    }
 
    // Cria um nucleo em uma posicao aleatoria e aplica tamanho/velocidade aleatorios.
    void SpawnEnergyCore()
    {
        if (energyCorePrefab == null)
            return;

        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        GameObject core = Instantiate(energyCorePrefab, spawnPosition, Quaternion.identity);

        float randomScale = Random.Range(minScale, maxScale);
        core.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        FallingEnergyCore fallingCore = core.GetComponent<FallingEnergyCore>();

        if (fallingCore != null)
        {
            fallingCore.speed = Random.Range(minSpeed, maxSpeed);
        }
    }
}
