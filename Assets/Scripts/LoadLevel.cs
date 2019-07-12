using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public int balloonToDestroy=0;
    public GameObject FadingCanvas;
    public GameObject FadingCanvasEnd;
    public TextMesh levelName;
    public GameObject winScreen;
    private GameObject cloneBonus;
    public GameObject balloonBonus;
    public Text winScore;
    public bool levelLoaded;
    public bool bonusOnce;
    public static Vector3 pos;

    public delegate void NextLevelRequested();
    public delegate void RequestSceneLoad(string scene);
    public delegate void LevelEnd();
    public static event LevelEnd OnLevelEnd;

    private void Start()
    {
        Participant.OnSceneRequested += NextLevelButton;
        bonusOnce = true;
        pos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.5f);
        FadingCanvas.GetComponentInChildren<Image>().GetComponent<Animator>().SetTrigger("FadeIn");
        FadingCanvasEnd.GetComponentInChildren<Image>().GetComponent<Animator>().SetTrigger("FadeIn");
        StartCoroutine(FadeTextToZeroAlpha(2.5f, levelName));
        levelLoaded = false;
    }

    public IEnumerator FadeTextToZeroAlpha(float time, TextMesh  text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
            yield return null;
        }
    }


    public void NextLevelButton(int index)
    {
        SceneManager.LoadScene(index);
    }

    private void OnDestroy()
    {
        Participant.OnSceneRequested -= NextLevelButton;
        Participant.OnNextLevelRequested -= nextLevelAuto;
    }

    private void nextLevelAuto()
    {
        StartCoroutine(LoadNextLevelAuto());
    }

    public void NextLevelButton(string levelName)
    {
        //La première fois qu'on passe le casque à l'enfant.
        if (!HeadSetAdjustmentManager.alreadySetUp && (levelName.StartsWith("Level") || levelName == "Tutorial"))
        {
                HeadSetAdjustmentManager.nextScene = levelName;
                levelName = "ChildHeadSetAdjustment";
        }
        
        SceneManager.LoadScene(levelName);
    }


    private void Update()
    {
        if(balloonToDestroy >= 0)
        {

            if (CircleSpawner.balloonDestroyed == balloonToDestroy && bonusOnce)
            {
                //StartCoroutine(Wait());
                //int rand = Random.Range(0, balloonBonus.Count);
                if (SharingManager.getLocalPlayer() != null)
                {

                    SharingManager.getLocalPlayer().CmdInstantiateGameObject(balloonBonus, pos, Quaternion.identity);
                    bonusOnce = false;
                    if (!levelLoaded)
                    {
                        TimerManager.levelEnd();
                        if(OnLevelEnd != null)
                        {
                            OnLevelEnd.Invoke();
                            Participant.OnNextLevelRequested += nextLevelAuto;
                            Invoke("saveData", 5);
                        }
                        else
                        {
                            StartCoroutine(LoadNextLevelAuto());
                        }
                    }
    
                }
            
            
            }

        }


    }

    public void LevelEndRequest()
    {
        TimerManager.levelEnd();
        if (OnLevelEnd != null)
        {
            OnLevelEnd.Invoke();
        }
    }
    private void saveData()
    {
        if (GameControl.control != null)
            GameControl.control.Save(SceneManager.GetActiveScene().name, ScoreManager.Instance.score);
        if (DataManager.instance != null)
            DataManager.instance.SaveDatas(SceneManager.GetActiveScene().name, ScoreManager.Instance.score);
    }

     IEnumerator Wait()
    {
       yield return new WaitForSeconds(1);
    }

    IEnumerator LoadNextLevelAuto()
    {
        levelLoaded = true;
        //yield return new WaitForSeconds(10);
        // Disable Game Manager
        if (gameObject.GetComponent<CircleSpawner>() != null)
            gameObject.GetComponent<CircleSpawner>().enabled = false;

        winScore.text = ScoreManager.Instance.score.ToString();
        winScreen.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        FadingCanvasEnd.GetComponentInChildren<Image>().GetComponent<Animator>().SetTrigger("FadeOut");

        yield return new WaitForSeconds(3.0f);

        if (SceneManager.GetActiveScene().name == "Level4") SceneManager.LoadScene(2);
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
        yield break;
    }


}
