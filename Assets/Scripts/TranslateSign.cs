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
    /// Reference to text to speech script
    /// </summary>
    public TextToSpeech textToSpeech;

    /// <summary>
    /// Reference to sign language bubble ui 
    /// </summary>
    public BubbleGroup signLanguageBubbleGroup;

    /// <summary>
    /// Reference to sign language bubble error ui 
    /// </summary>
    public BubbleGroup signLanguageErrorBubbleGroup;

    /// <summary>
    /// Reference to bubbleMgr script
    /// </summary>
    public BubbleMgr bubbleMgr;

    /// <summary>
    /// String representation of the full signed input
    /// </summary>
    private StringBuilder fullSignedText = new StringBuilder();

    /// <summary>
    /// Reference to the last time sign language was used by user
    /// </summary>
    private float lastSignTime = 0f;

    /// <summary>
    /// Check if user was previously signing
    /// </summary>
    private bool wasSigning = false;

    /// <summary>
    /// String containing current signed word
    /// </summary>
    private string signedWord;

    /// <summary>
    /// Reference to SignDanceManager script
    /// </summary>
    public SignDanceManager signDanceManager;

    /// <summary>
    /// Reference to Storybook script
    /// </summary>
    public Storybook storybook;

    public UserDataManager userDataManager;
    /// <summary>
    /// Check if user have been signing 
    /// </summary>
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

    /// <summary>
    /// Converts sign into text and into speech. Displays UI once done.
    /// </summary>
    private void SignEnd()
    {
        Debug.Log("Sign Pause detected.");
        CombineLetterInList();
        Debug.Log("Full sentence: " + fullSignedText.ToString());
        signedWord = fullSignedText.ToString().Trim();

        if (fullSignedText.ToString() != "")
        {
            bubbleMgr.HandleBubbleDismissal(signLanguageBubbleGroup);
            StartCoroutine(textToSpeech.Speak(signedWord));

            // Trigger signing names for page index 6
            signDanceManager.OnWordSigned(signedWord);

            // Call delay before signing current page
            StartCoroutine(DelayedSignCurrentPage());

            fullSignedText.Clear();
        }
    }

    /// <summary>
    /// Combines all the letters into a word 
    /// </summary>
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
            userDataManager.UpdateIndivBadges(1, "signTrackerToday", "totalSignedWords");
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
            // adds single words into a list 
            letterList.Add(sign);
            fullSignedText.Append(sign);
        }
        else
        {
            CombineLetterInList();
            sentenceList.Add(sign);
            fullSignedText.Append(sign + " ");
            userDataManager.UpdateIndivBadges(1, "signTrackerToday", "totalSignedWords");
        }

        // Update the UI display
        StartCoroutine(bubbleMgr.ActivateBubble(signLanguageBubbleGroup, fullSignedText.ToString(),false));
    }

    /// <summary>
    /// Returns currently detected signed word as string
    /// </summary>
    /// <returns></returns>
    public string GetSignedWord()
    {
        return signedWord;
    }

    /// <summary>
    /// Delays signing current page to allow enough time to get signedWord
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedSignCurrentPage()
    {
        yield return new WaitForSeconds(2f);
        storybook.SignCurrentPage();

        if (!storybook.storybookCompleted)
        {
            storybook.CheckStorybookCompleted();
        }
    }

}
