/*
* Author: Tan Ting Yu Gwyneth
* Date: 1/12/2024
* Description: This file defines the MainGoals class, which is used to store their results that can greatly determine their work ethic as a firefighter
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBadges
{
    /// <summary>
    /// Variable to store the date the player's health
    /// </summary>
    public string lastReset;

    /// <summary>
    /// Variable to store player score
    /// </summary>
    public int badgesCompleted;

    /// <summary>
    /// Initializes an empty MainGoals object
    /// </summary>
    public DailyBadges()
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
    public DailyBadges(string lastReset, int badgesCompleted)
    {
        this.lastReset = lastReset; // Set savedDog
        this.badgesCompleted = badgesCompleted; // Set time to save dog
    }
}

public class IndivBadges
{
    /// <summary>
    /// Variable to store the date the player's health
    /// </summary>
    public int goal;

    /// <summary>
    /// Variable to store player score
    /// </summary>
    public int currentScore;

    /// <summary>
    /// Initializes an empty MainGoals object
    /// </summary>
    public IndivBadges()
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
    public IndivBadges(int goal, int currentScore)
    {
        this.goal = goal; // Set savedDog
        this.currentScore = currentScore; // Set time to save dog
    }
}