/*
* Author: Tan Ting Yu Gwyneth
* Date: 14/7/2025
* Description: This file defines both the individual and overall daily badges classes
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBadges
{
    /// <summary>
    /// Variable to store the last reset date for the badges
    /// </summary>
    public long lastReset;

    /// <summary>
    /// Variable to store the number of badges completed
    /// </summary>
    public int badgesCompleted;

    /// <summary>
    /// Initializes an empty DailyBadges object
    /// </summary>
    public DailyBadges()
    {
        // This constructor initializes an empty DailyBadges object
    }

    /// <summary>
    /// Constructor with parameters to initialize all fields
    /// </summary>
    /// <param name="lastReset">Last reset date for all badges</param>
    /// <param name="badgesCompleted">Number of badges completed</param>
    public DailyBadges(long lastReset, int badgesCompleted)
    {
        this.lastReset = lastReset; 
        this.badgesCompleted = badgesCompleted; 
    }
}

public class IndivBadges
{
    /// <summary>
    /// Variable to store the minimum amount points to earn the badge
    /// </summary>
    public int goal;

    /// <summary>
    /// Variable to store the current score of the badge
    /// </summary>
    public int currentScore;

    /// <summary>
    /// Initializes an empty IndivBadges object
    /// </summary>
    public IndivBadges()
    {
        // This constructor initializes an empty IndivBadges object
    }

    /// <summary>
    /// Constructor with parameters to initialize all fields
    /// </summary>
    /// <param name="goal">Minimum amount points to earn the badge</param>
    /// <param name="currentScore">Current user score of the badge</param>
    public IndivBadges(int goal, int currentScore)
    {
        this.goal = goal; 
        this.currentScore = currentScore; 
    }
}