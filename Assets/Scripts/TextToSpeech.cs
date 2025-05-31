using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System;
using TMPro;

public class TextToSpeech : MonoBehaviour
{
    private string apiKey = "AIzaSyCcVIOOl5ke4pnsPXPMdTDWZ_QQre2KO2Y";
    public AudioSource audioSource;
    public BubbleGroup TextToSpeechBubbleGroup;
    public BubbleGroup TextToSpeechErrorBubbleGroup;
    public BubbleMgr bubbleMgr;

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
            StartCoroutine(bubbleMgr.ActivateBubble(TextToSpeechBubbleGroup,text, true));
        }
        else
        {
            StartCoroutine(bubbleMgr.ActivateBubble(TextToSpeechErrorBubbleGroup, $"Error: {www.error}\nCode: {www.responseCode}\n{www.downloadHandler.text}",true));
        }
    }

    [System.Serializable]
    public class TTSResponse
    {
        public string audioContent;
    }
}
