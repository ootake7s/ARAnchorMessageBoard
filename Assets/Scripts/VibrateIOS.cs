using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class VibrateIOS
{
    [DllImport ("__Internal")]
    private static extern void playSystemSound(int soundId);

    public static void PlaySystemSound(int soundId)
    {
        #if UNITY_IOS
        playSystemSound(soundId);
        #endif        
    }
}
