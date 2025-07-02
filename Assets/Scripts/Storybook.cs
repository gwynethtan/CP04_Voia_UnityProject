using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storybook : MonoBehaviour
{
    public FlipPage flipPage;
    public Transform bookPosition;

    private Dictionary<int, Action> actionsByPage;
    private Dictionary<int, bool> pageCompleted = new Dictionary<int, bool>();

    public TextMeshProUGUI text;
    public TextMeshProUGUI pageCheck;
    public TextMeshProUGUI completedCheck;

    void Start()
    {
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
        if(flipPage != null)
        {
            text.text = "Success";
        }
        else
        {
            text.text = "Null";
        }
    }

    public void SignCurrentPage()
    {
        if(flipPage == null)
        {
            completedCheck.text = "Cannot sign, no FlipPage.";
            return;
        }

        if (flipPage.isFlipping)
        {
            completedCheck.text = "Page is flipping...";
            return;
        }

        completedCheck.text = "CurrentPage signed";
        PageSign(flipPage.CurrentPage);
    }

    public void PageSign(int pageNum)
    {
        // completedCheck.text = "Page:::: " +pageNum;
        // Only trigger action if page matches current page
        if (pageNum == flipPage.CurrentPage) 
        {
            if (pageNum > 1)
            {
                int prevPage = pageNum - 1;
                if (!pageCompleted.TryGetValue(prevPage, out bool previousCompleted) || !previousCompleted) 
                {
                    //If previous page not there yet (no entry to dictionary)/previous page action not completed yet
                    completedCheck.text = "Previous page not complete.";
                    return;
                }
            }
            else
            {
                completedCheck.text = "Previous page completed, proceed.";
            }

            /*// praise new - For page 8 game interaction
            if (pageNum == 8 && flipPage.isPage8Active)
            {
                flipPage.health--;
                flipPage.slider.value = flipPage.health;

                if (flipPage.health <= 0)
                {
                    flipPage.isPage8Active = false;
                    MarkPageCompleted(8);
                    Debug.Log("Page 8 completed after 5 signs.");
                }

                return;
            }

            if (actionsByPage.TryGetValue(pageNum, out Action action))
            {
                action.Invoke();
                //MarkPageCompleted(pageNum); // - Praise (In case wrong)
                pageCheck.text = "Page Completed (Storybook): " + pageNum;
            }*/
        }

        if (actionsByPage.TryGetValue(pageNum, out Action action))
        {
            action.Invoke();
            MarkPageCompleted(pageNum); // - Praise (In case wrong)
            pageCheck.text = "Page Completed (Storybook): " + pageNum;
        }
        else
        {
            pageCheck.text = "nO MATCH";
        }
    }

    public void MarkPageCompleted(int pageNum)
    {
        pageCompleted[pageNum] = true;
        flipPage.pageFlipTrigger.enabled = true;
    }

    public bool CheckPageCompleted(int pageNum)
    {
        return pageCompleted.TryGetValue(pageNum, out bool completed) && completed;
    }

    void Page1()
    {
        flipPage.Page1Functions();
        pageCompleted[1] = true;
    }

    void Page3()
    {
        flipPage.Page3Functions();
        pageCompleted[2] = true;
    }

    void Page5()
    {
        flipPage.Page5Functions();
        pageCompleted[3] = true;
    }

    void Page6()
    {
        flipPage.Page6Functions();
        pageCompleted[4] = true;
    } 

    void Page8()
    {
        flipPage.Page8Functions();
        pageCompleted[5] = true;
    }

    void Page10()
    { 
        flipPage.Page10Functions();
        pageCompleted[6] = true;
    }

    void Page13()
    {
        flipPage.Page13Functions();
        pageCompleted[7] = true;
    }

    void Page14()
    {
        flipPage.Page14Functions();
        pageCompleted[8] = true;
    }
}
