using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField]
    Vector3 direction;
    [SerializeField]
    float speed = 1.0f;
    [SerializeField]
    LayerMask playerLayer;

    void OnTriggerStay(Collider other)
    {
        // Überprüfen, ob das Objekt im playerLayer ist
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            }
        }
    }
}
