using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreScreen : NetworkBehaviour
{
    private GameObject[] players;
    public PlayerControl P1;
    public PlayerControl P2;
    
    public TextMeshPro setP1;
    public TextMeshPro setP2;
    public TextMeshPro gameP1;
    public TextMeshPro gameP2;
    public TextMeshPro scoreP1;
    public TextMeshPro scoreP2;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("player");
        P1 = players[0].GetComponent<PlayerControl>();
        P2 = players[1].GetComponent<PlayerControl>();
    }
    private void Update()
    {
        ScoreTranslate(P1, scoreP1);
        ScoreTranslate(P2, scoreP2);
        gameP1.text = P1.games.ToString();
        gameP2.text = P2.games.ToString();
        setP1.text = P1.sets.ToString();
        setP2.text = P2.sets.ToString();
    }
    
    void ScoreTranslate(PlayerControl player, TextMeshPro elem)
    {
        switch (player.score)
        {
            case 0:
                elem.text = "0";
                break;
            case 1:
                elem.text = "15";
                break;
            case 2:
                elem.text = "30";
                break;
            case 3:
                elem.text = "40";
                break;
            case 4:
                elem.text = "50";
                break;
        }
    }
}
