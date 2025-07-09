/*
 * Author: Hoo Ying Qi Praise
 * Date: 
 * Description: 
 * This Script handles user authentication and menu navigation
 */

using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private DatabaseReference dbRef;

    public void UpdateUserData(string userId, UserData userData)
    {
        string json = JsonUtility.ToJson(userData); //Convert userData object to JSON string

        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json) //Save JSON data to the "users/userId" path in the database
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log($"Error saving user data for {userId}: {task.Exception}");
                }
                else
                {
                    Debug.Log($"User data updated successfully for {userId}!");
                }
            });
    }

    public void SaveInitialUserData(string userId, string email, string username)
    {
        if (string.IsNullOrEmpty(username)) // Assign a default username if none is provided
        {
            username = "User_" + userId; // Default username if empty
        }

        SaveUserData(userId, userData);  // Save user data to Firebase
    }

    private void SaveUserData(string userId, UserData userData)
    {
        string json = JsonUtility.ToJson(userData); // Convert userData object to JSON string

        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json); // Save JSON data to the "users/userId" path in the database
    }

    public void GetUserData(string userId, Action<UserData> callback)
    {
        // Get user data from the "users/userId" path in the database
        dbRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log($"Failed to fetch user data for {userId}: {task.Exception}");
                return;
            }

            // Check if user data exists
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                // Deserialize the user data from JSON string
                UserData userData = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
                callback?.Invoke(userData);  // Pass the user data to the callback function if it exists
            }
            else
            {
                Debug.Log($"No data found for user {userId}.");
            }
        });
    }
}
