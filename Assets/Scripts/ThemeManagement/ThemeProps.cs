using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeProps : MonoBehaviour {

    [Tooltip("Corresponding theme")]
    [SerializeField]
    ThemeManager.ThemeType theme;


    private void Awake()
    {
        this.AdaptToTheme(ThemeManager.getCurrentTheme());
    }

    // Use this for initialization
    void Start()
    {
        this.AdaptToTheme(ThemeManager.getCurrentTheme());
        
    }

    public void AdaptToTheme(ThemeManager.ThemeType t)
    {
        
        this.gameObject.SetActive(t == theme);
        
    }

}
