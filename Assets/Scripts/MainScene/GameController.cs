using System;
using UnityEngine;
using Mirror;

public class GameController : NetworkBehaviour
{
    private GameObject[] players;
    public PlayerControl player1;
    public PlayerControl player2;
    public Transform ballSpawnPosP1;
    public Transform ballSpawnPosP2;
    public GameObject ballPrefab;
    public GameObject ball;
    [SyncVar]
    public bool isGameEnded = false;
    [SyncVar]
    public string winnersMsg;

    [SyncVar]
    public bool isFirstPlayerServe = false;

    public int fieldLeftXBorder = -17;
    public int fieldRightXBorder = 17;

    public UIHandler uiHandler;

    public SoundManager sM;

    
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.OnDisconnectedEvent += OnClientDisconnected;
    }

    void OnDestroy()
    {
        NetworkServer.OnDisconnectedEvent -= OnClientDisconnected;
    }

    private void OnClientDisconnected(NetworkConnection conn)
    {
        // Проверяем, если подключен только один игрок, это хост
        if (NetworkServer.connections.Count == 1)
        {
            StopGame();
        }
    }
    
    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("player");
        player1 = players[0].GetComponent<PlayerControl>();
        player2 = players[1].GetComponent<PlayerControl>();
        ballSpawnPosP1 = players[0].transform.Find("PlayerSpawnball");
        ballSpawnPosP2 = players[1].transform.Find("PlayerSpawnball");
        ball = GameObject.FindGameObjectWithTag("Ball");
        uiHandler = GameObject.Find("Canvas(Clone)").GetComponent<UIHandler>();
        uiHandler.gameCon = this;
        sM = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }
    

    void FixedUpdate()
    {
        if (isServer)
        {
            if (ball == null || isGameEnded) return;
            ScoreControl();
            GamesCount();
            SetsCount();
            WinCheck();
        }
    }

    void WinCheck()
    {
        if (player1.sets - player2.sets > 2)
        {
            winnersMsg = "Player 1 is the Champion!";
            isGameEnded = true;
            RpcShowWinner(winnersMsg); // Добавьте этот вызов
        }

        if (player2.sets - player1.sets > 2)
        {
            winnersMsg = "Player 2 is the Champion!";
            isGameEnded = true;
            RpcShowWinner(winnersMsg); // Добавьте этот вызов
        }
    }

    [ClientRpc]
    void RpcShowWinner(string winnerMessage)
    {
        uiHandler.winBox.SetActive(true);
        uiHandler.winText.text = winnerMessage;
    }

    [Server]
    void SetsCount()
    {
        if (player1.games == 6)
        {
            player1.sets++;
            player1.games = 0;
            player2.games = 0;
            player1.score = 0;
            player2.score = 0;
            RpcUpdateSets(player1.sets, player2.sets);
            RpcUpdateGames(player1.games, player2.games);
            RpcUpdateScore(player1.score, player2.score);
        }

        if (player2.games == 6)
        {
            player2.sets++;
            player1.games = 0;
            player2.games = 0;
            player1.score = 0;
            player2.score = 0;
            RpcUpdateSets(player1.sets, player2.sets);
            RpcUpdateGames(player1.games, player2.games);
            RpcUpdateScore(player1.score, player2.score);
        }
    }

    [Server]
    void GamesCount()
    {
        if (player1.score == 4)
        {
            player1.games++;
            player1.score = 0;
            player2.score = 0;
            RpcUpdateGames(player1.games, player2.games);
            RpcUpdateScore(player1.score, player2.score);
        }
        if (player2.score == 4)
        {
            player2.games++;
            player1.score = 0;
            player2.score = 0;
            RpcUpdateGames(player1.games, player2.games);
            RpcUpdateScore(player1.score, player2.score);
        }

        if (player1.score == 3 && player2.score == 3)
        {
            player1.score = 0;
            player2.score = 0;
            player1.games++;
            player2.games++;
            RpcUpdateGames(player1.games, player2.games);
            RpcUpdateScore(player1.score, player2.score);
        }
    }
    
    [Server]
    void RespawnBall()
    {
        Transform ballSpawnPos = isFirstPlayerServe ? ballSpawnPosP1 : ballSpawnPosP2;
        isFirstPlayerServe = !isFirstPlayerServe;

        if (ball != null)
        {
            NetworkServer.Destroy(ball);
        }

        ball = Instantiate(ballPrefab, ballSpawnPos.position, ballSpawnPos.rotation);
        NetworkServer.Spawn(ball);
        player1.ball = ball.GetComponent<Ball>();
        player2.ball = ball.GetComponent<Ball>();
    }
    
    void ScoreControl()
    {
        if (ball == null || isGameEnded) return;
        bool isBallWithinFieldBounds = ball.transform.position.x > fieldLeftXBorder && ball.transform.position.x < fieldRightXBorder;

        if (ball.transform.position.z < player1.transform.position.z)
        {
            if (isBallWithinFieldBounds)
            {
                CmdUpdateScore(2);
            }
            else
            {
                CmdUpdateScore(1);
            }
            sM.PlayGoal();
            RespawnBall();
        }
        else if (ball.transform.position.z > player2.transform.position.z)
        {
            if (isBallWithinFieldBounds)
            {
                Debug.Log("loss p2");
                CmdUpdateScore(1);
            }
            else
            {
                CmdUpdateScore(2);
            }
            sM.PlayGoal();
            RespawnBall();
        }
    }
    
    public void StopGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateScore(int playerId)
    {
        if (playerId == 1)
        {
            player1.score++;
        }
        else if (playerId == 2)
        {
            player2.score++;
        }
        RpcUpdateScore(player1.score, player2.score);
    }

    [ClientRpc]
    public void RpcUpdateScore(int score1, int score2)
    {
        player1.score = score1;
        player2.score = score2;
    }

    [ClientRpc]
    void RpcUpdateGames(int games1, int games2)
    {
        player1.games = games1;
        player2.games = games2;
    }

    [ClientRpc]
    void RpcUpdateSets(int sets1, int sets2)
    {
        player1.sets = sets1;
        player2.sets = sets2;
    }
}
