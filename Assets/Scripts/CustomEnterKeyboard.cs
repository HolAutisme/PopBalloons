using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomEnterKeyboard : MonoBehaviour {

    private bool create;
    public Text inputText;

    

    public void SetCreateOrEdit(bool isCreateOrEdit)
    {
        create = isCreateOrEdit;
    }

    public void EnterKeyPressed()
    {
        if (create)
            GameControl.control.CreateProfile(inputText);
        else if (!create)
            GameControl.control.EditProfileUsername(inputText);
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        
    }
}
