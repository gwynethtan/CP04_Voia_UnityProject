/*
* Author: Tan Ting Yu Gwyneth
* Date: 1/12/2024
* Description: This file defines the MainGoals class, which is used to store their results that can greatly determine their work ethic as a firefighter
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityDetails
{
    /// <summary>
    /// Variable to store the date the player's health
    /// </summary>
    public int booksRead;

    /// <summary>
    /// Variable to store player score
    /// </summary>
    public int numberOfTimesSpelt;

    /// <summary>
    /// Variable to store player score
    /// </summary>
    public int typeOfWordsSpelt;

    /// <summary>
    /// Variable to store player score
    /// </summary>
    public int wordsSpeltCorrectly;

    /// <summary>
    /// Variable to stored the total signed words by user
    /// </summary>
    public int totalSignedWords;

    /// <summary>
    /// Initializes an empty MainGoals object
    /// </summary>
    public ActivityDetails()
    {
        // This constructor initializes an empty MainGoals object
    }

    /// <summary>
    /// Constructor with parameters to initialize all fields
    /// </summary>
    /// <param name="savedDog">Indicates if the player saved the dog</param>
    /// <param name="timeTakenToSaveDog">The time taken to save the dog</param>
    /// <param name="healthRemaining">The player's remaining health</param>
    /// <param name="score">The player's score</param>
    public ActivityDetails(int booksRead,int numberOfTimesSpelt, int totalSignedWords)
    {
        this.booksRead = booksRead; 
        this.numberOfTimesSpelt = numberOfTimesSpelt;
        this.totalSignedWords = totalSignedWords;
    }
}
