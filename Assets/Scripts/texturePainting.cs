using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class texturePainting : MonoBehaviour
{
    private Camera mCamera;
    private cameraFollow mCameraScript;
    
    private Texture2D _texture2D;
    [Range(2, 128)] [SerializeField] private int resWall = 128;
    private bool isDrawable;
    private float brushSize;
    private int fixedBrushSize = 12;

    private bool isPaintDone;
    // texture bounds
    private Bounds _bounds;
    private Vector3 maxBounds, minBounds, boundSize;
    
    // ui calculations
    private int totalCellCount;
    private int paintedCellCount;
    private float paintPercentage;
    
    // painting variables
    private float percentageOfX;
    private float percentageOfY;
    private int calculatedX;
    private int calculatedY;
    private int uValue;
    private int vValue;
    
    // ui
    [SerializeField] private TextMeshProUGUI paintText;

    private void Start()
    {
        // camera
        mCamera = Camera.main;
        mCameraScript = mCamera.GetComponent<cameraFollow>();
        
        // texture
        _texture2D = new Texture2D(resWall, resWall);
        GetComponent<Renderer>().material.mainTexture = _texture2D;

        // For calculations
        _bounds = GetComponent<Renderer>().bounds;
        maxBounds = _bounds.max;
        minBounds = _bounds.min;
        boundSize = _bounds.size;
        
        // set value
        brushSize = fixedBrushSize * fixedBrushSize; // brushsize
        totalCellCount = resWall * resWall;
    }

    private void Update()
    {
        if(mCameraScript.state != cameraFollow.CameraState.paintTheWall)
            return;
        
        if (Input.GetMouseButton(0) && !isPaintDone)
        {
            // painting
            Vector3 screenToWorldPoint = MouseWallPosition(Input.mousePosition);

            if (isDrawable)
            {
                // mouse x / total x * resolution of wall
                percentageOfX = ((screenToWorldPoint.x + maxBounds.x) / boundSize.x) * resWall;
                percentageOfY = ((screenToWorldPoint.y - minBounds.y) / boundSize.y) * resWall;
                
                calculatedX = Mathf.FloorToInt((resWall / 100) * percentageOfX);
                calculatedY = Mathf.FloorToInt((resWall / 100) * percentageOfY);

                for (int u = 0; u < Mathf.Min(calculatedX + fixedBrushSize , resWall); u++)
                {
                    for (int v = 0; v < Mathf.Min(calculatedY + fixedBrushSize , resWall); v++)
                    {
                        uValue = calculatedX - u; // calculate uv coordinates
                        vValue = calculatedY - v;

                        if (!((uValue * uValue) + (vValue * vValue) < brushSize)) continue;
                        // i have spend hours to figure out why color always return 0.804f instead of pure white
                        // probably cause of ambient and skybox but i dont have much time to solve this rn
                        if (!_texture2D.GetPixel(u, v).Equals(Color.red))
                        {
                            _texture2D.SetPixel(u,v, Color.red);
                            paintedCellCount += 1;//for ui calculation
                        }
                        // if its red dont count it needed for calculation of paint percentage
                    }
                }
                _texture2D.Apply();

                paintPercentage = ((float)paintedCellCount / (float)totalCellCount) * 100f;

                if (paintPercentage >= 99.99f) // sometimes player can miss 1 pixel hard to see, give the palyer some tolerance
                {
                    PaintDone();
                    paintPercentage = 100f;
                }

                UpdateUI();
            }
        }
    }

    private Vector3 MouseWallPosition(Vector3 mouseWorldPos)
    {
        float distanceToWall = Vector3.Dot(transform.position - mCamera.transform.position, mCamera.transform.forward); // get distance between camera and wall
        mouseWorldPos.z = distanceToWall; // z equal distance now calculations will made like camera front of wall with 90 degree

        return mCamera.ScreenToWorldPoint(mouseWorldPos); // return the world pos as screen to world for drawing
    }

    private void PaintDone()
    {
        mCameraScript.cameraRoutine = null;
        mCameraScript.state = cameraFollow.CameraState.afterPainting;
    }

    private void UpdateUI()
    {
        paintText.text = $"{Mathf.Floor(paintPercentage)}%";
        paintText.color = new Color(1 - (paintPercentage / 100), paintPercentage / 100, 0);
    }

    private void OnMouseOver()
    {
        // Mouse or finger is hovering this object so its drawable,
        isDrawable = true;
    }
    
    private void OnMouseExit()
    {
        isDrawable = false;
    }
}
