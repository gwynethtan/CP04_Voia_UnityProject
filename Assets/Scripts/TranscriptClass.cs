/*
 * Author: Tan Ting Yu Gwyneth
 * Date: 14/5/2025
 * Description: This script stores classes for everything related to the notifications
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Represents a single transcription alternative returned by the Google Speech-to-Text API.
/// </summary>
[System.Serializable]
public class Alternative
{
    public string transcript;
    public float confidence;
}

/// <summary>
/// Represents one result block containing one or more transcription alternatives.
/// </summary>
[System.Serializable]
public class Result
{
    public List<Alternative> alternatives;
    public string resultEndTime;
    public string languageCode;
}

/// <summary>
/// Represents the full response from the Google Speech-to-Text API.
/// </summary>
[System.Serializable]
public class TranscriptionResponse
{
    public List<Result> results;
    public string totalBilledTime;
    public string requestId;
}


/// <summary>
/// Represents a UI group for showing a speech bubble with text.
/// </summary>
[System.Serializable]
public class BubbleGroup
{
    public GameObject bubbleBg;
    public TMP_Text bubbleText;
    public bool bubbleActive=false;
}
