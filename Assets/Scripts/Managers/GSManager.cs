using UnityEngine;
using EasyMobile;
using System.Collections.Generic;
using System.Collections;

public class GSManager : MonoBehaviour 
{
    public string screenshotPath;

    private void Awake() 
    {
        GameServices.ManagedInit();
    }

    void OnEnable()
    {
        GameServices.UserLoginSucceeded += OnUserLoginSucceeded;
        GameServices.UserLoginFailed += OnUserLoginFailed;
    }

    // Unsubscribe
    void OnDisable()
    {
        GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
        GameServices.UserLoginFailed -= OnUserLoginFailed;
    }

    // Event handlers
    void OnUserLoginSucceeded()
    {
        Debug.Log("User logged in successfully.");
    }

    void OnUserLoginFailed()
    {
        Debug.Log("User login failed.");
    }

	public void ShowLeaderBoardUI()
	{
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }
        else
        {
		#if UNITY_ANDROID
					GameServices.Init();    // start a new initialization process
		#elif UNITY_IOS
			Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
		#endif
        }
	}

	public void ShowAchievementsUI()
	{
        if (GameServices.IsInitialized())
        {
            GameServices.ShowAchievementsUI();
        }
        else
        {
		#if UNITY_ANDROID
			GameServices.Init();    // start a new initialization process
		#elif UNITY_IOS
					Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
		#endif
        }
	}

	public void UnlockAchievement(string name)
	{
        if (GameServices.IsInitialized())
		{
            Debug.Log("Unlocking: " + name);
            // GameServices.RevealAchievement(name);
            GameServices.UnlockAchievement(name);
		}
        else
        {
		#if UNITY_ANDROID
					GameServices.Init();    // start a new initialization process
		#elif UNITY_IOS
					Debug.Log("Cannot unlock achievement: The user is not logged in to Game Center.");
		#endif
        }
	}

    public void ReportToLeaderboard(string name, int score)
    {

        if (GameServices.IsInitialized())
        {
            Debug.Log("Reporting Score: " + name);
            // GameServices.RevealAchievement(name);
            GameServices.ReportScore(score, name);
        }
        else
        {
        #if UNITY_ANDROID
                            GameServices.Init();    // start a new initialization process
        #elif UNITY_IOS
                    Debug.Log("Cannot report score: The user is not logged in to Game Center.");
        #endif
        }
    }

    public void ShareWithScreenShot()
    {
        StartCoroutine(SaveScreenshot());
        Sharing.ShareImage(screenshotPath, "Can you beat my score? Play Even Monkeys Can Count!");
    }

    public void Share()
    {
        #if UNITY_ANDROID
            Sharing.ShareText("Play a fun minimalist puzzle game. Even monkeys can count available for IOS & Android");
        #elif UNITY_IOS
            Sharing.ShareText("Play a fun minimalist puzzle game. Even monkeys can count available for IOS & Android");
        #endif
    }

    IEnumerator SaveScreenshot()
    {
        // Wait until the end of frame
        yield return new WaitForEndOfFrame();

        // The SaveScreenshot() method returns the path of the saved image
        // The provided file name will be added a ".png" extension automatically
        screenshotPath = Sharing.SaveScreenshot("screenshot");
    }
}