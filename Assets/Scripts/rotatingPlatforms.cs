using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatingPlatforms : MonoBehaviour
{
    [SerializeField] private Vector3 rotateAxis = Vector3.down;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.angularVelocity = rotateAxis;
        //transform.Rotate(rotateAxis, rotatingSpeed); // old way too
    }

    private void OnCollisionEnter(Collision col)
    { 
        // old way to do it works, but i dont want them to works like that
        //col.transform.parent = transform;
    }

    private void OnCollisionExit(Collision col)
    {
        //col.transform.parent = playerParent;
    }
}
