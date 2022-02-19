using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotateAxis = Vector3.down;
    private void FixedUpdate()
    {
        transform.Rotate(rotateAxis, 1f); // fixed speed
    }
}
