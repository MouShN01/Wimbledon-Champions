using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;

public class Ball : NetworkBehaviour
{
    public static Ball B;
    public Rigidbody rb;
    private int direction;
    [SyncVar]
    public Vector3 vel;
    private float waitTime = 1.0f;
    private float nextUpdateTime;
    public float speed = 2;
    private float chargeSpeed = 0.2f;
    [SyncVar]
    public bool isHitted = false;
    public float hitForce = 11f;
    
    [SyncVar]
    public float hitPower = 0f;
    
    private void Awake()
    {
        B = this;
    }

    private void Start()
    {
        StartCoroutine(UpdateDirection());
        rb = GetComponent<Rigidbody>();
        if (this.transform.position.z < 0)
        {
            vel = Vector3.forward * speed;
        }
        else
        {
            vel = -Vector3.forward * speed;
        }
        rb.velocity = vel.normalized * hitForce + new Vector3(0, 8, 0);
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + waitTime;
        }
        if (direction == 0)
        {
            hitPower = Mathf.Max(hitPower -= chargeSpeed, 0);
        }
        else
        {
            hitPower = Mathf.Min(hitPower += chargeSpeed, 100);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        GameObject go = col.gameObject;
        PlayerControl pc = go.GetComponent<PlayerControl>();
        if (go.CompareTag("player") && hitPower >= pc.loverBorder && hitPower <= pc.upperBorder)
        {
            vel *= -1;
            isHitted = true;
            rb.velocity = transform.rotation * vel.normalized * hitForce + new Vector3(0, 8, 0);
        }
    }

    IEnumerator UpdateDirection()
    {
        while (true)
        {
            direction = Random.Range(0, 2);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
