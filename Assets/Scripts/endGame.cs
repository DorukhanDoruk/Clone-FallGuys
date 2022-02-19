using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endGame : MonoBehaviour
{
    [SerializeField] private GameObject sprayCanPrefab;
    [SerializeField] private Transform theWall;
    [SerializeField] private _Manager _manager;
    private Camera mCamera;
    private cameraFollow mCameraScript;
    private GameObject sprayCan;

    private void Start()
    {
        mCamera = Camera.main;
        mCameraScript = mCamera.GetComponent<cameraFollow>();
    }

    private void Update()
    {
        if(mCameraScript.state == cameraFollow.CameraState.congrulations && sprayCan != null) // if paint done destroy spraycan
            Destroy(sprayCan);
        
        if(sprayCan == null)
             return;

        float distanceToWall = Vector3.Dot(theWall.transform.position - mCamera.transform.position,mCamera.transform.forward);
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = (distanceToWall / 3) * 2; // fix the mouse z near to wall

        Vector3 desiredPosition = Camera.main.ScreenToWorldPoint(mousePos);

        sprayCan.transform.position = desiredPosition + new Vector3(0.5f,-1f,0);
    }

    private void OnTriggerEnter(Collider other) // if player reach endline
    {
        //if (other.CompareTag("Player")) //-> not nessesary anymore procejt physics settings are setup for collision between player and endpoint
        //{
        // Player animator
        other.GetComponent<Animator>().SetBool("Finished", true); // make him dance
        // Player Controller
        other.GetComponent<characterController>().enabled = false; // we dont need character controller anymore
        // face towards camera and repositioning
        other.transform.rotation = Quaternion.Euler(0,180,0);
        // swtich to endgame UI
        _manager.DisablePlayUI();
        
        
        mCameraScript.state = cameraFollow.CameraState.paintTheWall; // change the state of camera

        sprayCan = Instantiate(sprayCanPrefab, mCamera.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity); // create spraycan
        //}
    }
}
