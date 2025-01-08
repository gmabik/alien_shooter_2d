using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;
using UnityEditor;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    #region singleton
    public static LobbyManager Instance;

    private void Awake()
    {
        if(Instance != null) Destroy(Instance);
        Instance = this;
    }
    #endregion

    #region lobby
    [Header("Panels")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject currentRoomPanel;
    [SerializeField] private GameObject gameUI;

    [Header("Lobby Creation")]
    [SerializeField] private TMP_InputField createRoomNameInput;
    [SerializeField] private TMP_Text createButtonText;

    [Header("Lobby Joining")]
    [SerializeField] private TMP_InputField JoinRoomNameInput;
    [SerializeField] private TMP_Text JoinButtonText;

    [Header("CurrentRoom")]
    [SerializeField] private TMP_Text currentRoomName;
    [SerializeField] private GameObject playButton;

    [Space(10)]
    [SerializeField]private NetworkRunner runner;
    [SerializeField] private Manager gameManager;

    private void Start()
    {
        lobbyPanel.SetActive(true);
        currentRoomPanel.SetActive(false);
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }
        runner.ProvideInput = true;
        runner.AddCallbacks(this);
    }

    private void StartLobby(string roomName)
    {
        StartGame(roomName, GameMode.Host);
    }

    private async void StartGame(string roomName, GameMode gameMode)
    {
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = roomName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            OnJoinedRoom(runner.SessionInfo);
        }
    }

    public void OnClickCreateRoom()
    {
        if (createRoomNameInput.text.Length >= 3)
        {
            StartLobby(createRoomNameInput.text);
        }
        else
        {
            StartCoroutine(WrongName());
        }
    }

    private IEnumerator WrongName()
    {
        createButtonText.text = "Wrong name";
        yield return new WaitForSeconds(1f);
        createButtonText.text = "Create";
    }

    public void OnJoinedRoom(SessionInfo session)
    {
        lobbyPanel.SetActive(false);
        currentRoomPanel.SetActive(true);
        currentRoomName.text = "Room: " + session.Name;
        //gameManager.gameObject.SetActive(false);
    }

    public void JoinRoom()
    {
        StartGame(JoinRoomNameInput.text, GameMode.Client);
    }

    public void LeaveRoom()
    {
        if (runner.SessionInfo != null)
        {
            runner.Shutdown();
            OnLeaveRoom();
        }
    }

    private void OnLeaveRoom()
    {
        lobbyPanel.SetActive(true);
        currentRoomPanel.SetActive(false);
    }

    public void LeaveLobby()
    {
        runner.Shutdown();
    }

    public void OnClickPlay()
    {
        OpenGameUI();
    }

    [Header("Score")]
    [SerializeField] private NetworkPrefabRef scoreManagerPrefab;

    public void OpenGameUI()
    {
        print("started opening game ui");
        if(runner.IsServer) Manager.instance.scoreManager = runner.Spawn(scoreManagerPrefab).gameObject.GetComponent<ScoreManager>();

        lobbyPanel.SetActive(false);
        currentRoomPanel.SetActive(false);
        gameUI.SetActive(true);
        gameManager.isGameStarted = true;
        print("opened game ui");
    }

    #endregion

    [SerializeField] private NetworkPrefabRef playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> Players = new();
    public TMP_Text hpText;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            networkPlayerObject.AssignInputAuthority(player);
            Players.Add(player, networkPlayerObject.GetComponent<NetworkObject>());
            if(Players.Count >= 2) OpenGameUI();
        }
        else OpenGameUI();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        if (Players.TryGetValue(player, out NetworkObject playerMovement))
        {
            Players.Remove(player);
            runner.Despawn(playerMovement);
        }
    }

    private Vector2 GetSpawnPosition()
    {
        return new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 0f));
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        NetworkInputData data = new NetworkInputData();

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        data.movementInput.x = horizontal;
        data.movementInput.y = vertical;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) 
    {
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
}
