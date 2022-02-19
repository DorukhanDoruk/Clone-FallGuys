using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

public class navigateBots : MonoBehaviour 
{
    public Transform spawnPoint;
    public Transform finishLine;
    private Animator _animator;
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject failureParticles;

    private Coroutine calculationRoutine;
    private Vector3 lastPos;
    private float distance;
    
    // a* pathfinding
    private AIPath aiPath;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        _animator = GetComponent<Animator>();
        lastPos = transform.position;
    }

    public void StartNavigation()
    {
        aiPath.destination = finishLine.position;
        calculationRoutine = StartCoroutine(CalculateSpeedForAnimation());
    } 

    private void OnCollisionEnter(Collision col)
    {
        if (!col.collider.CompareTag("Obstacles")) return;
        // use particles +1 on y axis to make sure its not spawned inside ground
        Instantiate(failureParticles, transform.position + new Vector3(0,1,0), Quaternion.identity); // prefab already has destroy method init
        // respawn
        transform.position = spawnPoint.transform.position;
    }

    /*
     * i need to do something like this cause A* Pathfinding library doesnt use rigidbody velocity so
     * only way to get a speed value from gameobject calculating distance between positions with periods
     */
    IEnumerator CalculateSpeedForAnimation()
    {
        while (true)
        {
            // a primitive way to findout if gameobject moving
            distance = Vector3.Distance(lastPos, transform.position);
            lastPos = transform.position;

            if (distance > 0.3f) // if moving even a bit set as 1
                distance = 1;

            _animator.SetFloat("Speed", distance);
            
            yield return new WaitForSeconds(0.2f);
        }
    }
}
