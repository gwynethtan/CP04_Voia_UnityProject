using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Features.Meta;

public class SceneCapture : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {

    }
}
