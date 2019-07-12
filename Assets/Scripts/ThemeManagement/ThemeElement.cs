using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeElement : MonoBehaviour {


    List<ThemeProps> probs;

    private void Start()
    {
        ThemeManager.onThemeChanged += AdaptChildToTheme;
        Invoke("init", 2);
    }

    private void OnDestroy()
    {
        ThemeManager.onThemeChanged -= AdaptChildToTheme;
    }
    

    private void init()
    {
        this.AdaptChildToTheme(ThemeManager.getCurrentTheme());
    }

    List<ThemeProps> getProbs()
    {
        if(probs == null)
        {
            probs = new List<ThemeProps>();
            this.GetComponentsInChildren<ThemeProps>(true, probs);
        }
        return probs;
    }

    void AdaptChildToTheme(ThemeManager.ThemeType theme)
    {
        
        //Will enable or disable probs matching their theme.
        foreach(ThemeProps tp in getProbs())
        {
            tp.AdaptToTheme(theme);
        }
    }
}
