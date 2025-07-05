/*
 * Author: Hoo Ying Qi Praise
 * Date: 
 * Description: 
 * This Script handles user authentication and menu navigation
 */

using TMPro;
// using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
// using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    // private FirebaseAuth auth;

    /// <summary>
    /// UI elements for displaying game stats
    /// </summary>
    public TMP_Text errorMessageText;
    public TMP_Text passText;

    /// <summary>
    /// Input fields for login, signup, and password reset
    /// </summary>
    public TMP_InputField UsernameInput, PasswordInput, SignUpEmailInput, EmailInput, SignUpPasswordInput, ForgotEmailInput;

    /// <summary>
    /// Buttons for various actions
    /// </summary>
    public Button SignUpBtn;

    private void OnLogIn()
    {
        string email = EmailInput.text.Trim();
        string password = PasswordInput.text.Trim();
        LogIn(email, password);
    }

    private void LogIn(string email, string password)
    {

    }
}
