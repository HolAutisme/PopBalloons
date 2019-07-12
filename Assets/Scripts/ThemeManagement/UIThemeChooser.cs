using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIThemeChooser : MonoBehaviour
{
    private void setTheme(ThemeManager.ThemeType t)
    {
        if(ThemeManager.getInstance() != null)
            ThemeManager.getInstance().CmdChangeTheme(t);
    }

    public void setChildRoom()
    {
        this.setTheme(ThemeManager.ThemeType.CHILDROOM);
    }

    public void setSpaaaace()
    {
        this.setTheme(ThemeManager.ThemeType.SPACE);
    }
    public void setRoaaar()
    {
        this.setTheme(ThemeManager.ThemeType.DINOSAUR);
    }
}
