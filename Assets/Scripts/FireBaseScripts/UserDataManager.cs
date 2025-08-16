/*
 * Author: Hoo Ying Qi Praise and Tan Ting Yu Gwyneth
 * Date: 14/7/2025
 * Description: This script handles saving and retrieving user data from Firebase Realtime Database
 */

using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    /// <summary>
    /// Gets instance of user data manager
    /// </summary>
    public static UserDataManager Instance { get; private set; }

    /// <summary>
    /// Reference to database
    /// </summary>
    private DatabaseReference dbRef;

    /// <summary>
    /// Reference to auth script for userId
    /// </summary>
    public AuthManager authManager;

    /// <summary>
    /// Variable to store the current user id 
    /// </summary>
    public string currentUserId = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /// <summary>
    /// Gets the current user id to store inside player current stats node
    /// </summary>
    /// <returns></returns>
    public string SetCurrentUserId()
    {
        currentUserId = authManager.currentUserId; // Checking if user logged out or not
        return currentUserId;
    }

    /// <summary>
    /// Stores new user data into database upon sign up / login
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <param name="username"></param>
    /// <param name="dateCreated"></param>
    /// <param name="userOnline"></param>
    public void SaveInitialUserData(string userId, string email, string username,long dateCreated, bool userOnline)
    {
        if (string.IsNullOrEmpty(username))
        {
            username = userId;
        }

        Debug.Log("Creating player");

        // New data created for it to be pushed into database later
        UserData userData = new UserData(email, username, dateCreated, userOnline);
        ActivityDetails activityDetails = new ActivityDetails(0, 0, 0);
        DailyBadges dailyBadges = new DailyBadges(dateCreated, 0);
        IndivBadges indivBadges = new IndivBadges(0, 0);

        // Generate unique paths
        var playerPath = dbRef.Child("users").Child(userId);
        var userDetailsPath = playerPath.Child("userDetails");
        var activityDetailsPath = playerPath.Child("activityDetails");
        var dailyBadgesPath = playerPath.Child("dailyBadges");
        var bookTrackerTodayPath = dailyBadgesPath.Child("bookTrackerToday");
        var pointTrackerTodayPath = dailyBadgesPath.Child("pointTrackerToday");
        var signTrackerTodayPath = dailyBadgesPath.Child("signTrackerToday");
        var wordTrackerTodayPath = dailyBadgesPath.Child("wordTrackerToday");

        // Use async methods to ensure data is set correctly
        userDetailsPath.SetRawJsonValueAsync(JsonUtility.ToJson(userData));
        activityDetailsPath.SetRawJsonValueAsync(JsonUtility.ToJson(activityDetails));
        dailyBadgesPath.SetRawJsonValueAsync(JsonUtility.ToJson(dailyBadges));
        bookTrackerTodayPath.SetRawJsonValueAsync(JsonUtility.ToJson(indivBadges));
        pointTrackerTodayPath.SetRawJsonValueAsync(JsonUtility.ToJson(indivBadges));
        signTrackerTodayPath.SetRawJsonValueAsync(JsonUtility.ToJson(indivBadges));
        wordTrackerTodayPath.SetRawJsonValueAsync(JsonUtility.ToJson(indivBadges));
        playerPath.Child("points").SetValueAsync(0);
    }

    /// <summary>
    /// Update user online status
    /// </summary>
    /// <param name="currentUserId"></param>
    /// <param name="playerOnline"></param>
    public void UpdateUserOnline(bool userOnline)
    {
        Dictionary<string, object> updatedDetails = new Dictionary<string, object>
        {
            ["userOnline"] = userOnline
        };
        dbRef.Child("users").Child(SetCurrentUserId()).Child("userDetails").UpdateChildrenAsync(updatedDetails);
        Debug.Log("Updated playerDetails date");
    }

    /// <summary>
    /// Updates relevant badges and points when user finished a task
    /// </summary>
    /// <param name="points"></param>
    /// <param name="badgeType"></param>
    /// <param name="activityType"></param>
    public async void UpdateIndivBadges(int points, string badgeType, string activityType)
    {
        var pointsRef = dbRef.Child("users").Child(SetCurrentUserId()).Child("points");
        DataSnapshot pointsSnap = await pointsRef.GetValueAsync();
        int currentPoints = pointsSnap.Exists ? Convert.ToInt32(pointsSnap.Value) : 0;
        await pointsRef.SetValueAsync(currentPoints + points);

        var activityRef = dbRef.Child("users").Child(SetCurrentUserId()).Child("activityDetails").Child(activityType);
        DataSnapshot activitySnap = await activityRef.GetValueAsync();
        int currentActivityCount = activitySnap.Exists ? Convert.ToInt32(activitySnap.Value) : 0;
        await activityRef.SetValueAsync(currentActivityCount + 1);

        var badgeScoreRef = dbRef.Child("users").Child(SetCurrentUserId()).Child("dailyBadges").Child(badgeType).Child("currentScore");
        DataSnapshot scoreSnap = await badgeScoreRef.GetValueAsync();
        int currentScore = scoreSnap.Exists ? Convert.ToInt32(scoreSnap.Value) : 0;
        int updatedScore = currentScore + 1;
        await badgeScoreRef.SetValueAsync(updatedScore);

        var badgeGoalRef = dbRef.Child("users").Child(SetCurrentUserId()).Child("dailyBadges").Child(badgeType).Child("badgeGoal");
        DataSnapshot goalSnap = await badgeGoalRef.GetValueAsync();
        int badgeGoal = goalSnap.Exists ? Convert.ToInt32(goalSnap.Value) : int.MaxValue;

        if (updatedScore >= badgeGoal)
        {
            var completedRef = dbRef.Child("users").Child(SetCurrentUserId()).Child("dailyBadges").Child("badgesCompleted");
            DataSnapshot completedSnap = await completedRef.GetValueAsync();
            int completedCount = completedSnap.Exists ? Convert.ToInt32(completedSnap.Value) : 0;
            await completedRef.SetValueAsync(completedCount + 1);
        }
    }

    /// <summary>
    /// Resets badge points every 24 hours
    /// </summary>
    public void CheckResetBadge()
    {
        GetLastResetDate((lastResetDate) =>
        {
            var lastDateTime = DateTimeOffset.FromUnixTimeSeconds(lastResetDate).DateTime;
            var now = DateTime.Now;

            if ((now - lastDateTime).TotalSeconds >= 86400)
            {
                // Reset logic
                var dailyBadgesPath = dbRef.Child(currentUserId).Child("dailyBadges");

                IndivBadges indivBadges = new IndivBadges(0, 0);
                dailyBadgesPath.Child("bookTrackerToday").SetRawJsonValueAsync(JsonUtility.ToJson(indivBadges));
                dailyBadgesPath.Child("pointTrackerToday").SetRawJsonValueAsync(JsonUtility.ToJson(indivBadges));
                dailyBadgesPath.Child("signTrackerToday").SetRawJsonValueAsync(JsonUtility.ToJson(indivBadges));
                dailyBadgesPath.Child("wordTrackerToday").SetRawJsonValueAsync(JsonUtility.ToJson(indivBadges));

                // Store current day points
                dbRef.Child(currentUserId).Child("dailyBadges").Child("pointTrackerToday").Child("currentScore").GetValueAsync().ContinueWithOnMainThread(scoreTask =>
                {
                    if (scoreTask.IsCompleted && scoreTask.Result.Exists)
                    {
                        Dictionary<string, object> updatedDetails = new Dictionary<string, object>
                        {
                            ["dayPoints"] = scoreTask.Result.Value
                        };
                        dbRef.Child("users").Child(currentUserId).Child("pointDetails").Child(DateTime.Now.ToString()).UpdateChildrenAsync(updatedDetails);
                    }
                });

                // Update last reset date
                long newResetTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                dailyBadgesPath.Child("lastReset").SetValueAsync(newResetTimestamp);
            }
        });
    }

    /// <summary>
    /// Checks the last reset date 
    /// </summary>
    /// <param name="callback"></param>
    public void GetLastResetDate(Action<int> callback)
    {
        dbRef.Child("users").Child(SetCurrentUserId()).Child("dailyBadges").Child("lastReset").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log($"Failed to fetch user data for {SetCurrentUserId()}: {task.Exception}");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists && int.TryParse(snapshot.Value.ToString(), out int goal))
            {
                callback.Invoke(goal);
            }
            else
            {
                Debug.Log($"Last reset not found for {SetCurrentUserId()}.");
            }
        });
    }
}
