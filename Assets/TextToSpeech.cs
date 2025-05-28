using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System;

public class TextToSpeech : MonoBehaviour
{
    private string apiKey = "AIzaSyD21NcemBDWzghzW3hRq-u6-iZWR3mc1SE";
    public AudioSource audioSource;

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
        }
        else
        {
            Debug.Log("TTS Error: " + www.error);
        }
    }

    [System.Serializable]
    public class TTSResponse
    {
        public string audioContent;
    }
}
