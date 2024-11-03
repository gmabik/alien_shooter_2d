using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager : MonoBehaviour
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
    private int score;
    [SerializeField] private TMP_Text scoreText;
    private int highscore;
    [SerializeField] private TMP_Text highscoreText;

    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private int enemyMaxCount;
    [SerializeField] private List<GameObject> spawnedEnemies;

    [Header("Boss")]

    [SerializeField] private List<GameObject> bossPrefabs;
    [SerializeField] private bool isBossSpawned;
    private int lastScoreWhenBossWasKilled;
    [SerializeField] private int scoreTreshholdToSpawnBoss;


    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);
        highscoreText.text = "" + highscore;

        spawnedEnemies = new List<GameObject>();
        SpawnNewEnemy();
    }

    void FixedUpdate()
    {
        AddScore(1);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "" + score;
        ManageHighscore();

        if(score >= (lastScoreWhenBossWasKilled + scoreTreshholdToSpawnBoss) && !isBossSpawned) StartCoroutine(SpawnBoss());
    }

    private void ManageHighscore()
    {
        if (score < highscore) return;
        highscore = score;
        highscoreText.text = "" + highscore;
        PlayerPrefs.SetInt("highscore", highscore);
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        enemy.GetComponent<EnemyScript>().enemyDeath -= OnEnemyDeath;
        if (enemy.GetComponent<EnemyScript>().isBoss)
        {
            AddScore(1000);
            isBossSpawned = false;
            lastScoreWhenBossWasKilled = score;
        }
        else AddScore(250);
        Destroy(enemy);
        if(!isBossSpawned)StartCoroutine(RespawnEnemy());
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(2f);
        SpawnNewEnemy();
    }

    private void SpawnNewEnemy()
    {
        while (spawnedEnemies.Count < enemyMaxCount && !isBossSpawned)
        {
            GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)]);
            int a = Random.Range(0, 2);
            newEnemy.transform.position = new(a == 0? Random.Range(-10f, -5f) : Random.Range(5f, 10f), Random.Range(2.75f, 8.75f), 0);
            spawnedEnemies.Add(newEnemy);
            newEnemy.GetComponent<EnemyScript>().enemyDeath += OnEnemyDeath;
        }
    }

    private IEnumerator SpawnBoss()
    {
        isBossSpawned = true;
        foreach(GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
        spawnedEnemies.Clear();
        yield return new WaitForSeconds(1f);

        GameObject newBoss = Instantiate(bossPrefabs[Random.Range(0, bossPrefabs.Count)]);
        newBoss.transform.position = new(0, 5.75f, 0);
        newBoss.GetComponent<EnemyScript>().enemyDeath += OnEnemyDeath;
    }
}
