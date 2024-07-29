using System;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XrAudioManager : MonoBehaviour
{

    [Header("Grab Interactables")]
    [SerializeField] XRGrabInteractable[] grabInteractables;

    [SerializeField] AudioSource grabSound;
    [SerializeField] AudioClip grabClip;
    [SerializeField] AudioClip keyClip;

    [SerializeField] AudioSource activatedSound;

    [SerializeField] AudioClip grabActivatedClip;

    [SerializeField] AudioClip wandActivatedClip;
    [Header("Drawer Interactable")]
    [SerializeField] DrawerInteractable2 drawer;

    [SerializeField] AudioSource drawerSound;
    [SerializeField] AudioClip drawerMoveClip;

    [Header("The Wall")]

    [SerializeField] TheWall wall;
    [SerializeField] AudioSource wallSound;

    [SerializeField] AudioClip destroyWallClip;
    [SerializeField] private AudioClip fallBackClip;
    private const string FallBackClip_Name = "fallBackClip";

    private void OnEnable()
    {
        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create(FallBackClip_Name, 1, 1, 1000, true);
        }

        SetGrabbables();

        if (drawer != null)
        {
            SetDrawerInteractable();
        }
        if (wall != null)
        {
            SetWall();
        }
    }

    private void SetGrabbables()
    {
        grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);
        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnterGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitGrabbable);
            grabInteractables[i].activated.AddListener(OnActivatedGrabbable);

        }
    }

    private void SetDrawerInteractable()
    {
        // Get the drawer sound
        drawerSound = drawer.transform.AddComponent<AudioSource>();
        // Get the drawer move clip from the drawer object
        drawerMoveClip = drawer.GetDrawerMoveClip;
        // If the drawer move clip is null, set it to the fall back clip
        CheckClip(drawerMoveClip);
        // setting the properties of the drawer sound
        drawerSound.clip = drawerMoveClip;
        drawerSound.loop = true;
        // Add the event listeners
        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);
    }

    private void SetWall()
    {
        destroyWallClip = wall.GetDestroyClip;
        CheckClip(destroyWallClip);
        wall.OnDestroy.AddListener(OnDestroyWall);
    }

    private void CheckClip(AudioClip clip)
    {
        if (clip == null)
        {
            clip = fallBackClip;

        }
    }

    private void OnDrawerStop(SelectExitEventArgs arg0)
    {
        drawerSound.Stop();
    }

    private void OnDrawerMove(SelectEnterEventArgs arg0)
    {
        drawerSound.Play();
    }

    private void OnActivatedGrabbable(ActivateEventArgs arg0)
    {
        GameObject tempGameObject = arg0.interactableObject.transform.gameObject;
        if (tempGameObject.GetComponent<WandControl>() != null)
        {
            activatedSound.clip = wandActivatedClip;
        }
        else
        {
            activatedSound.clip = grabActivatedClip;
        }
        activatedSound.Play();
    }

    private void OnSelectExitGrabbable(SelectExitEventArgs arg0)
    {
        grabSound.clip = grabClip;
        grabSound.Play();

    }

    private void OnSelectEnterGrabbable(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.CompareTag("Key"))
        {
            grabSound.clip = keyClip;
        }
        else
        {
            grabSound.clip = grabClip;
        }
        grabSound.Play();
    }

    private void OnDestroyWall()
    {
        if (wallSound != null)
        {
            wallSound.Play();
        }
    }

    private void OnDisable()
    {
        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }
    }
}
