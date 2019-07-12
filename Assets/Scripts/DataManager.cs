using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


#if !NETFX_CORE
using System.Threading;
#else
using Windows.System.Threading;
using Windows.Foundation;
#endif

public class DataManager : MonoBehaviour
{

    /// <summary>
    /// This script serialize several informations gathered from user's 
    /// behaviour and play results.
    /// </summary>
    public static DataManager instance;
    private string saveFile;
    private List<Datas> datasList;
    private DatasCollection datasCollection;
    private Datas datas;
    private string currentSaveTime;

#if !NETFX_CORE
    Thread DataManagement;
#endif
    private void Awake()
    {

        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
            Destroy(gameObject);
        currentSaveTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        if (GameControl.control.GetCurrentProfile().username != null)
            saveFile = Application.persistentDataPath + string.Format("/Datas/{0}/{1}.json", GameControl.control.GetCurrentProfile().username,currentSaveTime);
        currentSaveTime = DateTime.Now.ToString("yyyy/MM/dd/ HH:mm");
        LoadDatas();
        InitDatas();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += handleLevelEnd;
    }

    private void handleLevelEnd(Scene sc, LoadSceneMode mode)
    {
       if(!sc.name.StartsWith("Level"))
        {
            instance = null;
            Destroy(this);
        }
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= handleLevelEnd;
    }


    // Gather all the datas about balloons events
    public void AddBalloonsDatas(BalloonDatas datas)
    {
        if(datasList != null)
        {
            int datasIndex = datasList.Count - 1;
            int levelIndex = datasList[datasIndex].listLevelDatas.FindIndex(idx => idx.name == SceneManager.GetActiveScene().name);
            datasList[datasIndex].listLevelDatas[levelIndex].listBalloonDatas.Add(datas);
        }
    }

    // Gather all the datas about user while playing
    public void AddUsersDatas(UserDatas datas)
    {

        int datasIndex = datasList.Count - 1;
        int levelIndex = datasList[datasIndex].listLevelDatas.FindIndex(idx => idx.name == SceneManager.GetActiveScene().name);

        datasList[datasIndex].listLevelDatas[levelIndex].userDatas.Add(datas);
    }

    // Save all datas gathered during current level before loading the next one.
    public void SaveDatas(string level, int score)
    {

        if (!Directory.Exists(Application.persistentDataPath + "/Datas"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Datas");

        if (!Directory.Exists(Application.persistentDataPath + "/Datas"+ string.Format("/{0}", GameControl.control.GetCurrentProfile().username)))
            Directory.CreateDirectory(Application.persistentDataPath + "/Datas" + string.Format("/{0}", GameControl.control.GetCurrentProfile().username));

        int datasIndex = datasList.Count - 1;
        int levelIndex = datasList[datasIndex].listLevelDatas.FindIndex(idx => idx.name == level);
        
        datasList[datasIndex].listLevelDatas[levelIndex].name = level;
        datasList[datasIndex].listLevelDatas[levelIndex].score = score;
    
        string json = JsonUtility.ToJson(datasCollection, true);

#if !NETFX_CORE
        DataManagement = new Thread( () => File.WriteAllText(saveFile, json));
        DataManagement.Start();
#else
        IAsyncAction asyncAction = ThreadPool.RunAsync((workItem)=>File.WriteAllText(saveFile, json));
#endif
    }

    //Load all players profiles from json file
    public void LoadDatas()
    {
        string json = null;
        datasList = new List<Datas>();

        if (!Directory.Exists(Application.persistentDataPath + string.Format("/Datas/{0}", GameControl.control.GetCurrentProfile().username)))
            return;
        
        if (File.Exists(saveFile))
        {
            json = File.ReadAllText(saveFile);
            datasCollection = JsonUtility.FromJson<DatasCollection>(json);
            datasList = datasCollection.datasList;
        }
    }

    // Initialization of serialization container and objects
    private void InitDatas()
    {
        datas = new Datas();

        datas.dateTime = currentSaveTime;
        for (int i = 1; i <= 4; i++)
        {
            datas.listLevelDatas.Add(new LevelDatas());
            datas.listLevelDatas[i - 1].name = "Level" + i.ToString();
            datas.listLevelDatas[i - 1].score = 0;
            datas.listLevelDatas[i - 1].userDatas = new List<UserDatas>();
            datas.listLevelDatas[i - 1].listBalloonDatas = new List<BalloonDatas>();
            //datas.listLevelDatas[i - 1].userDatas.handPos = new List<Vector3>();
        }
        datasList.Add(datas);
        datasCollection = new DatasCollection(datasList);
    }
}

// Datas serialization objects
[Serializable]
public class UserDatas
{
    public Vector3 headPos;
    public float headRotationY;
    public float BPM;
    public float timeStamp;
    //public List<Vector3> headRot;
    //public List<Vector3> handPos;
}

[Serializable]
public class BalloonDatas
{
    // Temps du balloon
    public float timeOfSpawn;
    public float timeOfDestroy;
    public float lifeTime;

    //Gain de point ou condition de reussite / echec
    public float balloonPointGain;
    public bool balloonWasDestroyByUser;
    public bool balloonTimout;

    // distance parcourue depuis l'apparition du ballon.
    public float distance;

    //position du balloon
    public Vector3 balloonInitialPosition;

}

[Serializable]
public class LevelDatas
{
    public string name;
    public int score;
    public List<UserDatas> userDatas = new List<UserDatas>();
    public List<BalloonDatas> listBalloonDatas = new List<BalloonDatas>();
}

[Serializable]
public class Datas
{
    public string dateTime;
    public List<LevelDatas> listLevelDatas = new List<LevelDatas>();
}

[Serializable]
public class DatasCollection
{
    [SerializeField]
    public List<Datas> datasList;

    public DatasCollection(List<Datas> _datasList)
    {
        datasList = _datasList;
    }
}