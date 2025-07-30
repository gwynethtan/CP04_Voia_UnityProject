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
using Firebase;
using System;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    private FirebaseAuth auth;

    // Sign-up validation messages
    public TMP_Text signupUserErrorText;
    public TMP_Text signupEmailErrorText;
    public TMP_Text signupPasswErrorText;

    // Login error messages
    public TMP_Text loginAccErrorText;
    public TMP_Text loginPassErrorText;

    public TMP_InputField LogInUserInput;
    public TMP_InputField LogInPassInput;
    public TMP_InputField SignUpUserInput;
    public TMP_InputField SignUpEmailInput;
    public TMP_InputField SignUpPasswordInput;

    public Button LogInBtn;
    public Button SignUpBtn;

    public GameObject LogInCanvas;
    public GameObject SignUpCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        auth = FirebaseAuth.DefaultInstance;
    }

    public void OnLogIn()
    {
        string email = LogInUserInput.text.Trim();
        string password = LogInPassInput.text.Trim();
        LogIn(email, password);
    }

    private void LogIn(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            ShowLoginAccountError("Email cannot be empty!");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowLoginPasswordError("Password cannot be empty!");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                bool shownError = false;

                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    if (exception is FirebaseException firebaseEx)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        Debug.Log($"Login error: {errorCode}");

                        switch (errorCode)
                        {
                            case AuthError.UserNotFound:
                                ShowLoginAccountError("Account does not exist.");
                                shownError = true;
                                break;

                            case AuthError.WrongPassword:
                                ShowLoginAccountError("Incorrect email or password!");
                                shownError = true;
                                break;

                            case AuthError.InvalidEmail:
                                ShowLoginAccountError("Invalid email format.");
                                shownError = true;
                                break;
                        }
                    }
                }

                if (!shownError)
                {
                    Debug.Log("Unknown login error. Showing fallback message.");
                    ShowLoginAccountError("Incorrect Email or Password.");
                }

                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("User logged in successfully: " + user.Email);
        });
    }


    public void OnSignUp()
    {
        string username = SignUpUserInput.text.Trim();
        string email = SignUpEmailInput.text.Trim();
        string password = SignUpPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            ShowSignupUsernameError("Username cannot be empty!");
            return;
        }

        if (string.IsNullOrEmpty(email))
        {
            ShowSignupEmailError("Email cannot be empty!");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowSignupPasswordError("Password cannot be empty!");
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
                Debug.Log("Failed to create account.");
                ShowSignupEmailError("Something went wrong during sign-up.");
                return;
            }

            FirebaseUser newPlayer = task.Result.User;
            Debug.Log($"User account created successfully: {newPlayer.Email}");
            long currentTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            UserDataManager.Instance.SaveInitialUserData(newPlayer.UserId, email, username, currentTimestamp, true);
        });
    }

    // === Error Message Helpers ===

    private void ShowLoginAccountError(string message)
    {
        loginAccErrorText.text = message;
        loginAccErrorText.gameObject.SetActive(true);
        Invoke("HideLoginAccountError", 2f);
    }

    private void ShowLoginPasswordError(string message)
    {
        loginPassErrorText.text = message;
        loginPassErrorText.gameObject.SetActive(true);
        Invoke("HideLoginPasswordError", 2f);
    }

    private void HideLoginAccountError()
    {
        loginAccErrorText.gameObject.SetActive(false);
    }

    private void HideLoginPasswordError()
    {
        loginPassErrorText.gameObject.SetActive(false);
    }

    private void ShowSignupUsernameError(string message)
    {
        signupUserErrorText.text = message;
        signupUserErrorText.gameObject.SetActive(true);
        Invoke("HideSignupUsernameError", 2f);
    }

    private void ShowSignupEmailError(string message)
    {
        signupEmailErrorText.text = message;
        signupEmailErrorText.gameObject.SetActive(true);
        Invoke("HideSignupEmailError", 2f);
    }

    private void ShowSignupPasswordError(string message)
    {
        signupPasswErrorText.text = message;
        signupPasswErrorText.gameObject.SetActive(true);
        Invoke("HideSignupPasswordError", 2f);
    }

    private void HideSignupUsernameError()
    {
        signupUserErrorText.gameObject.SetActive(false);
    }

    private void HideSignupEmailError()
    {
        signupEmailErrorText.gameObject.SetActive(false);
    }

    private void HideSignupPasswordError()
    {
        signupPasswErrorText.gameObject.SetActive(false);
    }
}
