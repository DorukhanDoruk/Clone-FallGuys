using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController : MonoBehaviour
{
    // Rigidbody & Movement
    public Transform spawnPoint;
    [SerializeField] private GameObject failureParticles;
    [SerializeField] private bool touchControl;
    private Vector3 moveDirection;
    private float moveSpeed = 5f;
    
    // Touch Control
    private Vector3 touchPos;
    
    // Animator
    [SerializeField] private Animator _animator;

    private void Update()
    {
        if (!touchControl) // also can play with WASD for windows
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            moveDirection.Normalize();
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) // take touch spot once
            {
                touchPos = new Vector3(Input.mousePosition.x, 0f, Input.mousePosition.y);
            }

            if (Input.GetMouseButton(0)) // keep updating the movement direction while pressing
            {
                Vector3 dragDistance = new Vector3(Input.mousePosition.x, 0f, Input.mousePosition.y) - touchPos;
                moveDirection = Vector3.ClampMagnitude(dragDistance, 1f);
            }
            else // reset the values if stop touching
            {
                touchPos = Vector3.zero;
                moveDirection = Vector3.zero;
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        HandleAnimations(moveDirection);
    }

    private void Move()
    {
        // handle rigidbodys velocity and rotation
        if (moveDirection != Vector3.zero) // if drection exist
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            
            Quaternion rotateTo = Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, rotateTo, 720 * Time.deltaTime);
        }
        
    }

    private void HandleAnimations(Vector3 movementData)
    {
        float velocity = Vector3.Distance(Vector3.zero, movementData);
        _animator.SetFloat("Speed", velocity);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Obstacles"))
        {
            // use particles +1 on y axis to make sure its not spawned inside ground
            Instantiate(failureParticles, transform.position + new Vector3(0,1,0), Quaternion.identity); // prefab already has destroy method init
            // respawn
            transform.position = spawnPoint.transform.position;
        }
    }
}
