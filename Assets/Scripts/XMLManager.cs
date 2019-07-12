using System.Collections;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class XMLManager : MonoBehaviour {
    public static XMLManager ins;
	// Use this for initialization
	void Start () {
        ins = this;
	}

    public ItemDatabase itemDB;

}
[System.Serializable]
public class ItemEntry{
    public string username;
    public Vector3[] headPosition;
    public Vector3 balloonPosition;
    public string level;
    public Vector3 handPosition;
    public float timeBalloonApparition;
    public float whenBallonEnterFOV;
    public int scoreByLevel;
    public int balloonDestroyed;
    public float lifeTimeBalloon;
}

[System.Serializable]
public class ItemDatabase
{
    public List<ItemEntry> list = new List<ItemEntry>();
}
