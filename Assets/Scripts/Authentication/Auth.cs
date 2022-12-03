using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class Auth : MonoBehaviour
{
    internal static Action OnAuthenticated;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        SetupServices();

        await SignInAnonymouslyAsync();

        OnAuthenticated?.Invoke();
    }

    private void SetupServices() 
    {
        AuthenticationService.Instance.SignedIn += () => 
        {
            Debug.Log($"Player was signed in {AuthenticationService.Instance.PlayerId}");
        };

        AuthenticationService.Instance.SignInFailed += (error) => 
        {
            Debug.LogError(error);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log($"Player was signed out {AuthenticationService.Instance.PlayerId}");
        };
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
        }
        catch (Exception ex)
        {
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
