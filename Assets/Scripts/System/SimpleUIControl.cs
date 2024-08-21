using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIControl : MonoBehaviour
{
    [SerializeField] ProgressControl progressControl;
    [SerializeField] TMP_Text[] msgTexts;


    void OnEnable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
            progressControl.OnChallengeComplete.AddListener(ChallengeComplete);
        }
    }

    private void ChallengeComplete(string arg0)
    {
        SetText(arg0);
    }

    private void StartGame(string arg0)
    {

        SetText(arg0);
    }

    public void SetText(string msg)
    {

        for (int i = 0; i < msgTexts.Length; i++)
        {
            msgTexts[i].text = msg;
        }
    }
}
