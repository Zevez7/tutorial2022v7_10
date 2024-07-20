using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleHingeInteractable : XRSimpleInteractable
{

    private Transform grabHand;

    protected virtual void Start()
    {

        // change the gameobject material to the color red
        // if (GetComponent<Renderer>() != null)
        // {
        GetComponent<Renderer>().material.color = Color.red;
        // }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (grabHand != null)
        {
            transform.LookAt(grabHand, transform.forward);
        }

    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        grabHand = args.interactorObject.transform;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        grabHand = null;
        // test
    }
}
