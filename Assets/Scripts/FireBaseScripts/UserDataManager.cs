/*
 * Author: Hoo Ying Qi Praise and Tan Ting Yu Gwyneth
 * Date: 14 July 2025
 * Description: 
 * This Script handles saving and retrieving user data from Firebase Realtime Database
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance { get; private set; }
    private DatabaseReference dbRef;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveInitialUserData(string userId, string email, string username,string dateCreated, string userOnline)
    {
        if (string.IsNullOrEmpty(username))
        {
            username = userId;
        }

        Debug.Log("Creating player");

        // New data created for it to be pushed into database later
        UserData userData = new UserData(email, username, dateCreated, userOnline);
        ActivityDetails activityDetails = new ActivityDetails(0, 0, 0, 0);
        DailyBadges dailyBadges = new DailyBadges(dateCreated, 0);
        IndivBadges indivBadges = new IndivBadges(0, 0);

        // Generate unique paths
        var playerPath = dbRef.Child(userId);
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

        Dictionary<string, object> updatedDetails = new Dictionary<string, object>
        {
            ["points"] = 0
        };
        playerPath.UpdateChildrenAsync(updatedDetails);
    }

    public async void SettleIndivBadgesAsync(string currentUserId, int points, string badgeType, string activityType)
    {
        var pointsRef = dbRef.Child("users").Child(currentUserId).Child("points");
        DataSnapshot pointsSnap = await pointsRef.GetValueAsync();
        int currentPoints = pointsSnap.Exists ? Convert.ToInt32(pointsSnap.Value) : 0;
        await pointsRef.SetValueAsync(currentPoints + points);

        var activityRef = dbRef.Child("users").Child(currentUserId).Child("activityDetails").Child(activityType);
        DataSnapshot activitySnap = await activityRef.GetValueAsync();
        int currentActivityCount = activitySnap.Exists ? Convert.ToInt32(activitySnap.Value) : 0;
        await activityRef.SetValueAsync(currentActivityCount + 1);

        var badgeScoreRef = dbRef.Child("users").Child(currentUserId).Child(badgeType).Child("currentScore");
        DataSnapshot scoreSnap = await badgeScoreRef.GetValueAsync();
        int currentScore = scoreSnap.Exists ? Convert.ToInt32(scoreSnap.Value) : 0;
        int updatedScore = currentScore + 1;
        await badgeScoreRef.SetValueAsync(updatedScore);

        var badgeGoalRef = dbRef.Child("users").Child(currentUserId).Child(badgeType).Child("badgeGoal");
        DataSnapshot goalSnap = await badgeGoalRef.GetValueAsync();
        int badgeGoal = goalSnap.Exists ? Convert.ToInt32(goalSnap.Value) : int.MaxValue;

        if (updatedScore >= badgeGoal)
        {
            var completedRef = dbRef.Child("users").Child(currentUserId).Child("dailyBadges").Child("badgesCompleted");
            DataSnapshot completedSnap = await completedRef.GetValueAsync();
            int completedCount = completedSnap.Exists ? Convert.ToInt32(completedSnap.Value) : 0;
            await completedRef.SetValueAsync(completedCount + 1);
        }
    }


    //need check if need reset badge 
    public void CheckResetBadge(string currentUserId)
    {
        GetLastResetDate(currentUserId, (lastResetDate) =>
        {
            // Assuming lastResetDate is in Unix timestamp (seconds since epoch)
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
                        dbRef.Child("users").Child(currentUserId).Child("pointDetails").Child(DateTime.Now.ToString("yyyy-MM-dd")).UpdateChildrenAsync(updatedDetails);
                    }
                });

                // Update last reset date
                long newResetTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                dailyBadgesPath.Child("lastReset").SetValueAsync(newResetTimestamp);
            }
        });
    }

    /// <summary>
    /// Update user online status
    /// </summary>
    /// <param name="currentUserId"></param>
    /// <param name="playerOnline"></param>
    public void UpdateUserOnline(string currentUserId, bool playerOnline)
    {
        Dictionary<string, object> updatedDetails = new Dictionary<string, object>
        {
            ["playerOnline"] = playerOnline
        };
        dbRef.Child("users").Child(currentUserId).Child("userDetails").UpdateChildrenAsync(updatedDetails);
        Debug.Log("Updated playerDetails date");
    }


    public void GetLastResetDate(string userId, Action<int> callback)
    {
        dbRef.Child("users").Child(userId).Child("dailyBadges").Child("lastReset").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log($"Failed to fetch user data for {userId}: {task.Exception}");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists && int.TryParse(snapshot.Value.ToString(), out int goal))
            {
                callback.Invoke(goal);
            }
            else
            {
                Debug.Log($"No valid badgeGoal found for user {userId}.");
            }
        });
    }
}
