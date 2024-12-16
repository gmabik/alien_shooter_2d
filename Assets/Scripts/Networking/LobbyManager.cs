using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class LobbyManager : Fusion.Behaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject currentRoomPanel;

    [Header("Lobby Creation")]
    [SerializeField] private TMP_InputField createRoomNameInput;
    [SerializeField] private TMP_Text createButtonText;

    [Header("Lobby Joining")]
    [SerializeField] private TMP_InputField JoinRoomNameInput;
    [SerializeField] private TMP_Text JoinButtonText;

    [Header("CurrentRoom")]
    [SerializeField] private TMP_Text currentRoomName;
    [SerializeField] private GameObject playButton;

    private NetworkRunner networkRunner;

    private void Start()
    {
        lobbyPanel.SetActive(true);
        currentRoomPanel.SetActive(false);
        networkRunner = gameObject.AddComponent<NetworkRunner>();
    }

    private async void StartLobby(string roomName)
    {
        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            OnJoinedRoom(networkRunner.SessionInfo);
        }
        else
        {
            Debug.LogError("Failed to start NetworkRunner: " + result.ShutdownReason);
        }
    }

    private void Update()
    {
        //if (networkRunner.IsServer)
        //{
        //    playButton.SetActive(true);
        //}
        //else
        //{
        //    playButton.SetActive(false);
        //}
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
    }

    public async void JoinRoom()
    {
        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, JoinRoomNameInput.text);

        if (result.Ok)
        {
            OnJoinedRoom(networkRunner.SessionInfo);
        }
        else
        {
            StartCoroutine (WrongName());
        }
    }

    public void LeaveRoom()
    {
        if (networkRunner.SessionInfo != null)
        {
            networkRunner.Shutdown();
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
        networkRunner.Shutdown();
        SceneManager.LoadScene("ConnectToServer");
    }

    public void OnClickPlay()
    {
        if (networkRunner.IsServer)
        {
            SceneRef gameScene = networkRunner.GetSceneRef("Game");
            networkRunner.SceneManager.LoadScene(gameScene, new NetworkLoadSceneParameters());
        }
    }
}
