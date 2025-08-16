/*
 * Author: Hoo Ying Qi Praise
 * Date: 12/7/2025
 * Description: Represents a user's personal details to be stored in Firebase Realtime Database.
 */

[System.Serializable] // Allows the class to be serialized into JSON for Firebase
public class UserData
{
    /// <summary>
    /// User's email address
    /// </summary>
    public string email;

    /// <summary>
    /// User's display name or username
    /// </summary>
    public string username;

    /// <summary>
    /// Timestamp of when the user account was created (Unix time in seconds)
    /// </summary>
    public long dateCreated;

    /// <summary>
    /// Indicates whether the user is currently online
    /// </summary>
    public bool userOnline;

    /// <summary>
    /// Constructor to initialize a new UserData instance
    /// </summary>
    /// <param name="email">User's email</param>
    /// <param name="username">User's username</param>
    /// <param name="dateCreated">Account creation timestamp</param>
    /// <param name="userOnline">Whether the user is online</param>
    public UserData(string email, string username, long dateCreated, bool userOnline)
    {
        this.email = email;
        this.username = username;
        this.dateCreated = dateCreated;
        this.userOnline = userOnline;
    }
}

