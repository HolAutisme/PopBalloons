using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckValidUsername : MonoBehaviour {

    public Button createBtn;
    
    //Check if new profile's username length is long enough (3 character).
    public void CheckCharNumber(Text input)
    {
        if (input.text.Length >= 3)
            createBtn.interactable = true;
        else
            createBtn.interactable = false;
    }
}
