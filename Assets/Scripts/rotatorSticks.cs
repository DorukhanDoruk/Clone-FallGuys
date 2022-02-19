using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatorSticks : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        // collision pos - player pos
        Vector3 dir = col.contacts[0].point - col.transform.position;
        
        col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(
            dir.x <= 0 ? 5: -5,
            3,
            dir.z <= 0 ? 5: -5) * 1.2f;
    }
}
