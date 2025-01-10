using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour, INetworkRunnerCallbacks
{
    #region singleton
    public static Manager instance;
    private void Awake()
    {
        if(instance != null) Destroy(instance);
        instance = this;
    }
    #endregion
    [Header("Score")]
    public int score;
    public TMP_Text highscoreText;
    public TMP_Text scoreText;
    public ScoreManager scoreManager;

    [Header("Enemies")]
    [SerializeField] private List<NetworkPrefabRef> enemyPrefabs;
    [SerializeField] private int enemyMaxCount;
    private List<NetworkObject> spawnedEnemies;

    [Header("Boss")]

    [SerializeField] private List<NetworkPrefabRef> bossPrefabs;
    private bool isBossSpawned;
    private int lastScoreWhenBossWasKilled;
    [SerializeField] private int scoreTreshholdToSpawnBoss;


    [SerializeField] private NetworkRunner runner;
    public bool isGameStarted;
    void Start()
    {
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }
        runner.ProvideInput = true;
        runner.AddCallbacks(this);
    }

    private IEnumerator FirstEnemySpawn()
    {
        while (!isGameStarted)
        {
            yield return new WaitForSeconds(0.1f);
            print("waiting for game start");
        }
        SpawnNewEnemy();
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreManager != null) scoreManager.RpcUpdateScore(score);
    }

    public void FixedUpdate()
    {
        if(runner.IsServer && spawnedEnemies == null)
        {
            spawnedEnemies = new List<NetworkObject>();
            if (runner.IsServer) StartCoroutine(FirstEnemySpawn());
        }

        if (!isGameStarted) return;

        if (runner.IsServer) AddScore(1);
        if (score >= (lastScoreWhenBossWasKilled + scoreTreshholdToSpawnBoss) && !isBossSpawned) StartCoroutine(SpawnBoss());
    }

    private void OnEnemyDeath(NetworkObject enemy)
    {
        if (!runner.IsServer) return;

        spawnedEnemies.Remove(enemy);
        enemy.GetComponent<EnemyScript>().enemyDeath -= OnEnemyDeath;
        if (enemy.GetComponent<EnemyScript>().isBoss)
        {
            AddScore(1000);
            isBossSpawned = false;
            lastScoreWhenBossWasKilled = score;
        }
        else AddScore(250);
        runner.Despawn(enemy);
        if(!isBossSpawned)StartCoroutine(RespawnEnemy());
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(2f);
        SpawnNewEnemy();
    }

    private void SpawnNewEnemy()
    {
        if (!runner.IsServer) return;

        while (spawnedEnemies.Count < enemyMaxCount && !isBossSpawned)
        {
            NetworkObject newEnemy = runner.Spawn(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)]);
            int a = Random.Range(0, 2);
            newEnemy.transform.position = new(a == 0? Random.Range(-20f, -15f) : Random.Range(15f, 20f), Random.Range(10f, 12f), 0);
            spawnedEnemies.Add(newEnemy);
            newEnemy.GetComponent<EnemyScript>().enemyDeath += OnEnemyDeath;
        }
    }

    private IEnumerator SpawnBoss()
    {
        if (!runner.IsServer) yield return null;

        isBossSpawned = true;
        foreach(NetworkObject enemy in spawnedEnemies)
        {
            runner.Despawn(enemy);
        }
        spawnedEnemies.Clear();
        yield return new WaitForSeconds(1f);

        NetworkObject newBoss = runner.Spawn(bossPrefabs[Random.Range(0, bossPrefabs.Count)]);
        newBoss.transform.position = new(0, 12f, 0);
        newBoss.GetComponent<EnemyScript>().enemyDeath += OnEnemyDeath;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
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
    public void OnSceneLoadDone(NetworkRunner runner) 
    {
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
}
