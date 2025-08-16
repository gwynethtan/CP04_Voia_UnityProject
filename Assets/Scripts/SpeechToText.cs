/*
 * Author: Tan Ting Yu Gwyneth
 * Date: 4/5/2025
 * Description: This script manages speech to text conversion 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using static TextToSpeech;
using TMPro;
using System;
using UnityEngine.PlayerLoop;
using JetBrains.Annotations;
public class SpeechToText : MonoBehaviour
{
    /// <summary>
    /// Google API key
    /// </summary>
    private string apiKey = "AIzaSyCcVIOOl5ke4pnsPXPMdTDWZ_QQre2KO2Y";

    /// <summary>
    /// Google API url
    /// </summary>
    private string url = "https://speech.googleapis.com/v1/speech:recognize?key=";

    /// <summary>
    /// UI for speech to text bubble 
    /// </summary>
    public BubbleGroup SpeechToTextBubbleGroup;

    /// <summary>
    /// UI for speech to text error bubble 
    /// </summary>
    public BubbleGroup SpeechToTextErrorBubbleGroup;

    /// <summary>
    /// Reference to bubble manager 
    /// </summary>
    public BubbleMgr bubbleMgr;

    /// <summary>
    /// Live audio recording clip
    /// </summary>
    private AudioClip micClip;

    /// <summary>
    /// Active microphone device name
    /// </summary>
    private string micName;

    /// <summary>
    /// Window size for calculating volume
    /// </summary>
    private int sampleWindow = 128;

    /// <summary>
    /// Minimum volume to consider as speech
    /// </summary>
    float silenceThreshold = 0.01f;

    /// <summary>
    /// Minimum continuous sound to trigger transcription
    /// </summary>
    float minSoundDuration = 1.0f;

    /// <summary>
    /// Whether user was previously speaking
    /// </summary>
    bool wasSpeaking = false;

    /// <summary>
    /// Pause timer after speech ends
    /// </summary>
    float stopSpeakingPause = 0;

    /// <summary>
    /// Duration of current speech
    /// </summary>
    float stillSpeaking=0f;

    /// <summary>
    /// Current volume
    /// </summary>
    float volume;

    /// <summary>
    /// Current microphone position
    /// </summary>
    int micPosition;

    /// <summary>
    /// Track the previous words said 
    /// </summary>
    string prevTranscript="";

    /// <summary>
    /// Gets the microphone data
    /// </summary>
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            micClip = Microphone.Start(micName, true, 10, 44100);
        }
    }

    /// <summary>
    /// Converts speech to text every 3 seconds for the live captioning effect
    /// </summary>
    private void Update()
    {
        volume = GetVolume();
        micPosition = Microphone.GetPosition(micName);
        if (volume >= silenceThreshold)
        {
            wasSpeaking = true;
            stillSpeaking += Time.deltaTime;
            stopSpeakingPause = 0;
        }
        else  
        {
            if (wasSpeaking)
            {
                stopSpeakingPause += Time.deltaTime;
                if (stopSpeakingPause >= 0.6f) // finished speaking
                {
                    GetStartEndClip(micPosition, stillSpeaking + stopSpeakingPause);
                    wasSpeaking = false;
                    stillSpeaking = 0;
                    stopSpeakingPause = 0;
                }
            }

        }
    }


    /// <summary>
    /// Determines the start and end sample points of the clip and extracts a subclip
    /// </summary>
    /// <param name="micPositionFunc"></param>
    /// <param name="soundDuration"></param>
    void GetStartEndClip(int micPositionFunc,float soundDuration)
    {
        Debug.Log("hello:" + soundDuration);
        int endSample = micPositionFunc;
        int startSample = Mathf.Max(0, endSample - (int)(micClip.frequency * soundDuration));

        if (startSample < 0) startSample = 0;

        AudioClip subClip = TrimClip(micClip, startSample, endSample);
        StartCoroutine(RecordAndSend(subClip));
    }

    /// <summary>
    /// Calculates current microphone volume using RMS
    /// </summary>
    /// <returns></returns>
    float GetVolume()
    {
        float[] samples = new float[sampleWindow];
        int micPos = Microphone.GetPosition(micName) - sampleWindow;
        if (micPos < 0) return 0;
        micClip.GetData(samples, micPos);
        float sum = 0;
        foreach (var s in samples) sum += s * s;
        return Mathf.Sqrt(sum / sampleWindow);
    }

    /// <summary>
    /// Trims a portion of the audio clip from startSample to endSample
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="startSample"></param>
    /// <param name="endSample"></param>
    /// <returns></returns>
    public static AudioClip TrimClip(AudioClip clip, int startSample, int endSample)
    {
        float[] data = new float[endSample - startSample];
        clip.GetData(data, startSample);
        AudioClip newClip = AudioClip.Create("TrimmedClip", endSample - startSample, clip.channels, clip.frequency, false);
        newClip.SetData(data, 0);
        return newClip;
    }

    /// <summary>
    /// Converts the audio clip to WAV, sends it to Google Speech API, and displays the result
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    IEnumerator RecordAndSend(AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        Debug.Log("AudioClip Sample Count: " + clip.samples * clip.channels);
        Debug.Log("AudioClip Frequency: " + clip.frequency);
        Debug.Log("AudioClip Channels: " + clip.channels);

        byte[] wavData = WavUtility.FromAudioClip(clip);
        Debug.Log("WAV Data Length: " + wavData.Length);

        if (wavData == null || wavData.Length == 0)
        {
            Debug.LogError("Error: WAV data is empty or null. Check your WavUtility or audio recording.");
            yield break; // Exit the coroutine if no WAV data
        }

        string base64Audio = System.Convert.ToBase64String(wavData);
        Debug.Log("Base64 Audio Length: " + base64Audio.Length);

        string jsonPayload = $@"{{
          'config': {{
            'encoding':'LINEAR16',
            'sampleRateHertz':{clip.frequency},
            'languageCode':'en-US'
          }},
          'audio': {{
            'content':'{base64Audio}'
          }}
        }}";

        // Calling API 
        UnityWebRequest www = new UnityWebRequest(url + apiKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Transcription: " + www.downloadHandler.text);
            string jsonResponse = www.downloadHandler.text; // Or your raw JSON string

            TranscriptionResponse transcription = JsonConvert.DeserializeObject<TranscriptionResponse>(jsonResponse);

            // Access transcript
            if (transcription.results != null && transcription.results.Count > 0)
            {
                string transcript = transcription.results[0].alternatives[0].transcript;
                Debug.Log(transcript);
                if (transcript != null && transcript!= prevTranscript)
                {
                    StartCoroutine(bubbleMgr.ActivateBubble(SpeechToTextBubbleGroup, transcript, true)); // Displays text for user
                    prevTranscript=transcript;
                }
            }
        }
        else
        {
            StartCoroutine(bubbleMgr.ActivateBubble(SpeechToTextErrorBubbleGroup, www.error, true));
            Debug.LogError("Error: " + www.error);
            Debug.LogError("Response Body: " + www.downloadHandler.text); // Log the response body if available
        }
    }
}