using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ThemeManager : NetworkBehaviour {


    public enum ThemeType {CHILDROOM, SPACE, DINOSAUR}

    private static ThemeManager Instance;


    [SyncVar]
    public ThemeType currentTheme = ThemeType.CHILDROOM;


    public static ThemeManager getInstance()
    {
        return Instance;
    }


    public delegate void themeChanged(ThemeType p);
    public static event themeChanged onThemeChanged;

    public static ThemeType getCurrentTheme()
    {
        if(Instance != null)
            return Instance.currentTheme;
        return ThemeType.CHILDROOM;
    }
    private void Awake()
    {
        if(Instance != null)
        {
            DestroyImmediate(this);
        }
        else
        {
            Instance = this;
        }
    }

    [Command]
    public void CmdChangeTheme(ThemeType t)
    {
        currentTheme = t;
        RpcChangeTheme(t);
    }

    [ClientRpc]
    public void RpcChangeTheme(ThemeType t)
    {
        if (onThemeChanged != null)
        {
            onThemeChanged(t);
        }
    }

}
