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

    public GameObject cube; //for debug
    public TextMeshProUGUI text;


    void Start()
    {
        //total 14 pages
        actionsByPage = new Dictionary<int, Action>
        {
            {1, Page1}, // s
            {2, Page2},
            {3, Page3},
            {4, Page4},
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

    public void PageSign(int pageNum)
    {
        if (pageNum == flipPage.CurrentPage) 
        {
            /*if (pageNum > 1)
            {
                int prevPage = pageNum - 1;
                if (!pageCompleted.TryGetValue(prevPage, out bool previousCompleted) || !previousCompleted) 
                {
                    //If previous page not there yet (no entry to dictionary)/previous page action not completed yet
                    return;
                }
            }*/

            if (actionsByPage.TryGetValue(pageNum, out Action action))
            {
                action.Invoke();
            }
        }

        else
        {
            //Instantiate(cube);
        }
    }

    /* Will test
    public void MarkPageCompleted(int pageNum)
    {
        pageCompleted[pageNum] = true;
    }

    public bool CheckPageCompleted(int pageNum)
    {
        return pageCompleted.TryGetValue(pageNum, out bool completed) && completed;
    }*/


    void Page1()
    {
        flipPage.Page1Functions();

        //pageCompleted[1] = true;
    }

    void Page2()
    {
        //flipPage.Page2Functions();

        //pageCompleted[2] = true;
    }

    void Page3()
    {
        //flipPage.Page3Functions();
    }

    void Page4()
    {
        //flipPage.Page4Functions();
    }
}
