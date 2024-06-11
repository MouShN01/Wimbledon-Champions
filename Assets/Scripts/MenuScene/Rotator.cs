using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Transform target; 
    public float distance = 30.0f; 
    public float rotationSpeed = 10.0f;

    void Start()
    {
        if (target != null)
        {
            transform.position = target.position + new Vector3(0, distance, -distance);
            transform.LookAt(target);
        }
    }

    void Update()
    {
        if (target != null)
        {
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
            transform.LookAt(target);
        }
    }
}
