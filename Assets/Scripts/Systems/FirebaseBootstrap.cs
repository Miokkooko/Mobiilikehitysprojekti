using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using System;

public class FirebaseBootstrap : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseApp app;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError("Firebase deps failed: " + task.Result);
                return;
            }

            app = FirebaseApp.DefaultInstance;

            auth = FirebaseAuth.DefaultInstance;

            Debug.Log("Firebase fully initialized");

            SignIn();
        });
    }

    void SignIn()
    {
        var existingUser = FirebaseAuth.DefaultInstance.CurrentUser;

        if (existingUser != null)
        {
            Debug.Log("Reusing existing user: " + existingUser.UserId);
            SaveManager.InitializeFirebase(existingUser.UserId);
            return;
        }

        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Anonymous sign-in failed: " + task.Exception);
                return;
            }

            if (task.IsCanceled)
            {
                Debug.LogError("Anonymous sign-in was canceled");
                return;
            }

            var user = task.Result.User;

            Debug.Log("New anonymous user: " + user.UserId);

            SaveManager.InitializeFirebase(user.UserId);
        });
    }
}