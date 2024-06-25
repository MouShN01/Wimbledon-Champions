using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadRigging : MonoBehaviour
{
    [SerializeField] private Transform sourceObject;
    [SerializeField] private GameObject headAimObject;
    [SerializeField] private Rig rig;

    private void Update()
    {
        if (sourceObject == null)
        {
            sourceObject = FindObjectOfType<Ball>().transform;
        }
        else
        {
            headAimObject.transform.position = sourceObject.transform.position;
        }

        rig.weight = Mathf.Abs(rig.transform.position.z - sourceObject.transform.position.z) < 1 ? 0 : 1;
    }
}
