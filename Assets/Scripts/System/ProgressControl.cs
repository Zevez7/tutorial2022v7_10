using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.Events;
using System;
public class ProgressControl : MonoBehaviour
{
    public UnityEvent<string> OnStartGame;
    public UnityEvent<string> OnChallengeComplete;

    [Header("Start Button")]

    [SerializeField] XrButtonInteractable startButton;

    [SerializeField] GameObject keyIndicatorLight1;
    [SerializeField] GameObject keyIndicatorLight2;

    [Header("Drawer Interactable")]
    [SerializeField] DrawerInteractable2 drawer;

    XRSocketInteractor drawerSocket;
    [Header("Combo Lock")]

    [SerializeField] ComboLock comboLock;
    [Header("The Wall")]

    [SerializeField] TheWall wall;
    XRSocketInteractor wallSocket;

    [SerializeField] GameObject teleportationArea;

    [Header("Library")]
    [SerializeField] SimpleSliderControl librarySlider;

    [Header("Challenge Settings")]
    [SerializeField] string startGameString;

    [SerializeField] string[] challengeStrings;

    private bool startGameBool;

    private int challengeNumber;


    // Start is called before the first frame update
    void Start()
    {
        if (startButton != null)
        {

            startButton.selectEntered.AddListener(OnStartButtonPressed);

        }
        OnStartGame?.Invoke(startGameString);
        SetDrawerInteractable();
        if (comboLock != null)
        {
            comboLock.UnlockAction.AddListener(OnComboUnlocked);
        }

        if (wall != null)
        {
            SetWall();
        };
        if (librarySlider != null)
        {
            librarySlider.OnSliderActive.AddListener(OnLibrarySliderActive);
        }
    }

    private void OnLibrarySliderActive()
    {
        ChallengeComplete();
    }

    private void ChallengeComplete()
    {
        challengeNumber++;
        if (challengeNumber < challengeStrings.Length)
        {
            OnChallengeComplete?.Invoke(challengeStrings[challengeNumber]);
        }
        else if (challengeNumber >= challengeStrings.Length)
        {
            // OnChallengeComplete?.Invoke("All Challenges Complete");

        }
    }

    private void OnStartButtonPressed(SelectEnterEventArgs arg0)
    {
        if (!startGameBool)
        {
            startGameBool = true;
            Debug.Log("Start Button Pressed");
            Debug.Log("keyIndicatorLight1", keyIndicatorLight1);
            Debug.Log("keyIndicatorLight2", keyIndicatorLight2);
            // if (keyIndicatorLight1 != null && keyIndicatorLight2 != null)
            // {
            keyIndicatorLight1.SetActive(true);
            keyIndicatorLight2.SetActive(true);
            // }
            if (challengeNumber < challengeStrings.Length)
            {
                OnStartGame?.Invoke(challengeStrings[challengeNumber]);

            }
        }


    }
    private void SetDrawerInteractable()
    {
        if (drawer != null)
        {
            drawer.OnDrawerDetach.AddListener(OnDrawerDetach);
            drawerSocket = drawer.GetKeySocket;
            if (drawerSocket != null)
            {
                drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
            }
        }
    }

    private void OnDrawerDetach()
    {
        ChallengeComplete();
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        ChallengeComplete();
    }


    private void OnComboUnlocked()
    {
        ChallengeComplete();
    }

    private void SetWall()
    {
        wall.OnDestroy.AddListener(OnDestroyWall);

        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }

    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        ChallengeComplete();
    }

    private void OnDestroyWall()
    {
        ChallengeComplete();
        if (teleportationArea != null)
        {

            teleportationArea.SetActive(true);
        }
    }
}
