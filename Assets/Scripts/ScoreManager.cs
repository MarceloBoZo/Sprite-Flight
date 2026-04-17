using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private const int MAX_SCORES = 5;
    private const string SCORE_KEY = "HighScore_";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveScore(int score)
    {
        List<int> scores = LoadAllScores();
        scores.Add(score);
        scores.Sort((a, b) => b.CompareTo(a)); // highest to lowest

        for (int i = 0; i < MAX_SCORES; i++)
        {
            int value = i < scores.Count ? scores[i] : 0;
            PlayerPrefs.SetInt(SCORE_KEY + i, value);
        }

        PlayerPrefs.Save();
    }

    public List<int> LoadAllScores()
    {
        List<int> scores = new List<int>();
        for (int i = 0; i < MAX_SCORES; i++)
        {
            int score = PlayerPrefs.GetInt(SCORE_KEY + i, 0);
            if (score > 0) scores.Add(score);
        }
        return scores;
    }

    public void LoadSceneDelayed(string sceneName, float delay)
    {
        StartCoroutine(LoadSceneAfterDelay(sceneName, delay));
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}