using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

public class TranslateSign : MonoBehaviour
{
    /// <summary>
    /// Stores individual signed letters
    /// </summary>
    private List<string> letterList = new List<string>();

    /// <summary>
    /// Stores completed words for translation
    /// </summary>
    private List<string> sentenceList = new List<string>();

    /// <summary>
    /// Text UI to display all signed input
    /// </summary>
    public TextMeshProUGUI signedOutputText;

    public TextMeshProUGUI textToSpeechError;


    public TextToSpeech textToSpeech;

    /// <summary>
    /// String representation of the full signed input
    /// </summary>
    private StringBuilder fullSignedText = new StringBuilder();

    private float lastSignTime = 0f;
    private bool wasSigning = false;

    void Start()
    {
    }

    private void Update()
    {
        if (wasSigning)
        {
            lastSignTime += Time.deltaTime;
            if (lastSignTime >= 3f)
            {
                Debug.Log("Stopped signing");
                SignEnd();
                lastSignTime = 0f;
                wasSigning = false;
            }
        }
    }

    private void SignEnd()
    {
        Debug.Log("Sign Pause detected.");
        CombineLetterInList();
        Debug.Log("Full sentence: " + fullSignedText.ToString());
        textToSpeechError.text = "No signed speech";
        if (fullSignedText.ToString() != "")
        {
            textToSpeechError.text = "Signed speech incoming";
            StartCoroutine(textToSpeech.Speak(fullSignedText.ToString()));
        }
    }

    private void CombineLetterInList()
    {
        if (letterList.Count > 0)
        {
            StringBuilder constructedWord = new StringBuilder();
            foreach (string letter in letterList)
            {
                constructedWord.Append(letter);
            }
            sentenceList.Add(constructedWord.ToString());
            letterList.Clear();
            fullSignedText.Append(" ");
        }
    }

    /// <summary>
    /// Call this when a sign (letter or word) is detected
    /// </summary>
    public void SignedWord(string sign)
    {
        lastSignTime = 0f;
        wasSigning = true;
        if (sign.Length == 1)
        {
            // Single letter — add to letter list
            letterList.Add(sign);
            fullSignedText.Append(sign);
        }
        else
        {
            CombineLetterInList();
            sentenceList.Add(sign);
            fullSignedText.Append(sign + " ");
        }
        // Update the UI display
        signedOutputText.text = fullSignedText.ToString();
    }
}
