/*
* Author: Tan Ting Yu Gwyneth
* Date: 1/12/2024
* Description: This file defines the activity details of the class, which stores their overall activity points
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityDetails
{
    /// <summary>
    /// Variable to store the number of books read
    /// </summary>
    public int booksRead;

    /// <summary>
    /// Variable to store the number of times they spelt a word with sign language
    /// </summary>
    public int numberOfTimesSpelt;

    /// <summary>
    /// Variable to stored the total words signed by user
    /// </summary>
    public int totalSignedWords;

    /// <summary>
    /// Initializes an empty ActivityDetails object
    /// </summary>
    public ActivityDetails()
    {
        // This constructor initializes an empty ActivityDetails object
    }

    /// <summary>
    /// Constructor with parameters to initialize all fields
    /// </summary>
    /// <param name="booksRead">Number of books read</param>
    /// <param name="numberOfTimesSpelt">Number of times they spelt a word</param>
    /// <param name="totalSignedWords">Total words signed by user</param>
    public ActivityDetails(int booksRead,int numberOfTimesSpelt, int totalSignedWords)
    {
        this.booksRead = booksRead; 
        this.numberOfTimesSpelt = numberOfTimesSpelt;
        this.totalSignedWords = totalSignedWords;
    }
}
