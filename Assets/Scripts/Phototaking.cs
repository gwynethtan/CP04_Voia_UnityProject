/*
 * Author: Jacie Thoo Yixuan
 * Date: 30/7/2025
 * Description: This Script handles the photo taking functions 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Phototaking : MonoBehaviour
{
    /// <summary>
    /// Supabase link
    /// </summary>
    public string supabaseUrl = "https://kpilsdibrzlotjpnhvyk.supabase.co";

    /// <summary>
    /// Supabase anon key (public)
    /// </summary>
    public string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtwaWxzZGlicnpsb3RqcG5odnlrIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA5MzQ1MjEsImV4cCI6MjA2NjUxMDUyMX0.5QxItKV3Bzy1V2Fm3A4ctQ4NOSXNrYcYKvapUarjFFE";

    /// <summary>
    /// Bucket name for where the images are stored
    /// </summary>
    public string bucketName = "media";

    /// <summary>
    /// Folder name (Local)
    /// </summary>
    private const string CAMERA_FOLDER = "Photos";

    /// <summary>
    /// Folder name (in supabase bucket)
    /// </summary>
    private const string UPLOAD_FOLDER = "images";

    /// <summary>
    /// Path for saving image (local)
    /// </summary>
    private string photosFolder;

    [SerializeField]
    /// <summary>
    /// Camera (viewfinder)
    /// </summary>
    private Camera renderCamera = null;

    [SerializeField]
    /// <summary>
    /// Camera (viewfinder)
    /// </summary>
    private Camera mainCam = null;

    /// <summary>
    /// Reference to database script
    /// </summary>
    public UserDataManager userDataManager;

    /// <summary>
    /// Turns off camera at the start, gets folder for saving photos
    /// </summary>
    private void Start()
    {
       // renderCamera = renderCamera = Camera.main;
        //CreateRenderTexture();

        photosFolder = Path.Combine(Application.persistentDataPath, "Photos");
        if (!Directory.Exists(photosFolder))
        {
            Directory.CreateDirectory(photosFolder);
            Debug.Log($"Created folder: {photosFolder}");
        }
    }

    /// <summary>
    /// Create render texture
    /// </summary>
    private void CreateRenderTexture(Camera camera)
    {
        RenderTexture newTexture = new RenderTexture(640, 360, 32, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
        newTexture.antiAliasing = 4;

        camera.targetTexture = newTexture;
    }

    /// <summary>
    /// Takes photo, to be assigned in inspector
    /// </summary>
    public void NewTakePhoto()
    {
        renderCamera.enabled = true;
        renderCamera.CopyFrom(mainCam);
        StartCoroutine(TakePhotoCoroutine());
    }

    /// <summary>
    /// Wait before taking photo
    /// </summary>
    /// <returns></returns>
    private IEnumerator TakePhotoCoroutine()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        TakePhoto();
    }

    /// <summary>
    /// Takes photo with the camera in the scene
    /// </summary>
    public async void TakePhoto()
    {
        // Find and assign renderCamera if needed (same as before)
        if (renderCamera == null)
        {
            renderCamera.CopyFrom(Camera.main);
            if (renderCamera == null)
            {
                Debug.LogError("TakePhoto: No camera found.");
                return;
            }
            CreateRenderTexture(renderCamera);
        }

        // Make sure photosFolder is initialized
        if (string.IsNullOrEmpty(photosFolder))
        {
            photosFolder = Path.Combine(Application.persistentDataPath, "Photos");

            if (!Directory.Exists(photosFolder))
            {
                Directory.CreateDirectory(photosFolder);
                Debug.Log($"Created folder: {photosFolder}");
            }
        }

        // Capture
        Texture2D photoTexture = RenderCameraToTexture(renderCamera);
        if (photoTexture == null)
        {
            Debug.LogError("TakePhoto: photoTexture is null.");
            return;
        }

        Debug.Log("TakePhoto: Photo taken successfully.");
        await SaveAndUploadPhoto(photoTexture);

        await Task.Yield();
    }


    /// <summary>
    /// Save photo to local storage and upload to supabase
    /// From DDA Wk14 slides
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    private async Task SaveAndUploadPhoto(Texture2D texture)
    {
        string fileName = $"Photo_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        string filePath = Path.Combine(photosFolder, fileName);

        // Save as png file
        byte[] imageData = texture.EncodeToPNG();
        await File.WriteAllBytesAsync(filePath, imageData);
        Debug.Log($"Polaroid photo saved to: {filePath}");

        // Upload to Supabase
        await UploadFileUsingPost(filePath);
        await Task.Yield();
    }

    /// <summary>
    /// Upload file to supabase
    /// From DDA Wk14 slides
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public async Task UploadFileUsingPost(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File does not exist: {filePath}");
            return;
        }
        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);
        string uploadUrl = $"{supabaseUrl}/storage/v1/object/{bucketName}/{UPLOAD_FOLDER}/{fileName}";
        Debug.Log("Upload URL: " + uploadUrl);
        userDataManager.AddImage(uploadUrl);


        try
        {
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", fileData, fileName, "image/png");
            using (UnityWebRequest request = UnityWebRequest.Post(uploadUrl, form))
            {
                request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"File uploaded successfully: {fileName}");

                    // Public url
                    //string publicUrl = $"{supabaseUrl}/storage/v1/object/{bucketName}/{UPLOAD_FOLDER}/{fileName}";
                    string publicUrl = $"{supabaseUrl}/storage/v1/object/{bucketName}/{UPLOAD_FOLDER}/{fileName}";
                    Debug.Log(publicUrl);

                    // Save url to supabase
                    //myDatabase.AddImage(publicUrl);
                    renderCamera.enabled = false;
                }
                else
                {
                    Debug.LogError($"Upload failed: {request.error}");
                    Debug.LogError($"Response: {request.downloadHandler.text}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error uploading file: {ex.Message}");
        }
        await Task.Yield();
    }

    /// <summary>
    /// Renders whatever the camera in the scene shows as Texture2D
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    private Texture2D RenderCameraToTexture(Camera camera)
    {
        CreateRenderTexture(camera);
        camera.Render();
        RenderTexture.active = camera.targetTexture;

        Texture2D photo = new Texture2D(640, 360, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, 640, 360), 0, 0);
        photo.Apply();

        return photo;
    }
}
