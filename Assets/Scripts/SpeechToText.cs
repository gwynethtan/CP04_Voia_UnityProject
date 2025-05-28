using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using static TextToSpeech;
using TMPro;
using System;
using UnityEngine.PlayerLoop;
public class SpeechToText : MonoBehaviour
{
    private string apiKey = "AIzaSyCcVIOOl5ke4pnsPXPMdTDWZ_QQre2KO2Y"; // Replace with your actual API key
    private string url = "https://speech.googleapis.com/v1/speech:recognize?key=";
    public TextMeshProUGUI speechToTextError;
    /// <summary>
    /// Text UI to display text said by other individuals
    /// </summary>
    public TextMeshProUGUI saidText;

    private AudioClip micClip;
    private string micName;
    private int sampleWindow = 128;

    float silenceThreshold = 0.01f;
    float minSoundDuration = 1.0f;
    float soundStartTime = 0f;
    bool wasSpeaking = false;
    float stopSpeakingPause = 0;
    float stillSpeaking=0f;
    float volume;
    int micPosition;

    bool currentSpeaking;
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            micClip = Microphone.Start(micName, true, 10, 44100);
        }
    }

    private void Update()
    {

        volume = GetVolume();
        micPosition = Microphone.GetPosition(micName);
        if (volume >= silenceThreshold)
        {
            wasSpeaking = true;
            currentSpeaking = true;
            stillSpeaking+= Time.deltaTime;
            Debug.Log("U r talking");
            if (stopSpeakingPause > 0)
            {
                stopSpeakingPause = 0;
            }
            if (stillSpeaking >= minSoundDuration)
            {
                Debug.Log("over limit");
                GetStartEndClip(micPosition, stillSpeaking);
                stillSpeaking = 0; // to reset to ensure transcription comes every 2 sec
            }

        }
        else
        {
            if (wasSpeaking)
            {
                stopSpeakingPause += Time.deltaTime;
                if (stopSpeakingPause >= 1) //long pauses coz there are pauses in between words when speaking
                {
                    Debug.Log("slay"+ stillSpeaking);
                    wasSpeaking = false;
                    if (stillSpeaking>=1) 
                    {
                        GetStartEndClip(micPosition,stillSpeaking+stopSpeakingPause);
                        stillSpeaking = 0;
                    }
                    stopSpeakingPause = 0;
                }
                else
                {
                    stillSpeaking += Time.deltaTime; // Srill increasing till 2 sec when so stillSpeaking>0 wouldnt work 

                }
            }
        }
    }
    



    void GetStartEndClip(int micPositionFunc,float soundDuration)
    {
        Debug.Log("hello:" + soundDuration);
        int endSample = micPositionFunc;
        int startSample = Mathf.Max(0, endSample - (int)(micClip.frequency * soundDuration));

        if (startSample < 0) startSample = 0;

        AudioClip subClip = TrimClip(micClip, startSample, endSample);
        StartCoroutine(RecordAndSend(subClip));
    }

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

    public static AudioClip TrimClip(AudioClip clip, int startSample, int endSample)
    {
        float[] data = new float[endSample - startSample];
        clip.GetData(data, startSample);
        AudioClip newClip = AudioClip.Create("TrimmedClip", endSample - startSample, clip.channels, clip.frequency, false);
        newClip.SetData(data, 0);
        return newClip;
    }
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
                saidText.text = transcript;
            }


        }
        else
        {
            speechToTextError.text = www.error;
            Debug.LogError("Error: " + www.error);
            Debug.LogError("Response Body: " + www.downloadHandler.text); // Log the response body if available
        }
    }
}