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

    public GameObject cube; //for debug
    public TextMeshProUGUI text;


    void Start()
    {
        actionsByPage = new Dictionary<int, Action>
        {
            {1, Page1},
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

    void Page1()
    {
        flipPage.Page1Functions();
    }

    void Page2()
    {
        //wtv
    }

    void Page3()
    {
        //wtv
    }

    void Page4()
    {
        //wtv
    }
}
