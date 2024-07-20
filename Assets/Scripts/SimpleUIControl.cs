using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIControl : MonoBehaviour
{
    [SerializeField] XrButtonInteractable startButton;
    [SerializeField] string[] msgStrings;
    [SerializeField] TMP_Text[] msgTexts;

    [SerializeField] GameObject keyIndicatorLight1;
    [SerializeField] GameObject keyIndicatorLight2;

    // Start is called before the first frame update
    void Start()
    {
        if (startButton != null)
        {
            startButton.selectEntered.AddListener(OnStartButtonPressed);

        }

    }

    private void OnStartButtonPressed(SelectEnterEventArgs arg0)
    {
        Debug.Log("Start Button Pressed");
        if (keyIndicatorLight1 != null && keyIndicatorLight2 != null)
        {
            keyIndicatorLight1.SetActive(true);
            keyIndicatorLight2.SetActive(true);
        }
        SetText(msgStrings[1]);
    }

    public void SetText(string msg)
    {

        msgTexts[0].text = msg;
        // for (int i = 0; i < msgStrings.Length; i++)
        // {

        // }
    }
}
