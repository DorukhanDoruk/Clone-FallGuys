using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform player;
    [SerializeField] private Transform wall;
    [SerializeField] private Transform danceSpot;
    public Vector3 positionOffset; // for distance
    public Vector3 rotationOffset; // for rotation
    public CameraState state;
    public Coroutine cameraRoutine;

    public enum CameraState
    {
        followPlayer,
        paintTheWall,
        afterPainting,
        congrulations
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case CameraState.followPlayer:
                FollowPlayer();
                break;
            case CameraState.paintTheWall:
                Vector3 movePosition = wall.position + new Vector3(positionOffset.x, positionOffset.y-8f, positionOffset.z);
                
                if(cameraRoutine == null)
                    cameraRoutine = StartCoroutine(CinematicMovement(movePosition, 3));
                // we may set cameraroutine == null after we are done with it but not that necessary rn
                break;
            case CameraState.afterPainting:
                player.position = danceSpot.position;
                Vector3 movePos = wall.position + new Vector3(positionOffset.x, positionOffset.y-8f, positionOffset.z-8f);

                if (cameraRoutine == null)
                    cameraRoutine = StartCoroutine(CinematicMovement(movePos, 3));
                break;
            case CameraState.congrulations:
                //dont do anything yet
                break;
        }
    }

    private void FollowPlayer()
    {
        transform.rotation = Quaternion.Euler(30f, transform.rotation.y, transform.rotation.z);
        Vector3 movePosition = player.position + positionOffset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, movePosition, 0.2f);
        transform.position = smoothPos;
    }
    
    IEnumerator CinematicMovement(Vector3 targetPos, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            // Calculation
            float eulerX = Mathf.Lerp(0, rotationOffset.x - 10, time / duration); // 0 to current rotation + new rotation
            // Apply
            transform.rotation = Quaternion.Euler(rotationOffset.x > 10 ? (rotationOffset.x - eulerX) : (rotationOffset.x + eulerX), transform.rotation.y, transform.rotation.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, time / duration);
            // Handle
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // move to final position cause lerp will stop at 0,99;
        if(state == CameraState.afterPainting)
            state = CameraState.congrulations;
        
        StopCoroutine(cameraRoutine);
    }
}
