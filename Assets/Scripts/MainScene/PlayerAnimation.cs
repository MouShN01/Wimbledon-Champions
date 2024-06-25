using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerControl _playerControl;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayLeftRun(bool isRunning)
    {
        _animator.SetBool("isRunningLeft", isRunning);
    }

    public void PlayRightRun(bool isRunning)
    {
        _animator.SetBool("isRunningRight", isRunning);
    }

    public void PlayLeftSwing(bool isSwinging)
    {
        if (isSwinging)
        {
            _animator.SetBool("isHitting", true);
            _animator.Play("swingleft");
        }
        else
        {
            _animator.Play("unswingleft");
        }
    }
    
    public void PlayRightSwing(bool isSwinging)
    {
        if (isSwinging)
        {
            _animator.SetBool("isHitting", true);
            _animator.Play("swingright");
        }
        else
        {
            _animator.Play("unswingright");
        }
    }

    public void PlayLeftHit()
    {
        _animator.SetBool("isHitting", false);
        _animator.Play("hitleft");
    }

    public void PlayRightHit()
    {
        _animator.SetBool("isHitting", false);
        _animator.Play("hitright");
    }
}
