using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignDanceManager : MonoBehaviour
{
    /*
    [Header("Text")]
    // Shows what name to dance
    public TextMeshProUGUI promptText;
    // Show whether correct or not (For debug)
    public TextMeshProUGUI feedbackText;*/

    [Header("References")]
    public FlipPage flipPage;  
    public Storybook storybook;


    [Header("Wolves")]
    public GameObject DB_Sing; 
    public GameObject LB_Sing; 
    public GameObject LG_Sing; 

    [Header("Game")]
    public int health = 3;

    // Words to sign
    private string[] targetWords = { "VAL", "BEL", "ELLE" };
    private int currentWordIndex = 0;
    public bool completedAllWords = false;

    public BubbleGroup bubbleGroup;
    public BubbleMgr bubbleMgr;

    private void Start()
    {
        ShowNextPrompt();

        // Assign references
        TranslateSign translateSign = FindObjectOfType<TranslateSign>();
        if (translateSign != null)
        {
            translateSign.signDanceManager = this;
        }

        storybook = FindObjectOfType<Storybook>();
        if (storybook != null)
        {
            storybook.signDanceManager = this;
        }

        bubbleMgr = FindObjectOfType<BubbleMgr>();
    }

    /// <summary>
    /// Call when word is signed
    /// Checks if word is correct and trigger animation
    /// </summary>
    /// <param name="signedWord"></param>
    public void OnWordSigned(string signedWord)
    {
        // Only accept input on page index 6 (Page 10)
        if (flipPage == null || flipPage.GetCurrentPageIndex() != 6)
        {
            Debug.Log("Not on page 10.");
            return;
        }

        signedWord = signedWord.ToUpper();

        // Check if signed word matches current target name
        if (signedWord == targetWords[currentWordIndex])
        {
            ShowEval($"{signedWord} signed correctly!");
            TriggerWolfDance(currentWordIndex);
            currentWordIndex++;

            // Check if all names have been signed
            if (currentWordIndex >= targetWords.Length)
            {
                completedAllWords = true;
                ShowEval("All words signed correctly, tree defeated!");
                return;
            }

            // Show next name to sign
            ShowNextPrompt();
        }
        else
        {
            ShowEval($"{signedWord} signed wrongly. Please try again.");
        }
    }

    /// <summary>
    /// Function to show next name to sign
    /// </summary>
    public void ShowNextPrompt()
    {
        if (currentWordIndex < targetWords.Length)
        {
            ShowNamePrompt($"Sign: {targetWords[currentWordIndex]}");
        }
        else
        {
            ShowNamePrompt("");
        }
    }

    /// <summary>
    /// Trigger wolf animation and health based on signed name
    /// </summary>
    /// <param name="index"></param>
    private void TriggerWolfDance(int index)
    {
        if (health <= 0) return;

        switch (index)
        {
            // First sign
            case 0:
                DB_Sing.GetComponent<Animator>().SetBool("isSigned3", true);
                health -= 1;
                break;
            // Second sign
            case 1:
                LB_Sing.GetComponent<Animator>().SetBool("isSigned", true);
                health -= 1;
                break;
            // Third sign
            case 2:
                LG_Sing.GetComponent<Animator>().SetBool("isSigned4", true);
                health -= 1;

                // Addtional things to happen
                flipPage.angry.SetActive(false);
                flipPage.smiling.SetActive(true);
                flipPage.angryEffect.SetActive(false);
                flipPage.rain.SetActive(false);

                // Mark page completed
                storybook.pageCompleted[6] = true;
                storybook.MarkPageCompleted(6);
                //feedbackText.gameObject.SetActive(false);
                //promptText.gameObject.SetActive(false);
                break;
        }

        if (flipPage != null)
        {
            // Update health bar
            flipPage.UpdateHealthBar(health);
        }
    }

    public void ShowNamePrompt(string namePrompt)
    {
        StartCoroutine(bubbleMgr.ActivateBubble(bubbleGroup, namePrompt, false));
    }

    public void DisableNamePrompt()
    {
        bubbleMgr.HandleBubbleDismissal(bubbleGroup);
    }

    public void ShowEval(string evaluationText)
    {
        StartCoroutine(bubbleMgr.ActivateBubble(bubbleGroup, evaluationText, true));
    }

    /// <summary>
    /// Reset game state (JIC)
    /// </summary>
    public void ResetGame()
    {
        currentWordIndex = 0;
        completedAllWords = false;
        ShowNextPrompt();
    }
}
