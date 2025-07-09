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

    public TMP_Text errorMessageText;
    public TMP_Text passText;

    public TMP_InputField UsernameInput, PasswordInput, SignUpEmailInput, EmailInput, SignUpPasswordInput, ForgotEmailInput;

    public Button LogInBtn;
    public Button SignUpBtn;
    public Button ForgetBtn;

    public GameObject LogInCanvas;
    public GameObject SignUpCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        auth = FirebaseAuth.DefaultInstance;
    }

    private void OnLogIn()
    {
        string email = UsernameInput.text.Trim();
        string password = PasswordInput.text.Trim();
        LogIn(email, password);
    }

    private void LogIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log($"Account does not exist. Error Logging In: {task.Exception}");
                DisplayErrorMessage("Account does not exist!");
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("User logged in successfully");
            // Optionally: load new scene or pull user data
        });
    }

    private void DisplayErrorMessage(string message)
    {
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
        Invoke("HideErrorMessage", 1f);
    }

    private void HideErrorMessage()
    {
        errorMessageText.gameObject.SetActive(false);
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

    private void CreateUserAccount(string email, string password, string username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log($"Error creating account: {task.Exception}");
                return;
            }

            FirebaseUser newPlayer = task.Result.User;
            Debug.Log($"User account created successfully: {newPlayer.Email}");

            // Save initial user data after successful signup
            UserDataManager.Instance.SaveInitialUserData(newPlayer.UserId, email, username);
        });
    }
}
