/*
 * Author: Jacie Thoo Yixuan
 * Date: 27/7/2025
 * Description: This Script handles the mini game mechanics
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignDanceManager : MonoBehaviour
{
    [Header("Text")]
    /// <summary>
    /// Canvas that holds name prompt
    /// </summary>
    public GameObject namePromptUI;

    /// <summary>
    /// Text that displays the wolf name prompt (to sign)
    /// </summary>
    public TextMeshProUGUI namePromptText;

    [Header("References")]
    /// <summary>
    /// Reference to PageManager script
    /// </summary>
    public PageManager pageManager;

    /// <summary>
    /// Reference to Storybook script
    /// </summary>
    public Storybook storybook;

    /// <summary>
    /// Reference to BubbleGroup
    /// </summary>
    public BubbleGroup bubbleGroup;

    /// <summary>
    /// Reference to BubbleMgr script
    /// </summary>
    public BubbleMgr bubbleMgr;


    [Header("Wolves")]
    /// <summary>
    /// Reference to wolf 1
    /// </summary>
    public GameObject DB_Sing; 

    /// <summary>
    /// Reference to wolf 2
    /// </summary>
    public GameObject LB_Sing;

    /// <summary>
    /// Reference to wolf 3
    /// </summary>
    public GameObject LG_Sing; 

    [Header("Game")]
    /// <summary>
    /// Starting tree health amount for the mini game
    /// </summary>
    public int health = 3;

    /// <summary>
    /// Array of wolf names user must sign
    /// </summary>
    private string[] targetWords = { "VAL", "BEL", "ELLE" };

    /// <summary>
    /// Tracks current index of the target word being signed
    /// </summary>
    private int currentWordIndex = 0;

    /// <summary>
    /// Whether all target words have been correctly soigned
    /// </summary>
    public bool completedAllWords = false;


    /// <summary>
    /// Initialises references
    /// </summary>
    private void Start()
    {
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
        if (bubbleMgr == null || bubbleGroup == null)
        {
            return;
        }

        // Only accept input on page index 6 (Page 10)
        if (pageManager == null || pageManager.GetCurrentPageIndex() != 6)
        {
            Debug.Log("Not on page 10.");
            return;
        }

        if (!pageManager.danceSigned || signedWord == "dance")
        {
            return;
        }

        signedWord = signedWord.ToUpper();

        // Check if signed word matches current target name
        if (signedWord == targetWords[currentWordIndex] && pageManager.danceSigned)
        {
            ShowEval($"{targetWords[currentWordIndex]} signed correctly!");
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
            ShowEval($"{targetWords[currentWordIndex]} signed wrongly. Please try again.");
        }
    }

    /// <summary>
    /// Function to show next name to sign
    /// </summary>
    public void ShowNextPrompt()
    {
        if (currentWordIndex < targetWords.Length)
        {
            namePromptText.text = $"Sign: {targetWords[currentWordIndex]}";
        }
        else
        {
            namePromptText.text = "";
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
                pageManager.angry.SetActive(false);
                pageManager.smiling.SetActive(true);
                pageManager.angryEffect.SetActive(false);
                pageManager.rain.SetActive(false);
                pageManager.slider.gameObject.SetActive(false);

                // Mark page completed
                storybook.pageCompleted[6] = true;
                storybook.MarkPageCompleted(6);
                StartCoroutine(bubbleMgr.Fade(1f, 0f, namePromptUI));

                break;
        }

        if (pageManager != null)
        {
            // Update health bar
            pageManager.UpdateHealthBar(health);
        }
    }

    /// <summary>
    /// Show evaluation whether correct or wrong with bubblemgr
    /// </summary>
    /// <param name="evaluationText"></param>
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