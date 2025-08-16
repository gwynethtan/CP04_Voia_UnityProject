/*
 * Author: Jacie Thoo Yixuan
 * Date: 
 * Description: This Script handles storybook
 */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storybook : MonoBehaviour
{
    /// <summary>
    /// Reference to flipPage script
    /// </summary>
    public PageManager pageManager;

    /// <summary>
    /// Reference to signDanceManager script
    /// </summary>
    public SignDanceManager signDanceManager;

    /// <summary>
    /// Reference to translateSign script
    /// </summary>
    public TranslateSign translateSign;

    /// <summary>
    /// Reference to userDataManager script
    /// </summary>
    public UserDataManager userDataManager;

    /// <summary>
    /// Stores the position of the book
    /// </summary>
    public Transform bookPosition;

    private Dictionary<int, Action> actionsByPage;
    public Dictionary<int, bool> pageCompleted = new Dictionary<int, bool>();

    // Dictionary for matching page and words
    private Dictionary<int, string> expectedSigns = new Dictionary<int, string>()
    {
        { 1, "Wolf" },
        { 2, "Sing" },
        { 3, "Talking" },
        { 4, "Rain" },
        { 5, "Climb" },
        { 6, "Dance" },
        { 7, "Heart" },
        { 8, "Happily" }
    };

    public string signedWord;

    public TextMeshProUGUI text;
    public TextMeshProUGUI pageCheck;
    public TextMeshProUGUI completedCheck;

    public bool storybookCompleted = false;

    public GameObject congratsMsg;

    void Start()
    {
        congratsMsg.SetActive(false);
        signDanceManager = FindObjectOfType<SignDanceManager>();
        if (signDanceManager != null)
        {
            signDanceManager.storybook = this;
        }

        //total 14 actual pages, 8 signed pages
        actionsByPage = new Dictionary<int, Action>
        {
            {1, Page1},
            {2, Page3},
            {3, Page5},
            {4, Page6},
            {5, Page8},
            {6, Page10},
            {7, Page13},
            {8, Page14},
        };

    }

    
    void Update()
    {
        // Keep updating signedWord
        signedWord = translateSign.GetSignedWord();
        //Debug.Log(signedWord);
    }

    public void SignCurrentPage()
    {
        if(pageManager == null)
        {
            Debug.Log("Cannot sign, no FlipPage.");
            return;
        }

        if (pageManager.isFlipping)
        {
            Debug.Log("Page is flipping...");
            return;
        }

        Debug.Log("CurrentPage signed");
        PageSign(pageManager.CurrentPage);
    }

    /// <summary>
    /// Compares signedWord and expectedWord and starts page activities if correct
    /// </summary>
    /// <param name="pageNum"></param>
    public void PageSign(int pageNum)
    {
        if (pageNum == pageManager.CurrentPage)
        {
            if (pageNum > 1)
            {
                int prevPage = pageNum - 1;
                // Check whether previous page completed
                if (!pageCompleted.TryGetValue(prevPage, out bool previousCompleted) || !previousCompleted)
                {
                    Debug.Log("Previous page not complete");
                    return;
                }
            }

            // Check if there is expected sign for this page
            if (!expectedSigns.TryGetValue(pageNum, out string expectedWord))
            {
                Debug.Log("No expected sign");
                return;
            }

            // Check whether something has been signed ans stored as signedWord
            if (string.IsNullOrEmpty(signedWord))
            {
                Debug.Log("No signed word detected");
                return;
            }

            // Compare signed word to expected word (ignore case)
            if (signedWord.Equals(expectedWord, StringComparison.OrdinalIgnoreCase) )
            {
                Debug.Log($"Correct sign for page {pageNum}: {signedWord}");

                // Call page action
                if (actionsByPage.TryGetValue(pageNum, out Action action))
                {
                    action.Invoke();

                    // Mark page completed unless it is game page
                    if (pageManager.CurrentPage != 6) 
                    {
                        MarkPageCompleted(pageNum);
                    }
                    Debug.Log("Page Completed (Storybook): " + pageNum);
                }
            }

            // if incorrect word signed
            else
            {
                Debug.Log($"Incorrect sign. Expected; {expectedWord}");
                return;
            }
        }
        else
        {
            Debug.Log("Page not matching");
        }
    }


    /// <summary>
    /// Checks whether page is done and unlock trigger to flip to next page
    /// </summary>
    /// <param name="pageNum"></param>
    public void MarkPageCompleted(int pageNum)
    {
        if (pageCompleted[pageNum] == true)
        {
            pageManager.pageFlipTrigger.enabled = true;
        }
    }

    void Page1()
    {
        pageManager.Page1Functions();
        pageCompleted[1] = true;
    }

    void Page3()
    {
        pageManager.Page3Functions();
        pageCompleted[2] = true;
    }

    void Page5()
    {
        pageManager.Page5Functions();
        pageCompleted[3] = true;
    }

    void Page6()
    {
        pageManager.Page6Functions();
        pageCompleted[4] = true;
    } 

    void Page8()
    {
        pageManager.Page8Functions();
        pageCompleted[5] = true;
    }

    /// <summary>
    /// Game page
    /// </summary>
    void Page10()
    { 
        pageManager.Page10Functions();
    }

    void Page13()
    {
        pageManager.Page13Functions();
        pageCompleted[7] = true;
    }

    void Page14()
    {
        pageManager.Page14Functions();
        pageCompleted[8] = true;
    }

    public void CheckStorybookCompleted()
    {
        for (int i = 1; i <= 8; i++)
        {
            if (!pageCompleted.ContainsKey(i) || !pageCompleted[i])
            {
                Debug.Log("Not all pages complete.");
                return;
            }
        }

        storybookCompleted = true;
        Debug.Log("Storybook completed");
        congratsMsg.SetActive(true);
        userDataManager.UpdateIndivBadges(1, "bookTrackerToday", "booksRead");
    }
}
