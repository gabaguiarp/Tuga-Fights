using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtils
{
    static Dictionary<float, WaitForSeconds> WaitTimes = new Dictionary<float, WaitForSeconds>();
    static Dictionary<float, WaitForSecondsRealtime> WaitRealtimes = new Dictionary<float, WaitForSecondsRealtime>();
    static WaitForEndOfFrame s_WaitOneFrame= new WaitForEndOfFrame();
    static WaitForFixedUpdate s_WaitForPhysics = new WaitForFixedUpdate();

    public static WaitForEndOfFrame WaitOneFrame => s_WaitOneFrame;
    public static WaitForFixedUpdate WaitForPhysics => s_WaitForPhysics;

    public static WaitForSeconds GetWaitTime(float seconds)
    {
        if (!WaitTimes.ContainsKey(seconds))
            WaitTimes.Add(seconds, new WaitForSeconds(seconds));

        return WaitTimes[seconds];
    }

    public static WaitForSecondsRealtime GetWaitRealtime(float seconds)
    {
        if (!WaitRealtimes.ContainsKey(seconds))
            WaitRealtimes.Add(seconds, new WaitForSecondsRealtime(seconds));

        return WaitRealtimes[seconds];
    }
}
