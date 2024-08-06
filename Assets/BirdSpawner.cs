using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab; // Kuþ prefabý
    public Transform spawnPoint; // Kuþlarýn doðacaðý nokta
    public int birdCount = 5; // Doðurulacak kuþ sayýsý
    public float spawnInterval = 1f; // Kuþlarýn doðma aralýðý

    void Start()
    {
        StartCoroutine(SpawnBirds());
    }

    IEnumerator SpawnBirds()
    {
        for (int i = 0; i < birdCount; i++)
        {
            Instantiate(birdPrefab, spawnPoint.position, spawnPoint.rotation);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
