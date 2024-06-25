using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInput : NetworkBehaviour
{
    private PlayerActions _playerActions;
    private PlayerAnimation _playerAnimation;
    private PlayerNetwork _playerNetwork;
    private PlayerControl _playerControl;

    private void Start()
    {
        _playerActions = GetComponent<PlayerActions>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerNetwork = GetComponent<PlayerNetwork>();
        _playerControl = GetComponent<PlayerControl>();
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        HorizontalMovement();
        
        if (Input.GetMouseButtonDown(0))
        {
            _playerNetwork.CmdSetRightHandSwing(false);
            _playerNetwork.CmdSetLeftHandSwing(true);
            _playerAnimation.PlayLeftSwing(true);
            _playerControl.isSwinging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _playerAnimation.PlayLeftSwing(false);
            _playerControl.isSwinging = false;
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            _playerNetwork.CmdSetRightHandSwing(true);
            _playerNetwork.CmdSetLeftHandSwing(false);
            _playerControl.isSwinging = true;
            _playerAnimation.PlayRightSwing(_playerControl.isSwinging);
        }

        if (Input.GetMouseButtonUp(1))
        {
            _playerControl.isSwinging = false;
            _playerAnimation.PlayRightSwing(_playerControl.isSwinging);
        }
        
        if (Input.GetMouseButtonDown(2) && !_playerControl.centerMode)
        {
            _playerNetwork.CmdSetCenterMode(true);
        }
        else if (Input.GetMouseButtonDown(2) && _playerControl.centerMode)
        {
            _playerNetwork.CmdSetCenterMode(false);
        }
    }

    private void HorizontalMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        _playerActions.MovePlayer(horizontalInput);
        if (horizontalInput > 0)
        {
            _playerAnimation.PlayRightRun(true);
            _playerAnimation.PlayLeftRun(false);
        }
        else if(horizontalInput < 0)
        {
            _playerAnimation.PlayRightRun(false);
            _playerAnimation.PlayLeftRun(true);
        }
        else
        {
            _playerAnimation.PlayRightRun(false);
            _playerAnimation.PlayLeftRun(false);
        }
    }
}
