using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;
public class ComboLock : MonoBehaviour
{

    [SerializeField] TMP_Text userInputText;
    [SerializeField] XrButtonInteractable[] comboButtons;
    [SerializeField] TMP_Text infoText;
    private const string startString = "Enter 3 Digit Combo";
    private const string resetString = "Enter 3 Digit to Reset Combo";
    [SerializeField] Image lockedPanel;
    [SerializeField] Color unlockedColors;
    [SerializeField] Color lockedColors;
    [SerializeField] private const string unlockedString = "Unlocked";
    [SerializeField] private const string lockedString = "Locked";
    [SerializeField] TMP_Text lockedText;

    [SerializeField] bool isLocked;
    [SerializeField] bool isResettable;

    private bool resetCombo;

    [SerializeField] int[] comboValues = new int[3];
    [SerializeField] int[] inputValues;

    private int maxButtonPresses;

    private int buttonPresses;

    void Start()
    {
        maxButtonPresses = comboValues.Length;
        ResetUserValues();

        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].selectEntered.AddListener(OnComboButtonsPressed);
        }


    }

    private void OnComboButtonsPressed(SelectEnterEventArgs arg0)
    {
        if (buttonPresses >= maxButtonPresses)
        {
            // too many button presses, reset
            userInputText.text = "";
            buttonPresses = 0;
        }
        else
        {
            for (int i = 0; i < comboButtons.Length; i++)
            {
                if (arg0.interactableObject.transform.name == comboButtons[i].transform.name)
                {
                    userInputText.text += i.ToString();
                    inputValues[buttonPresses] = i;
                }
                else
                {
                    comboButtons[i].resetColor();
                }
            }
            buttonPresses++;
            if (buttonPresses == maxButtonPresses)
            {
                CheckCombo();
            }

        }
    }

    private void CheckCombo()
    {
        if (resetCombo)
        {
            resetCombo = false;
            LockCombo();
            return;
        }

        int matches = 0;

        for (int i = 0; i < maxButtonPresses; i++)
        {
            if (comboValues[i] == inputValues[i])
            {
                matches++;
            }
        }
        if (matches == maxButtonPresses)
        {
            UnlockCombo();
            isLocked = false;
            lockedPanel.color = unlockedColors;
            lockedText.text = unlockedString;
        }
        else
        {
            ResetUserValues();
        }
    }

    private void UnlockCombo()
    {
        isLocked = false;
        lockedPanel.color = unlockedColors;
        lockedText.text = unlockedString;
        if (isResettable)
        {
            ResetCombo();
        }
    }
    private void LockCombo()
    {
        isLocked = true;
        lockedPanel.color = lockedColors;
        lockedText.text = lockedString;
        infoText.text = startString;
        for (int i = 0; i < maxButtonPresses; i++)
        {
            comboValues[i] = inputValues[i];
        }
        ResetUserValues();
    }
    private void ResetCombo()
    {
        infoText.text = resetString;
        ResetUserValues();
        resetCombo = true;
    }

    private void ResetUserValues()
    {
        inputValues = new int[maxButtonPresses];
        userInputText.text = "";
        buttonPresses = 0;
    }
}
