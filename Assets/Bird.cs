using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float flySpeed = 5f; // Ku�un u�ma h�z�
    private Transform target; // Ku�un hedefi
    private bool isFleeing = false; // Ka�ma durumu
    private Vector3 fleeDirection;

    void Start()
    {
        // Tarla �zerindeki �r�nleri hedef olarak al
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
            // Hedefe do�ru hareket et
            transform.position = Vector3.MoveTowards(transform.position, target.position, flySpeed * Time.deltaTime);
        }
        else if (isFleeing)
        {
            // Ka��� y�n�ne do�ru hareket et
            transform.position += fleeDirection * flySpeed * Time.deltaTime;
        }
    }

    public void Flee(Vector3 direction)
    {
        isFleeing = true;
        fleeDirection = direction;
    }
}
