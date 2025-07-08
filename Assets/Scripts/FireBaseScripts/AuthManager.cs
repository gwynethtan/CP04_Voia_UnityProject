/*
 * Author: Hoo Ying Qi Praise
 * Date: 
 * Description: 
 * This Script handles user authentication and menu navigation
 */

using TMPro;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    private FirebaseAuth auth;

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
    public Button LogInBtn;
    public Button SignUpBtn;
    public Button CreateBtn;
    public Button ForgetBtn;

    private void OnLogIn()
    {
        string username = UsernameInput.text.Trim();
        string password = PasswordInput.text.Trim();
        LogIn(username, password);
    }

    private void LogIn(string username, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(username, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log($"Account does not exist. Error Logging In: {task.Exception}");

                DisplayErrorMessage("Account does not exist!");
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log($"User logged in successfully");

        });
    }
    private void DisplayErrorMessage(string message)
    {
        errorMessageText.text = message; // Set the error message text
        errorMessageText.gameObject.SetActive(true); // Show the error message UI

        // Hide the error message after 1 seconds
        Invoke("HideErrorMessage", 1f);
    }
    private void HideErrorMessage()
    {
        errorMessageText.gameObject.SetActive(false); // Hide the error message UI
    }

    private void OnSignUp()
    {
        string email = EmailInput.text.Trim();
        string username = UsernameInput.text.Trim();
        string password = PasswordInput.text.Trim();


        if (string.IsNullOrEmpty(username))
        {
            Debug.Log("Username cannot be empty!");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.Log("Password cannot be empty!");
            return;
        }

        CreateUserAccount(email, password, username);
    }

    public void CreateUserAccount(string email, string password, string username)
    {

    }
}
