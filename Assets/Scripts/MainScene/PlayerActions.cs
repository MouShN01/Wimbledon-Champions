using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private  PlayerControl _playerControl; 
    private  Rigidbody _rb;
    private Vector3 _moveVector;
   

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerControl = GetComponent<PlayerControl>();
    }

    public void MovePlayer(float moveDirection)
    {
        float adjustedHorizontalInput = transform.eulerAngles.y == 180 ? -moveDirection : moveDirection;
        
        _moveVector.x = adjustedHorizontalInput;
        _rb.MovePosition(_rb.position + _moveVector * _playerControl.speed * Time.deltaTime);
    }
}
