/*
 * Author: Hoo Ying Qi Praise
 * Date: 
 * Description: 
 * Represents user data to be stored in Firebase, with fields for email, username
 */

/// <summary>
/// Allows this class to be serialized for JSON conversion in Unity
/// </summary>
/// 
[System.Serializable]
public class UserData
{
    public string email;
    public string username;

    /// <summary>
    /// Initializes the UserData object with provided email and username.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="username"></param>
    public UserData(string email, string username)
    {
        this.email = email;
        this.username = username;
    }
}

