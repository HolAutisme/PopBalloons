using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugKeyboard : MonoBehaviour {
    public GameObject keyboard;
	
    public void ActiveKeyboard()
    {
        keyboard.SetActive(true);
    }
}
