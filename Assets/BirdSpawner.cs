using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab; // Ku� prefab�
    public Transform spawnPoint; // Ku�lar�n do�aca�� nokta
    public int birdCount = 5; // Do�urulacak ku� say�s�
    public float spawnInterval = 1f; // Ku�lar�n do�ma aral���

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
