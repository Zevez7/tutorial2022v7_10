using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;


public class XrButtonInteractable : XRSimpleInteractable
{
    [SerializeField] Image buttonImage;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color pressedColor;

    [SerializeField] private Color selectedColor;


    private bool isPressed;


    void Start()
    {
        resetColor();
    }

    // Update is called once per frame

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        isPressed = false;
        buttonImage.color = highlightColor;
        Debug.Log("Hover Entered");
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (!isPressed)
        {
            buttonImage.color = normalColor;

        }
        Debug.Log("Hover Exited");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        isPressed = true;
        buttonImage.color = selectedColor;
        Debug.Log("Select Entered");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // buttonImage.color = normalColor;
        buttonImage.color = selectedColor;
        Debug.Log("Select Exited");
    }


    public void resetColor()
    {
        buttonImage.color = normalColor;
    }
}
