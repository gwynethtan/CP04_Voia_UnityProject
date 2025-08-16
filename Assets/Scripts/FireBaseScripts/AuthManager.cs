/*
 * Author: Hoo Ying Qi Praise
 * Date: 12/7/2025
 * Description: Handles user authentication (sign-up, login, logout) and navigation between login, signup, and main content panels.
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
    /// <summary>
    /// Singleton instance for global access
    /// </summary>
    public static AuthManager Instance { get; private set; }

    /// <summary>
    /// Firebase Authentication reference
    /// </summary>
    private FirebaseAuth auth;

    // Sign-up validation messages
    public TMP_Text signupUserErrorText;
    public TMP_Text signupEmailErrorText;
    public TMP_Text signupPasswErrorText;
    public TMP_Text signupCompleteText;

    // Login error messages 
    public TMP_Text loginAccErrorText;
    public TMP_Text loginPassErrorText;

    // Input fields for login and signup
    public TMP_InputField LogInUserInput;
    public TMP_InputField LogInPassInput;
    public TMP_InputField SignUpUserInput;
    public TMP_InputField SignUpEmailInput;
    public TMP_InputField SignUpPasswordInput;

    // UI Panels and Buttons
    public GameObject logInPanel;
    public GameObject logOutPanel;
    public Button LogInBtn;
    public Button SignUpBtn;
    public GameObject LogInCanvas;
    public GameObject SignUpCanvas;
    public GameObject bookCanvas;

    /// <summary>
    /// Stores the currently logged-in user's ID
    /// </summary>
    public string currentUserId;

    /// <summary>
    /// Reference to UserDataManager for database interactions
    /// </summary>
    public UserDataManager userDataManager;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize FirebaseAuth
        auth = FirebaseAuth.DefaultInstance;
    }

    private void Start()
    {
        // Ensure Firebase dependencies are resolved
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase Auth initialized successfully.");

                // Subscribe to authentication state changes
                auth.StateChanged += AuthOnStateChanged;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    /// <summary>
    /// Triggered whenever Firebase Auth state changes
    /// Updates current user ID and checks daily badge reset
    /// </summary>
    private void AuthOnStateChanged(object sender, EventArgs e)
    {
        Debug.Log("Auth state changed");

        if (auth.CurrentUser == null || !auth.CurrentUser.IsValid())
        {
            Debug.Log("User Not Authenticated");
            currentUserId = "";
        }
        else
        {
            Debug.Log("Current User is: " + auth.CurrentUser.UserId);
            currentUserId = auth.CurrentUser.UserId;

            // Check if daily badge points need reset
            userDataManager.CheckResetBadge();
        }
    }

    /// <summary>
    /// Login Process
    /// </summary>
    public void OnLogIn()
    {
        string email = LogInUserInput.text.Trim();
        string password = LogInPassInput.text.Trim();
        LogIn(email, password);
    }

    /// <summary>
    /// Validate input fields
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    private void LogIn(string email, string password)
    {
        // LogIn filed empty error handling
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

        // Attempt Firebase login
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // Handle Firebase-specific login errors
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

                // Fallback message for unknown errors
                if (!shownError)
                {
                    Debug.Log("Unknown login error. Showing fallback message.");
                    ShowLoginAccountError("Incorrect Email or Password.");
                }
                return;
            }

            // Login success: hide login panel, show main book panel
            FirebaseUser user = task.Result.User;
            LogInCanvas.SetActive(false);
            bookCanvas.SetActive(true);
            Debug.Log("User logged in successfully: " + user.Email);
        });
    }

    /// <summary>
    /// Sign-Up Process
    /// </summary>
    public void OnSignUp()
    {
        string username = SignUpUserInput.text.Trim();
        string email = SignUpEmailInput.text.Trim();
        string password = SignUpPasswordInput.text.Trim();

        // Sign Up filed empty error handling
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

    /// <summary>
    /// Account Creation
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="username"></param>
    private void CreateUserAccount(string email, string password, string username)
    {
        // Attempt Firebase account creation
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to create account.");
                ShowSignupEmailError("Something went wrong during sign-up.");
                return;
            }

            // Account creation successful
            FirebaseUser newPlayer = task.Result.User;
            Debug.Log($"User account created successfully: {newPlayer.Email}");

            // Save initial user data to Firebase database
            long currentTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            UserDataManager.Instance.SaveInitialUserData(newPlayer.UserId, email, username, currentTimestamp, true);

            // Show signup success message
            ShowSignupComplete();
        });
    }

    /// <summary>
    /// Log Out Process
    /// </summary>
    public void Logout()
    {
        userDataManager.UpdateUserOnline(false);
        auth.SignOut();

        logOutPanel.gameObject.SetActive(false);
        logInPanel.gameObject.SetActive(true);
    }

    /// <summary>
    ///  Error message helpers for login and signup
    /// </summary>
    /// <param name="message"></param>

    private void ShowLoginAccountError(string message)
    {
        loginAccErrorText.text = message;
        loginAccErrorText.gameObject.SetActive(true);
        Invoke("HideLoginAccountError", 2f);
    }

    /// <summary>
    /// Shows the login password error for 2 seconds
    /// </summary>
    /// <param name="message">Error message to display</param>
    private void ShowLoginPasswordError(string message)
    {
        loginPassErrorText.text = message; // Set the error text
        loginPassErrorText.gameObject.SetActive(true); // Make it visible
        Invoke("HideLoginPasswordError", 2f); // Hide automatically after 2 seconds
    }

    /// <summary>
    /// Hides the login account error immediately
    /// </summary>
    private void HideLoginAccountError() => loginAccErrorText.gameObject.SetActive(false);

    /// <summary>
    /// Hides the login password error immediately
    /// </summary>
    private void HideLoginPasswordError() => loginPassErrorText.gameObject.SetActive(false);

    /// <summary>
    /// Shows the signup username error for 2 seconds
    /// </summary>
    private void ShowSignupUsernameError(string message)
    {
        signupUserErrorText.text = message;
        signupUserErrorText.gameObject.SetActive(true);
        Invoke("HideSignupUsernameError", 2f);
    }

    /// <summary>
    /// Shows the signup email error for 2 seconds
    /// </summary>
    private void ShowSignupEmailError(string message)
    {
        signupEmailErrorText.text = message;
        signupEmailErrorText.gameObject.SetActive(true);
        Invoke("HideSignupEmailError", 2f);
    }

    /// <summary>
    /// Shows the signup password error for 2 seconds
    /// </summary>
    private void ShowSignupPasswordError(string message)
    {
        signupPasswErrorText.text = message;
        signupPasswErrorText.gameObject.SetActive(true);
        Invoke("HideSignupPasswordError", 2f);
    }

    /// <summary>
    /// Hides the signup username error immediately
    /// </summary>
    private void HideSignupUsernameError() => signupUserErrorText.gameObject.SetActive(false);

    /// <summary>
    /// Hides the signup email error immediately
    /// </summary>
    private void HideSignupEmailError() => signupEmailErrorText.gameObject.SetActive(false);

    /// <summary>
    /// Hides the signup password error immediately
    /// </summary>
    private void HideSignupPasswordError() => signupPasswErrorText.gameObject.SetActive(false);


    /// <summary>
    /// Shows a signup completion message for 2 seconds
    /// </summary>
    private void ShowSignupComplete()
    {
        signupCompleteText.gameObject.SetActive(true); // Show success message
        Invoke(nameof(HideSignupComplete), 2f); // Hide automatically after 2 seconds
    }

    /// <summary>
    /// Hides the signup completion message immediately
    /// </summary>
    private void HideSignupComplete() => signupCompleteText.gameObject.SetActive(false);

}
