using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoard : MonoBehaviour {


    public void TryMoveBoard()
    {
        if(ButtonManager.instance != null)
        {
            ButtonManager.instance.EnableDisplayer();
        }
    }
}
