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
    public List<GameObject> spawnedEnemies;

    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);
        highscoreText.text = "Highscore: " + highscore;

        spawnedEnemies = new List<GameObject>();
        while(spawnedEnemies.Count < enemyMaxCount)
        {
            SpawnNewEnemy();
        }
    }

    void FixedUpdate()
    {
        AddScore(1);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
        ManageHighscore();
    }

    private void ManageHighscore()
    {
        if (score < highscore) return;
        highscore = score;
        highscoreText.text = "Highscore: " + highscore;
        PlayerPrefs.SetInt("highscore", highscore);
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        Destroy(enemy);
        AddScore(250);
        StartCoroutine(RespawnEnemy());
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(2f);
        SpawnNewEnemy();
    }

    private void SpawnNewEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)]);
        newEnemy.transform.position = new(Random.Range(-5f, 5f), Random.Range(2.75f, 8.75f), 0);
        spawnedEnemies.Add(newEnemy);
        newEnemy.GetComponent<EnemyScript>().enemyDeath += OnEnemyDeath;
    }
}
