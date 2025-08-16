/*
 * Author: Jacie Thoo Yixuan
 * Date: 9/6/2025
 * Description: This Script handles the hint button functions
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using TMPro;

public class HintButton : MonoBehaviour
{
    /// <summary>
    /// Reference to pageManager
    /// </summary>
    public PageManager pageManager;

    /// <summary>
    /// Reference to video player
    /// </summary>
    public VideoPlayer videoPlayer;

    /// <summary>
    /// Reference to hint canvas
    /// </summary>
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

    /// <summary>
    /// Sets up the video player and ensures the hint canvas is hidden at the start
    /// </summary>
    void Start()
    {
        if (videoPlayer != null)
        {
            // Hide hint canvas once video ends
            videoPlayer.loopPointReached += HideHintCanvas;
        }

        if (hintCanvas != null)
        {
            hintCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the hint video for current page
    /// </summary>
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

    /// <summary>
    /// Loads and plays hint video from Supabase URL
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Hide hint canvas and stop video when it finishes
    /// </summary>
    /// <param name="vp"></param>
    private void HideHintCanvas(VideoPlayer vp)
    {
        hintCanvas.SetActive(false);
        videoPlayer.Stop();
    }
}