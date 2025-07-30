/*
 * Author: Hoo Ying Qi Praise
 * Date: 
 * Description: 
 * Represents user data to be stored in Firebase, with fields for email, username
 */

[System.Serializable]
public class UserData
{
    public string email;
    public string username;
    public string dateCreated;
    public string userOnline;

    public UserData(string email, string username, string dateCreated, string userOnline)
    {
        this.email = email;
        this.username = username;
        this.dateCreated = dateCreated;
        this.userOnline = userOnline;
    }
}
