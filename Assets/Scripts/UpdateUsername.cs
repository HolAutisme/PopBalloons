using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUsername : MonoBehaviour {

    public static UpdateUsername usernameUI;

    private void Awake()
    {
        if (usernameUI == null)
            usernameUI = this;
        else if (usernameUI != null)
            Destroy(gameObject);
    }

    public void SetUsername(string username)
    {
        if (gameObject.activeInHierarchy)
            gameObject.GetComponent<Text>().text = username;
    }

    public void SetUsernameCurrentProfile()
    {
        if (gameObject.activeInHierarchy)
            gameObject.GetComponent<Text>().text = GameControl.control.currentProfile.username;
    }
}
