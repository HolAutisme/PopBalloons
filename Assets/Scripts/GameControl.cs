using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    /// <summary>
    /// This method serialize game and player's datas.
    /// Saving score, levels, and player personal informations.
    /// </summary>
    public static GameControl control;
    public PlayerProfile currentProfile;
    public List<PlayerProfile> playerProfileList;
    public GameObject popUpWarning;
    private PlayerProfileCollection playerProfileCollection;
    private string saveFile;


    private void Awake()
    {
        if (control != null)
        {
            Destroy(control.gameObject);
        }
        DontDestroyOnLoad(gameObject);
        control = this;
    }



    private void Start()
    {
        //Init global variables
        currentProfile = null;
        playerProfileList = new List<PlayerProfile>();
        playerProfileCollection = new PlayerProfileCollection(playerProfileList);
        saveFile = Application.persistentDataPath + "/Saves/gameData.json";
        //Load all profile list on start
        LoadAllProfiles();
    }

    //Load all players profiles from json file
    public void LoadAllProfiles()
    {
        string json = null;

        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            return;

        if (File.Exists(saveFile))
        {
            json = File.ReadAllText(saveFile);
            playerProfileCollection = JsonUtility.FromJson<PlayerProfileCollection>(json);
            playerProfileList = playerProfileCollection.playerProfileList;
        }

        //Load latest currentProfile active
        foreach (PlayerProfile playerProfile in playerProfileList)
        {
            if (playerProfile.active)
                currentProfile = playerProfile;
        }
    }

    //Create a new profile with all data
    public void CreateProfile(Text username)
    {
        PlayerProfile playerProfile = InitPlayerProfile(username.text);

        //Check if any similar profile has already been created
        if (playerProfileList.Any(item => item.username == username.text))
            return;

        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");

        playerProfileList.Add(playerProfile);
        string json = JsonUtility.ToJson(playerProfileCollection, true);
        File.WriteAllText(saveFile, json);
    }

    //Edit an existing profile in the saved json file
    public void EditProfileUsername(Text username)
    {
        //Check if save directory exist
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            return;

        //Update username in profile players list
        foreach (PlayerProfile playerProfile in playerProfileList)
        {
            if (playerProfile.username == currentProfile.username)
                playerProfile.username = username.text;
        }

        //CurrentProfileUpdate
        if (currentProfile != null)
            currentProfile.username = username.text;

        string json = JsonUtility.ToJson(playerProfileCollection, true);
        File.WriteAllText(saveFile, json);
    }


    //Delete an existing profile in the saved json file
    public void DeleteProfile(Text username)
    {
        
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
        {
            
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
            
        }
        playerProfileList.Remove(currentProfile);

        //CurrentProfileUpdate
        currentProfile = null;
        string json = JsonUtility.ToJson(playerProfileCollection, true);
        File.WriteAllText(saveFile, json);
        popUpWarning.SetActive(false);

    }
    public void ValidateDelete()
    {
        
        popUpWarning.SetActive(true);
        
    }
    public void CancelDelete()
    {
        popUpWarning.SetActive(false);
    }

    //Save current player profile data in json
    public void Save(string levelName, int score)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");

        foreach (Level level in currentProfile.levelsInfo)
        {
            if (levelName == level.name)
                level.score = score;
        }

        for (int i = 0; i < playerProfileList.Count; i++)
        {
            if (playerProfileList[i].username == currentProfile.username)
                playerProfileList[i] = currentProfile;
        }

        string json = JsonUtility.ToJson(playerProfileCollection, true);
        File.WriteAllText(saveFile, json);
    }

    // Get the active playing profile
    public PlayerProfile GetCurrentProfile()
    {
        return currentProfile;
    }

    //Set current playing profile to active
    public void SetCurrentProfile(Text username)
    {
        foreach (PlayerProfile playerProfile in GameControl.control.playerProfileList)
        {
            playerProfile.active = false;
            if (playerProfile.username == username.text)
            {
                currentProfile = playerProfile;
                playerProfile.active = true;
            }
        }

        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");

        string json = JsonUtility.ToJson(playerProfileCollection, true);
        File.WriteAllText(saveFile, json);
    }

    //Initialize default player profile datas
    private PlayerProfile InitPlayerProfile(string username)
    {
        PlayerProfile playerProfile = new PlayerProfile();
        List<Level> levelsInfo = new List<Level>();

        for (int lvl = 1; lvl <= 4; lvl++)
        {
            Level level = new Level();
            level.name = "Level" + lvl.ToString();
            level.score = 0;
            levelsInfo.Add(level);
        }
        playerProfile.username = username;
        playerProfile.levelsInfo = levelsInfo;
        return playerProfile;
    }
}

[Serializable]
public class PlayerProfileCollection
{
    public List<PlayerProfile> playerProfileList;

    public PlayerProfileCollection(List<PlayerProfile> _playerProfileList)
    {
        playerProfileList = _playerProfileList;
    }
}

[Serializable]
public class PlayerProfile
{
    public bool active = false;
    public string username = null;
    public List<Level> levelsInfo = new List<Level>();
}

[Serializable]
public class Level
{
    public string name = null;
    public int score = 0;
}
