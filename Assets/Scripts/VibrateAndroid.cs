using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VibrateAndroid
{
    #if UNITY_ANDROID
        public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    #else
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
    #endif

    public static void Vibrate(long milliseconds)
    {
        #if UNITY_ANDROID
            //vibrator.Call("vibrator", milliseconds);
            vibrator.Call("vibrator");
        #else
            Handheld.Vibrate();
        #endif
    }

}
