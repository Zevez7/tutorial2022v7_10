using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ComboLock : MonoBehaviour
{
    public UnityEvent UnlockAction;
    private void OnUnlocked() => UnlockAction?.Invoke();
    public UnityEvent LockAction;
    private void OnLocked() => LockAction?.Invoke();

    public UnityEvent ComboButtonPressed;

    private void OnComboButtonPress() => ComboButtonPressed?.Invoke();



    [SerializeField] TMP_Text userInputText;
    [SerializeField] XrButtonInteractable[] comboButtons;
    [SerializeField] TMP_Text infoText;
    private const string Start_String = "Enter 3 Digit Combo";
    private const string Reset_String = "Enter 3 Digit to Reset Combo";
    [SerializeField] Image lockedPanel;
    [SerializeField] Color unlockedColors;
    [SerializeField] Color lockedColors;
    [SerializeField] private const string Unlocked_String = "Unlocked";
    [SerializeField] private const string Locked_String = "Locked";
    [SerializeField] TMP_Text lockedText;

    [SerializeField] bool isLocked;
    [SerializeField] bool isResettable;

    private bool resetCombo;

    [SerializeField] int[] comboValues = new int[3];
    [SerializeField] int[] inputValues;


    [SerializeField] AudioClip lockComboClip;
    public AudioClip GetLockClip => lockComboClip;
    [SerializeField] AudioClip unlockComboClip;
    public AudioClip GetUnlockClip => unlockComboClip;
    [SerializeField] AudioClip comboButtonPressedClip;
    public AudioClip GetComboPressedClip => comboButtonPressedClip;


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
            else
            {
                OnComboButtonPress();
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
            lockedText.text = Unlocked_String;
        }
        else
        {
            ResetUserValues();
        }
    }

    private void UnlockCombo()
    {
        isLocked = false;
        OnUnlocked();
        lockedPanel.color = unlockedColors;
        lockedText.text = Unlocked_String;
        if (isResettable)
        {
            ResetCombo();
        }
    }
    private void LockCombo()
    {
        isLocked = true;
        OnLocked();
        lockedPanel.color = lockedColors;
        lockedText.text = Locked_String;
        infoText.text = Start_String;
        for (int i = 0; i < maxButtonPresses; i++)
        {
            comboValues[i] = inputValues[i];
        }
        ResetUserValues();
    }
    private void ResetCombo()
    {
        infoText.text = Reset_String;
        ResetUserValues();
        resetCombo = true;
    }

    private void ResetUserValues()
    {
        if (isLocked)
        {
            OnLocked();
        }
        inputValues = new int[maxButtonPresses];
        userInputText.text = "";
        buttonPresses = 0;
    }
}
