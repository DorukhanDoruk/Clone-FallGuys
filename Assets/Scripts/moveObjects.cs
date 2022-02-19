using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveObjects : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector3 moveAxis = Vector3.forward;

    private Vector3 startPos, endPos;

    private void Start()
    {
        startPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        endPos = new Vector3(
            moveAxis.x!=0?moveAxis.x:transform.localPosition.x, 
            moveAxis.y!=0?moveAxis.y:transform.localPosition.y, 
            moveAxis.z!=0?moveAxis.z:transform.localPosition.z);
    }

    void FixedUpdate()
    {
        float lerpValue = (Mathf.Sin(speed * Time.time) + 1f) / 2f;
        // Sin always returns -1 to 1 adding, +1 makes 0 to 2 and division with 2 means 0 to 1
        transform.localPosition = Vector3.Lerp(startPos, endPos, lerpValue);
    }
}
