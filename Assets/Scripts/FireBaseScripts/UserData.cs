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
    public long dateCreated;
    public bool userOnline;

    public UserData(string email, string username, long dateCreated, bool userOnline)
    {
        this.email = email;
        this.username = username;
        this.dateCreated = dateCreated;
        this.userOnline = userOnline;
    }
}
