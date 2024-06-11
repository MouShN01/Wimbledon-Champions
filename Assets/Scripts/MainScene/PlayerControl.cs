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
    public Camera mainCam;
    public Vector3 camOffset;
    private Rigidbody rb;
    public GameObject headGig;
    public Rig rig;
    public float swingSpeed = 1f;
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
    private Vector3 moveVector;
    public Animator anim;
    public bool isSwinging = false;
    public bool isLastHit = false;
    public Ball ball;

    public GameObject hudPrefab;
    private GameObject hudInstance;
    private UIHandler uiHandler;

    public GameController gC;

    void Awake()
    {
        S = this;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rig = headGig.GetComponent<Rig>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        mainCam = Camera.main;
        camOffset = new Vector3(0, 16.17f, -9.72f);
        SetCamPos();

        // Создаем HUD для локального игрока
        hudInstance = Instantiate(hudPrefab);
        hudInstance.transform.SetParent(GameObject.Find("Canvas(Clone)").transform, false);

        // Получаем ссылку на UIHandler и передаем ему ссылку на игрока
        uiHandler = hudInstance.GetComponent<UIHandler>();
        uiHandler.Player = this;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float adjustedHorizontalInput = transform.eulerAngles.y == 180 ? -horizontalInput : horizontalInput;

        moveVector.x = adjustedHorizontalInput;
        rb.MovePosition(rb.position + moveVector * speed * Time.deltaTime);

        if (isSwinging)
        {
            CmdUpdateBorders(true);
        }
        else
        {
            CmdUpdateBorders(false);
        }

        RightHandHit();
        LeftHandHit();
        RunRight();
        RunLeft();
        CenterHitMode();
        UpdateHUD();

        if (ball == null)
        {
            ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
        }
        else
        {
            if (headGig.transform.position.z >= ball.transform.position.z)
            {
                rig.weight = 0;
            }
            else
            {
                rig.weight = 100;
            }
        }
    }

    [Command]
    void CmdUpdateBorders(bool isSwinging)
    {
        if (isSwinging)
        {
            loverBorder = Mathf.Min(loverBorder + swingSpeed, 90);
            upperBorder = Mathf.Min(upperBorder + swingSpeed, 110);
        }
        else
        {
            loverBorder = Mathf.Max(loverBorder - swingSpeed, -10);
            upperBorder = Mathf.Max(upperBorder - swingSpeed, 10);
        }
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

    [Command]
    void CmdSetRightHandSwing(bool value)
    {
        isRightHandSwing = value;
    }

    [Command]
    void CmdSetLeftHandSwing(bool value)
    {
        isLeftHandSwing = value;
    }

    [Command]
    void CmdSetCenterMode(bool value)
    {
        centerMode = value;
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
                anim.Play("hitright");
                ball.isHitted = false;
                anim.SetBool("isHitting", false);
                ball.transform.rotation = Quaternion.AngleAxis(-10, Vector3.up);
            }

            if (isLeftHandSwing && !centerMode)
            {
                anim.Play("hitleft");
                ball.isHitted = false;
                anim.SetBool("isHitting", false);
                ball.transform.rotation = Quaternion.AngleAxis(10, Vector3.up);
            }
            if (centerMode)
            {
                anim.Play("hitleft");
                ball.isHitted = false;
                anim.SetBool("isHitting", false);
                ball.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
            }
        }
    }

    void CenterHitMode()
    {
        if (Input.GetMouseButtonDown(2) && !centerMode)
        {
            CmdSetCenterMode(true);
        }
        else if (Input.GetMouseButtonDown(2) && centerMode)
        {
            CmdSetCenterMode(false);
        }
    }

    void RightHandHit()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CmdSetRightHandSwing(true);
            CmdSetLeftHandSwing(false);
            anim.SetBool("isHitting", true);
            isSwinging = true;
            anim.Play("swingright");
        }

        if (Input.GetMouseButtonUp(1))
        {
            anim.Play("unswingright");
            isSwinging = false;
        }
    }

    void LeftHandHit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CmdSetRightHandSwing(false);
            CmdSetLeftHandSwing(true);
            anim.SetBool("isHitting", true);
            isSwinging = true;
            anim.Play("swingleft");
        }

        if (Input.GetMouseButtonUp(0))
        {
            anim.Play("unswingleft");
            isSwinging = false;
        }
    }

    void RunRight()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetBool("isRunningRight", true);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            anim.SetBool("isRunningRight", false);
        }
    }

    void RunLeft()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetBool("isRunningLeft", true);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            anim.SetBool("isRunningLeft", false);
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
