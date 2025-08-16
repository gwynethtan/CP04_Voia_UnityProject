/*
 * Author: Tan Ting Yu Gwyneth
 * Date: 4/5/2025
 * Description: This script manages notifications for the text to speech conversion
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System;
using TMPro;

public class TextToSpeech : MonoBehaviour
{
    /// <summary>
    /// Google API key
    /// </summary>
    private string apiKey = "AIzaSyCcVIOOl5ke4pnsPXPMdTDWZ_QQre2KO2Y";

    /// <summary>
    /// Output for audio
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// UI for text to speech bubble
    /// </summary>
    public BubbleGroup TextToSpeechBubbleGroup;

    /// <summary>
    /// UI for text to speech error bubble 
    /// </summary>
    public BubbleGroup TextToSpeechErrorBubbleGroup;

    /// <summary>
    /// Reference to BubbleMgr script
    /// </summary>
    public BubbleMgr bubbleMgr;

    /// <summary>
    /// Converts sign language text to into speech for hearing individuals
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public IEnumerator Speak(string text)
    {
        string url = "https://texttospeech.googleapis.com/v1/text:synthesize?key=" + apiKey;

        string jsonPayload = $@"{{
          'input':{{'text':'{text}'}},
          'voice':{{'languageCode':'en-US','ssmlGender':'NEUTRAL'}},
          'audioConfig':{{'audioEncoding':'LINEAR16'}}
        }}";

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var json = www.downloadHandler.text;
            var base64Audio = JsonUtility.FromJson<TTSResponse>(json).audioContent;
            byte[] wavBytes = Convert.FromBase64String(base64Audio);

            string wavPath = Path.Combine(Application.persistentDataPath, "tts.wav");
            File.WriteAllBytes(wavPath, wavBytes);
            Debug.Log("Saved WAV to: " + wavPath);

            AudioClip clip = WavUtility.ToAudioClip(wavBytes);
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
            StartCoroutine(bubbleMgr.ActivateBubble(TextToSpeechBubbleGroup,text, true)); // Shows users that text has been converted
        }
        else
        {
            StartCoroutine(bubbleMgr.ActivateBubble(TextToSpeechErrorBubbleGroup, $"Error: {www.error}\nCode: {www.responseCode}\n{www.downloadHandler.text}",true));
        }
    }

    /// <summary>
    /// Get audio content
    /// </summary>
    [System.Serializable]
    public class TTSResponse
    {
        public string audioContent;
    }
}
