using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSlider : MonoBehaviour
{
    public Scrollbar Target;
    public Button TheOtherButton;
    public float Step = 0.1f;
    public bool upButton;
    [SerializeField]
    private Transform profileListContent;

    public void Increment()
    {
        if (Target == null || TheOtherButton == null) throw new Exception("Setup first!");
        Target.value = Mathf.Clamp(Target.value + Step, 0, 1);
        GetComponent<Button>().interactable = Target.value != 1;
        TheOtherButton.interactable = true;
    }

    public void Decrement()
    {
        if (Target == null || TheOtherButton == null) throw new Exception("Setup first!");
        Target.value = Mathf.Clamp(Target.value - Step, 0, 1);
        GetComponent<Button>().interactable = Target.value != 0;
        TheOtherButton.interactable = true;
    }

    public void Update()
    {
        if (profileListContent.childCount > 4)
        {
            if (upButton)
                GetComponent<Button>().interactable = (Target.value < 0.9);
             else
                GetComponent<Button>().interactable = (Target.value > 0.1);
        }
        else
        {
            GetComponent<Button>().interactable = false;
            TheOtherButton.interactable = false;
        }
    }
}