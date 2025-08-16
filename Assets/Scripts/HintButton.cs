using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using TMPro;

public class HintButton : MonoBehaviour
{
    public PageManager pageManager;
    public VideoPlayer videoPlayer;
    public GameObject hintCanvas;

    /// <summary>
    /// Base URL from supabase
    /// </summary>
    private string baseUrl = "https://kpilsdibrzlotjpnhvyk.supabase.co/storage/v1/object/public/media/signVideos/";

    /// <summary>
    /// Dictionary with video file names for each page
    /// </summary>
    private Dictionary<int, string> videoFiles = new Dictionary<int, string>()
    {
        { 1, "wolf.mp4" },
        { 2, "sing.mp4" },
        { 3, "talking.mp4"},
        { 4, "rain.mp4" },
        { 5, "climb.mp4" },
        { 6, "dance.mp4" },
        { 7, "heart.mp4" },
        { 8, "happy.mp4" }
    };

    void Start()
    {
        if (videoPlayer != null)
        {
            // HIDE HINT CANVAS ONCE VID ENDS
            videoPlayer.loopPointReached += HideHintCanvas;
        }

        if (hintCanvas != null)
        {
            hintCanvas.SetActive(false);
        }
    }

    public void ShowHint()
    {
        int currentPage = pageManager.CurrentPage;
        Debug.Log("Hint Button pressed");

        if (!videoFiles.TryGetValue(currentPage, out string fileName))
        {
            Debug.Log("No hint video");
            return;
        }

        string fullUrl = baseUrl + fileName;
        Debug.Log(fullUrl);
        StartCoroutine(PlayVideo(fullUrl));
    }

    private IEnumerator PlayVideo(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            videoPlayer.url = url;
            hintCanvas.SetActive(true);
            videoPlayer.Play();
            Debug.Log("Playing hint video");
        }
    }

    private void HideHintCanvas(VideoPlayer vp)
    {
        hintCanvas.SetActive(false);
        videoPlayer.Stop();
    }
}
