using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfOpenClosehis : MonoBehaviour
{
    public GameObject PopUpQuiz; // The pop-up quiz UI
    public GameObject ToolBar; // The toolbar UI

    private bool isOff = true; // Tracks the state of the toolbar
    private bool lastPopUpState; // Tracks the previous state of the PopUpQuiz

    void Start()
    {
        // Initialize the toolbar's state based on PopUpQuiz's initial state
        lastPopUpState = PopUpQuiz.activeSelf;

        if (PopUpQuiz.activeSelf)
        {
            isOff = false;
            ToolBar.SetActive(false); // Hide the toolbar if the quiz is active
        }
        else
        {
            isOff = true;
            ToolBar.SetActive(true); // Show the toolbar if the quiz is inactive
        }
    }

    void Update()
    {
        // Track the current state of PopUpQuiz
        bool currentPopUpState = PopUpQuiz.activeSelf;

        // If the state has changed, update the toolbar accordingly
        if (currentPopUpState != lastPopUpState)
        {
            if (currentPopUpState) // PopUpQuiz is active, hide the toolbar
            {
                isOff = false;
                ToolBar.SetActive(false);
            }
            else // PopUpQuiz is inactive, show the toolbar
            {
                isOff = true;
                ToolBar.SetActive(true);
            }

            // Update the last state
            lastPopUpState = currentPopUpState;
        }
    }
}
