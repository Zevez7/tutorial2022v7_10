using System;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class XrAudioManager : MonoBehaviour
{
    [Header("Progress Control")]

    [SerializeField] ProgressControl progressControl;
    [SerializeField] AudioSource progressSound;
    [SerializeField] AudioClip startGameClip;
    [SerializeField] AudioClip challengeCompleteClip;

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

    XRSocketInteractor drawerSocket;
    XrPhysicsButtonInteractable drawerPhysicsButton;

    private bool isDetached;

    AudioSource drawerSound;

    AudioSource drawerSocketSound;
    AudioClip drawerMoveClip;
    AudioClip drawerSocketedClip;

    [Header("Hinge Interactable")]

    [SerializeField]
    SimpleHingeInteractable[] cabinetDoors =
        new SimpleHingeInteractable[2];

    AudioSource[] cabinetDoorSound;

    AudioClip cabinetDoorMoveClip;


    [Header("Combo Lock")]
    [SerializeField] ComboLock comboLock;

    AudioSource comboLockSound;

    AudioClip lockComboClip;
    AudioClip unlockComboClip;
    AudioClip comboButtonPressedClip;



    [Header("The Wall")]

    [SerializeField] TheWall wall;
    XRSocketInteractor wallSocket;
    [SerializeField] AudioSource wallSound;
    AudioSource wallSocketSound;
    AudioClip destroyWallClip;
    AudioClip wallSocketClip;

    [Header("Local Audio Settings")]

    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip fallBackClip;
    private const string FallBackClip_Name = "fallBackClip";

    private bool startAudioBool;

    public object OnComboButtonPressed { get; private set; }

    private void OnEnable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
            progressControl.OnChallengeComplete.AddListener(OnChallengeComplete);
        }
        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create(FallBackClip_Name, 1, 1, 1000, true);
        }


        SetGrabbables();

        if (drawer != null)
        {
            SetDrawerInteractable();
        }
        cabinetDoorSound = new AudioSource[cabinetDoors.Length];
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (cabinetDoors[i] != null)
            {
                SetCabinetDoors(i);
            }
        }
        if (comboLock != null)
        {
            SetComboLock();
        }
        if (wall != null)
        {
            SetWall();
        }
    }

    private void OnDisable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.RemoveListener(StartGame);
            progressControl.OnChallengeComplete.RemoveListener(OnChallengeComplete);
        }
        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.RemoveListener(OnSelectEnterGrabbable);
            grabInteractables[i].selectExited.RemoveListener(OnSelectExitGrabbable);
            grabInteractables[i].activated.RemoveListener(OnActivatedGrabbable);
        }
        if (drawer != null)
        {
            drawer.selectEntered.RemoveListener(OnDrawerMove);
            drawer.selectExited.RemoveListener(OnDrawerStop);
            drawer.OnDrawerDetach.RemoveListener(OnDrawerDetach);
        }


        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            cabinetDoors[i].OnHingeSelected.RemoveListener(OnDoorMove);
            cabinetDoors[i].selectExited.RemoveListener(OnDoorStop);
        }
        if (comboLock != null)
        {
            comboLock.UnlockAction.RemoveListener(OnComboUnlocked);
            comboLock.LockAction.RemoveListener(OnComboLocked);
            comboLock.ComboButtonPressed.RemoveListener(ComboButtonPressed);
        }

        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }


        if (wallSocket != null)
        {

            wallSocket.selectEntered.RemoveListener(OnWallSocketed);
        }

    }
    private void OnChallengeComplete(string arg0)
    {
        if (progressSound != null && challengeCompleteClip != null)
        {
            progressSound.clip = challengeCompleteClip;
            progressSound.Play();
        }
    }

    private void StartGame(string arg0)
    {
        if (!startAudioBool)
        {
            startAudioBool = true;
            if (backgroundMusic != null & backgroundMusicClip != null)
            {
                backgroundMusic.clip = backgroundMusicClip;
                backgroundMusic.Play();
            }
        }
        else
        {
            if (progressSound != null && startGameClip != null)
            {
                progressSound.clip = startGameClip;
                progressSound.Play();
            }
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
        };
    }

    private void SetDrawerInteractable()
    {
        // Get the drawer sound
        drawerSound = drawer.transform.AddComponent<AudioSource>();
        // Get the drawer move clip from the drawer object
        drawerMoveClip = drawer.GetDrawerMoveClip;
        // If the drawer move clip is null, set it to the fall back clip
        CheckClip(ref drawerMoveClip);
        // setting the properties of the drawer sound
        drawerSound.clip = drawerMoveClip;
        drawerSound.loop = true;
        // Add the event listeners
        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);
        drawer.OnDrawerDetach.AddListener(OnDrawerDetach);
        drawerSocket = drawer.GetKeySocket;
        if (drawerSocket != null)
        {
            drawerSocketSound = drawerSocket.transform.AddComponent<AudioSource>();
            drawerSocketedClip = drawer.GetSocketedClip;
            CheckClip(ref drawerSocketedClip);
            drawerSocketSound.clip = drawerSocketedClip;
            drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
        }

        drawerPhysicsButton = drawer.GetPhysicsButton;
        if (drawerPhysicsButton != null)
        {
            drawerPhysicsButton.OnBaseEnter.AddListener(OnPhysicsButtonEnter);
            drawerPhysicsButton.OnBaseExit.AddListener(OnPhysicsButtonExit);
        }
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        drawerSocketSound.Play();
    }

    private void OnDrawerDetach()
    {
        isDetached = true;
        drawerSound.Stop();
    }

    private void OnPhysicsButtonExit()
    {
        PlayGrabSound(keyClip);
    }

    private void OnPhysicsButtonEnter()
    {
        PlayGrabSound(keyClip);



    }

    private void SetCabinetDoors(int index)
    {
        cabinetDoorSound[index] = cabinetDoors[index].transform.AddComponent<AudioSource>();
        cabinetDoorMoveClip = cabinetDoors[index].GetHingeMoveClip;
        CheckClip(ref cabinetDoorMoveClip);
        cabinetDoorSound[index].clip = cabinetDoorMoveClip;
        cabinetDoors[index].OnHingeSelected.AddListener(OnDoorMove);
        cabinetDoors[index].selectExited.AddListener(OnDoorStop);
    }

    private void SetComboLock()
    {
        comboLockSound = comboLock.transform.AddComponent<AudioSource>();
        lockComboClip = comboLock.GetLockClip;

        CheckClip(ref lockComboClip);
        unlockComboClip = comboLock.GetUnlockClip;
        CheckClip(ref unlockComboClip);
        comboButtonPressedClip = comboLock.GetComboPressedClip;
        CheckClip(ref comboButtonPressedClip);

        comboLock.UnlockAction.AddListener(OnComboUnlocked);
        comboLock.LockAction.AddListener(OnComboLocked);
        comboLock.ComboButtonPressed.AddListener(ComboButtonPressed);
    }

    private void ComboButtonPressed()
    {
        comboLockSound.clip = comboButtonPressedClip;
        comboLockSound.Play();
    }
    private void OnComboLocked()
    {
        comboLockSound.clip = lockComboClip;
        comboLockSound.Play();
    }

    private void OnComboUnlocked()
    {
        comboLockSound.clip = unlockComboClip;
        comboLockSound.Play();
    }


    private void OnDoorStop(SelectExitEventArgs arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0.interactableObject == cabinetDoors[i])
            {
                cabinetDoorSound[i].Stop();
            }
        }
    }

    private void OnDoorMove(SimpleHingeInteractable arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0 == cabinetDoors[i])
            {
                cabinetDoorSound[i].Play();
            }
        }
    }

    private void SetWall()
    {
        destroyWallClip = wall.GetDestroyClip;
        CheckClip(ref destroyWallClip);
        wall.OnDestroy.AddListener(OnDestroyWall);

        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocketSound = wallSocket.transform.AddComponent<AudioSource>();
            wallSocketClip = wall.GetSocketClip;
            CheckClip(ref wallSocketClip);
            wallSocketSound.clip = wallSocketClip;
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }


    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        wallSocketSound.Play();
    }

    // CheckClip is a helper function that checks if the clip 
    // is null and sets it to the fall back clip if it is
    private void CheckClip(ref AudioClip clip)
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
        if (isDetached)
        {
            PlayGrabSound(grabClip);
        }
        else
        {
            drawerSound.Play();
        }
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
        PlayGrabSound(grabClip);

        // grabSound.Play();

    }

    private void OnSelectEnterGrabbable(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.CompareTag("Key"))
        {
            PlayGrabSound(keyClip);
        }
        else
        {
            PlayGrabSound(grabClip);
        }
    }

    private void OnDestroyWall()
    {
        if (wallSound != null)
        {
            wallSound.Play();
        }
    }

    private void PlayGrabSound(AudioClip clip)
    {
        grabSound.clip = clip;
        grabSound.Play();

    }

}
