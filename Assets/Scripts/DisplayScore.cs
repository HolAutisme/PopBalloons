using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour {

    public GameObject scoreBorder;
    public Text score;
    private string levelName;
    private List<GameObject> levelsBtn;

	// Use this for initialization
	void Start () {

        levelsBtn = new List<GameObject>();
        foreach(Button btn in gameObject.GetComponentsInChildren<Button>())
        {
            levelsBtn.Add(btn.gameObject);
        }
        LoadProfileLevels();
    }

    public void LoadProfileLevels()
    {
        foreach (GameObject btn in levelsBtn)
        {
            levelName = btn.name;
            foreach (Level level in GameControl.control.currentProfile.levelsInfo)
            {
                if (level.name == levelName && level.score > 0)
                {
                    btn.transform.GetChild(1).gameObject.SetActive(true);
                    btn.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = level.score.ToString();
                }
            }
        }
    }
}
