using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Features.Meta;

public class SceneCapture : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get reference to arsession
        var arSession = Object.FindAnyObjectByType<ARSession>();

        // Access scene cpature API
        var success = (arSession.subsystem as MetaOpenXRSessionSubsystem)
            .TryRequestSceneCapture();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
