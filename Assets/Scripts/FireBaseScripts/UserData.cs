/*
 * Author: Hoo Ying Qi Praise
 * Date: 12/7/2025
 * Description: Represents user personal details to be stored in Firebase
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
