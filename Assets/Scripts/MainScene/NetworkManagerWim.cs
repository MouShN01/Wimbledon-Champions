using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkManagerWim : NetworkManager
{
    public Transform firstPlayerSpawn;
    public Transform secondPlayerSpawn;
    GameObject ball;
    private GameObject gameManager;
    private GameObject screenP1;
    private GameObject screenP2;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // add player at correct spawn position
        Transform start;
        if (numPlayers == 0)
        {
            firstPlayerSpawn = GameObject.Find("Point1").transform;
            start = firstPlayerSpawn;
        }
        else
        {
            secondPlayerSpawn = GameObject.Find("Point2").transform;
            start = secondPlayerSpawn;
        }
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        // spawn ball if two players
        if (numPlayers == 2)
        {
            ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
            NetworkServer.Spawn(ball);
            gameManager = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));
            NetworkServer.Spawn(gameManager);
            screenP1 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "ScoreScreenP1"));
            NetworkServer.Spawn(screenP1);
            screenP2 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "ScoreScreenP2"));
            NetworkServer.Spawn(screenP2);

        }
    }
}
