using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable2 : XRGrabInteractable
{

    public UnityEvent OnDrawerDetach;
    [SerializeField] Transform drawerTransform;

    [SerializeField] XRSocketInteractor keySocket;
    [SerializeField] XrPhysicsButtonInteractable physicsButton;
    public XrPhysicsButtonInteractable GetPhysicsButton => physicsButton;

    public XRSocketInteractor GetKeySocket => keySocket;

    // same as keyIndicatorLight
    [SerializeField] GameObject lightPointer1;

    [SerializeField] GameObject lightPointer2;
    [SerializeField] public bool isLocked;
    [SerializeField] public bool isDetachable;
    [SerializeField] public bool isDetached;


    private Transform parentTransform;
    private const string Default_Layer = "Default";
    private const string Grab_Layer = "Grab";
    private bool isGrabbed;

    private Vector3 limitPosition;

    [SerializeField]
    float drawerlimitz = 0.8f;

    [SerializeField]
    private Vector3 limitDistances = new Vector3(.02f, .02f, 0);

    [SerializeField] AudioClip drawerMoveClip;

    public AudioClip GetDrawerMoveClip => drawerMoveClip;

    [SerializeField] AudioClip socketedClip;
    private Rigidbody rb;

    public AudioClip GetSocketedClip => socketedClip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            keySocket.selectExited.AddListener(OnDrawerLocked);
        }
        parentTransform = transform.parent.transform;
        limitPosition = drawerTransform.localPosition;
        if (physicsButton != null)
        {
            physicsButton.OnBaseEnter.AddListener(OnIsDetachable);
            physicsButton.OnBaseExit.AddListener(OnIsNotDetachable);
        }
    }

    private void OnIsNotDetachable()
    {
        isDetachable = false;
    }
    private void OnIsDetachable()
    {
        isDetachable = true;
    }


    private void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        Debug.Log("Drawer Unlocked");
        if (lightPointer1 != null && lightPointer2 != null)
        {
            lightPointer1.SetActive(false);
            lightPointer2.SetActive(false);
        }
    }

    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        Debug.Log("Drawer Locked");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!isLocked)
        {
            transform.SetParent(parentTransform);
            isGrabbed = true;
        }
        else
        {
            ChangeLayerMask(Default_Layer);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (isDetached)
        {
            ChangeLayerMask(Grab_Layer);
            isGrabbed = false;
            transform.localPosition = drawerTransform.localPosition;
        }
        else
        {
            rb.isKinematic = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDetached)
        {
            if (isGrabbed && drawerTransform != null)
            {
                drawerTransform.localPosition = new Vector3(
                    drawerTransform.localPosition.x,
                    drawerTransform.localPosition.y,
                    transform.localPosition.z
                );
                CheckLimits();
            }
        }

    }

    private void CheckLimits()
    {
        // Add your implementation here
        if (
            transform.localPosition.x >= limitPosition.x + limitDistances.x
            || transform.localPosition.x <= limitPosition.x - limitDistances.x
        )
        {
            ChangeLayerMask(Default_Layer);
        }
        else if (
            transform.localPosition.y >= limitPosition.y + limitDistances.y
            || transform.localPosition.y <= limitPosition.y - limitDistances.y
        )
        {
            ChangeLayerMask(Default_Layer);
        }
        else if (drawerTransform.localPosition.z <= limitPosition.z - limitDistances.z)
        {
            isGrabbed = false;
            drawerTransform.localPosition = limitPosition;
            ChangeLayerMask(Default_Layer);
        }
        else if (drawerTransform.localPosition.z >= drawerlimitz + limitDistances.z)
        {
            if (!isDetachable)
            {
                isGrabbed = false;
                drawerTransform.localPosition = new Vector3(
                    drawerTransform.localPosition.x,
                    drawerTransform.localPosition.y,
                    drawerlimitz
                );
                ChangeLayerMask(Default_Layer);
            }
            else
            {
                DetachDrawer();
            }
        }

    }
    private void DetachDrawer()
    {
        isDetached = true;
        drawerTransform.SetParent(this.transform);
        OnDrawerDetach?.Invoke();
    }
    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
