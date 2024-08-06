using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float flySpeed = 5f; // Kuþun uçma hýzý
    private Transform target; // Kuþun hedefi
    private bool isFleeing = false; // Kaçma durumu
    private Vector3 fleeDirection;

    void Start()
    {
        // Tarla üzerindeki ürünleri hedef olarak al
        GameObject[] products = GameObject.FindGameObjectsWithTag("product");
        if (products.Length > 0)
        {
            target = products[Random.Range(0, products.Length)].transform;
        }
    }

    void Update()
    {
        if (!isFleeing && target != null)
        {
            // Hedefe doðru hareket et
            transform.position = Vector3.MoveTowards(transform.position, target.position, flySpeed * Time.deltaTime);
        }
        else if (isFleeing)
        {
            // Kaçýþ yönüne doðru hareket et
            transform.position += fleeDirection * flySpeed * Time.deltaTime;
        }
    }

    public void Flee(Vector3 direction)
    {
        isFleeing = true;
        fleeDirection = direction;
    }
}
