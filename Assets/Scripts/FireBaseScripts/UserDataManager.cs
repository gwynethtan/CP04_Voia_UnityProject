/*
 * Author: Hoo Ying Qi Praise
 * Date: 
 * Description: 
 * This Script handles saving and retrieving user data from Firebase Realtime Database
 */

using System;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

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

    public void SaveInitialUserData(string userId, string email, string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            username = "User_" + userId;
        }

        UserData userData = new UserData(email, username);
        SaveUserData(userId, userData);
    }

    private void SaveUserData(string userId, UserData userData)
    {
        string json = JsonUtility.ToJson(userData);
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json)
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

    public void GetUserData(string userId, Action<UserData> callback)
    {
        dbRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log($"Failed to fetch user data for {userId}: {task.Exception}");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                UserData userData = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
                callback?.Invoke(userData);
            }
            else
            {
                Debug.Log($"No data found for user {userId}.");
            }
        });
    }
}
