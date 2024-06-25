using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Mirror;
using Unity.VisualScripting;

public class PlayerControl : NetworkBehaviour
{
    public static PlayerControl S;
    private PlayerNetwork _playerNetwork;
    private PlayerAnimation _playerAnimation;
    public Camera mainCam;
    public Vector3 camOffset;
    public float speed = 0.5f;

    [SyncVar(hook = nameof(OnLoverBorderChanged))]
    public float loverBorder = -10f;
    [SyncVar(hook = nameof(OnUpperBorderChanged))]
    public float upperBorder = 10f;
    [SyncVar(hook = nameof(OnRightHandSwingChanged))]
    public bool isRightHandSwing = false;
    [SyncVar(hook = nameof(OnLeftHandSwingChanged))]
    public bool isLeftHandSwing = false;
    [SyncVar(hook = nameof(OnCenterModeChanged))]
    public bool centerMode = false;

    public int score = 0;
    public int games = 0;
    public int sets = 0;
    
    public Animator anim;

    public GameObject hudPrefab;
    private GameObject hudInstance;
    private UIHandler uiHandler;
    
    public float swingSpeed = 1f;
    public bool isSwinging = false;
    
    public Ball ball;

    void Awake()
    {
        S = this;
        _playerNetwork = GetComponent<PlayerNetwork>();
        _playerAnimation = GetComponent<PlayerAnimation>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        mainCam = Camera.main;
        camOffset = new Vector3(0, 16.17f, -9.72f);
        SetCamPos();
        
        hudInstance = Instantiate(hudPrefab);
        hudInstance.transform.SetParent(GameObject.Find("Canvas(Clone)").transform, false);
        
        uiHandler = hudInstance.GetComponent<UIHandler>();
        uiHandler.Player = this;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (isSwinging)
        {
            _playerNetwork.CmdUpdateBorders(true);
        }
        else
        {
            _playerNetwork.CmdUpdateBorders(false);
        }
        
        UpdateHUD();
        
    }

    private void OnLoverBorderChanged(float oldLoverBorder, float newLoverBorder)
    {
        loverBorder = newLoverBorder;
        UpdateHUD();
    }

    private void OnUpperBorderChanged(float oldUpperBorder, float newUpperBorder)
    {
        upperBorder = newUpperBorder;
        UpdateHUD();
    }

    private void OnRightHandSwingChanged(bool oldRightHandSwing, bool newRightHandSwing)
    {
        isRightHandSwing = newRightHandSwing;
    }

    private void OnLeftHandSwingChanged(bool oldLeftHandSwing, bool newLeftHandSwing)
    {
        isLeftHandSwing = newLeftHandSwing;
    }

    private void OnCenterModeChanged(bool oldCenterMode, bool newCenterMode)
    {
        centerMode = newCenterMode;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Ball"))
        {
            this.GetComponent<AudioSource>().Play();
            if (isRightHandSwing && !centerMode)
            {
                ball.isHitted = false;
                _playerAnimation.PlayRightHit();
                ball.transform.rotation = Quaternion.AngleAxis(-10, Vector3.up);
            }

            if (isLeftHandSwing && !centerMode)
            {
                ball.isHitted = false;
                _playerAnimation.PlayLeftHit();
                ball.transform.rotation = Quaternion.AngleAxis(10, Vector3.up);
            }
            if (centerMode)
            {
                ball.isHitted = false;
                _playerAnimation.PlayLeftHit();
                ball.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
            }
        }
    }
    

    void SetCamPos()
    {
        Vector3 rotatedOffset = Quaternion.Euler(0, transform.eulerAngles.y, 0) * camOffset;
        mainCam.transform.position = transform.position + rotatedOffset;

        mainCam.transform.LookAt(transform.position + Vector3.up * 10);
    }

    void UpdateHUD()
    {
        if (uiHandler != null)
        {
            uiHandler.UpdateHUD(upperBorder, loverBorder);
        }
    }
}
