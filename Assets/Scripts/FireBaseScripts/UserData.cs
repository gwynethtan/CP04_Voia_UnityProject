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

    public UserData(string email, string username)
    {
        this.email = email;
        this.username = username;
    }
}
