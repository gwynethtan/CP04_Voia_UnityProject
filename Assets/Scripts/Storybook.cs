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
    //private Dictionary<int, bool> pageCompleted = new Dictionary<int, bool>(); // Use later for error handling + double side book
    private Dictionary<int, bool> pageCompleted = new Dictionary<int, bool>();

    public GameObject cube; //for debug
    public TextMeshProUGUI text;


    void Start()
    {
        //total 14 pages
        actionsByPage = new Dictionary<int, Action>
        {
            {1, Page1},
            {2, Page2},
            {3, Page3},
            {4, Page4},
            {5, Page5},
            {6, Page6},
            {7, Page7},
            {8, Page8},
            {9, Page9},
            {10, Page10},
        };
    }

    void Update()
    {
        if(flipPage != null)
        {
            text.text = "Success";
            /* OnPageFlipped(flipPage.CurrentPage); */
        }
        else
        {
            text.text = "Null";
        }
    }

    public void PageSign(int pageNum)
    {
        // Only trigger action if page matches current page
        if (pageNum == flipPage.CurrentPage) 
        {
            if (pageNum > 1)
            {
                int prevPage = pageNum - 1;
                if (!pageCompleted.TryGetValue(prevPage, out bool previousCompleted) || !previousCompleted) 
                {
                    //If previous page not there yet (no entry to dictionary)/previous page action not completed yet
                    return;
                }
            }

            // praise new - For page 8 game interaction
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
                MarkPageCompleted(pageNum); // - Praise (In case wrong)
            }
        }

        else
        {
            //Instantiate(cube);
        }
    }
    public void MarkPageCompleted(int pageNum)
    {
        pageCompleted[pageNum] = true;
    }

    public bool CheckPageCompleted(int pageNum)
    {
        return pageCompleted.TryGetValue(pageNum, out bool completed) && completed;
    }

    /* public void OnPageFlipped(int newPage)
    {
        if (newPage == 2 || newPage == 4)
        {
            if (!pageCompleted.ContainsKey(newPage))
            {
                MarkPageCompleted(newPage);
                Debug.Log("Auto-completed page " + newPage);
            }
        }
    } */

    void Page1()
    {
        flipPage.Page1Functions();

        pageCompleted[1] = true;
    }

    void Page2()
    {
        flipPage.Page2Functions();

        pageCompleted[2] = true;
    }

    void Page3()
    {
        flipPage.Page3Functions();

        pageCompleted[3] = true;
    }

    void Page4()
    {
        flipPage.Page4Functions();

        pageCompleted[4] = true;
    }

    void Page5()
    {
        flipPage.Page5Functions();

        pageCompleted[5] = true;
    } 

    void Page6()
    {
        flipPage.Page6Functions();

        pageCompleted[6] = true;
    }

    void Page7()
    { 
        flipPage.Page7Functions();
        pageCompleted[7] = true;
    }

    void Page8()
    {
        flipPage.Page8Functions();
        pageCompleted[8] = true;
    }

    void Page9()
    {
        flipPage.Page9Functions();
        pageCompleted[9] = true;
    }

    void Page10()
    {
        flipPage.Page10Functions();
        pageCompleted[10] = true;
    }
}
