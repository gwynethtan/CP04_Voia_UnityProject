/*
 * Author: Jacie Thoo Yixuan
 * Date: 3/6/2025
 * Description: This Script handles the scene capture 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Features.Meta;

public class SceneCapture : MonoBehaviour
{
    /// <summary>
    /// Starts scene capture on MR
    /// </summary>
    public void StartSceneCapture()
    {
        // Get reference to arsession
        var arSession = Object.FindAnyObjectByType<ARSession>();

        if (arSession != null)
        {
            // Access scene capture API
            var success = (arSession.subsystem as MetaOpenXRSessionSubsystem)
                .TryRequestSceneCapture();

            Debug.Log("Scene capture success.");
        }
        else
        {
            Debug.Log("Scene capture failed.");
        }
    }
}
