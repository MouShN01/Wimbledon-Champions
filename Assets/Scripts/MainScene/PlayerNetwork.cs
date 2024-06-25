using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNetwork : NetworkBehaviour
{
    private PlayerControl _playerControl;

    private void Start()
    {
        _playerControl = GetComponent<PlayerControl>();
    }

    [Command]
    public void CmdUpdateBorders(bool isSwinging)
    {
        if (isSwinging)
        {
            _playerControl.loverBorder = Mathf.Min(_playerControl.loverBorder + _playerControl.swingSpeed, 90);
            _playerControl.upperBorder = Mathf.Min(_playerControl.upperBorder + _playerControl.swingSpeed, 110);
        }
        else
        {
            _playerControl.loverBorder = Mathf.Max(_playerControl.loverBorder - _playerControl.swingSpeed, -10);
            _playerControl.upperBorder = Mathf.Max(_playerControl.upperBorder - _playerControl.swingSpeed, 10);
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdSetRightHandSwing(bool value)
    {
        _playerControl.isRightHandSwing = value;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetLeftHandSwing(bool value)
    {
        _playerControl.isLeftHandSwing = value;
    }

    [Command]
    public void CmdSetCenterMode(bool value)
    {
        _playerControl.centerMode = value;
    }
}
