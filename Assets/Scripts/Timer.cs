using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    /// <summary>
    /// TEMPORARY timer for limited time demo
    /// </summary>
    public float timer;
    public GameObject gameManager;
    public GameObject winScreen;
    public Text winScore;

    void Update()
    {
        timer = Mathf.Max(0, timer - Time.deltaTime);
        var timeSpan = System.TimeSpan.FromSeconds(timer);
        GetComponent<TextMesh>().text = timeSpan.Minutes.ToString("00") + ":" +
                        timeSpan.Seconds.ToString("00");
        if (timer <= 21f)
            GetComponent<TextMesh>().color = Color.red;
        if (timer <= 0f)
        {
            StartCoroutine(LoadNextLevel());
            Debug.Log(timer);
        }
    }

    IEnumerator LoadNextLevel()
    {
        gameManager.SetActive(false);
        //Score has change, this part is obselete
        winScore.text = ScoreManager.Instance.ToString();


        winScreen.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        if (SceneManager.GetActiveScene().name == "Virtuality") SceneManager.LoadScene(2);

    }
}