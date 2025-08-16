/*
 * Author: Tan Ting Yu Gwyneth
 * Date: 2/6/2025
 * Description: This script handles logic for user movement detection
 */

using PDollarGestureRecognizer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.UI;
using static PDollarGestureRecognizer.GestureIO;
public class MovementMgr : MonoBehaviour
{
    /// <summary>
    /// Reference to translateSign script
    /// </summary>
    public TranslateSign translateSign;

    /// <summary>
    /// Determine left or right hand 
    /// </summary>
    public XRNode inputSource; 

    /// <summary>
    /// Define moving hand 
    /// </summary>
    public Transform movementSource;

    /// <summary>
    /// Display cube when moving
    /// </summary>
    public GameObject debugCubePrefab;

    /// <summary>
    /// Display debugging messages
    /// </summary>
    public TMP_Text debugDisplay;

    /// <summary>
    /// Flag to check if user creating new movement
    /// </summary>
    public Toggle createNewMovement;

    /// <summary>
    /// Pose that the user should be creating with
    /// </summary>
    public XRHandPose creationPose;

    /// <summary>
    /// Gesture name for the pose user is creating 
    /// </summary>
    public string newGestureName;

    /// <summary>
    /// Check if any significant movement made
    /// </summary>
    public float newPositionThresholdDistance = 0.05f;

    /// <summary>
    /// Accurate set of coordinates for the proper movement 
    /// </summary>
    private List<Gesture> properCoordinatesList = new List<Gesture>();

    /// <summary>
    /// Stores a list of coordinates of the users hand position 
    /// </summary>
    private List<Vector3> currentCoordinatesList = new List<Vector3>();

    /// <summary>
    /// Accuracy rate for the pose to be recognised
    /// </summary>
    public float recognitionThreshold = 0.7f;

    /// <summary>
    /// Defines what hand the manager is managing for 
    /// </summary>
    public bool isRightHand;

    /// <summary>
    /// Checks if the hand is moving
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// Checks how long the pose was still
    /// </summary>
    public float stillTime;

    /// <summary>
    /// Word for poses that has movement 
    /// </summary>
    public string currentStaticWord = "";

    /// <summary>
    /// Defines file naming to show whether the file is for the right or left hand 
    /// </summary>
    private string handName;

    /// <summary>
    /// Defines if the manager is serving left or right hand 
    /// </summary>
    private void Start()
    {
        if (isRightHand)
        {
            handName = "Right";
        }
        else
        {
            handName = "Left";
        }
    }
    /// <summary>
    /// Checks the current hand pose and starts, updates, or ends the movement based on the pose.
    /// </summary>
    private void Update()
    {
        XRHandSubsystem handSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();

        // Gets the current hand in the system 
        XRHand currentHand;

        if (inputSource == XRNode.LeftHand)
        {
            currentHand = handSubsystem.leftHand;
        }
        else
        {
            currentHand = handSubsystem.rightHand;
        }

        var handJointsUpdatedEventArgs = new XRHandJointsUpdatedEventArgs { hand = currentHand };

        // Check the current condition of the pose
        bool handPose = false;

        if (creationPose != null)
        {
            handPose = creationPose.CheckConditions(handJointsUpdatedEventArgs);
        }

        if (createNewMovement.isOn)
        {
            if (handPose && isMoving)
            {
                UpdateMovement();
            }
            else if (handPose && !isMoving)
            {
                RecordCreationMovement();
            }
            else if (!handPose && isMoving)
            {
                EndCreationMovement();
            }
        }
        else
        {
            if (isMoving && handPose)
            {
                UpdateMovement();
            }
            else if (!isMoving)
            {
                DisplayHandMovementProgress("Hand is not moving", debugDisplay);
            }
            else if (!handPose)
            {
                DisplayHandMovementProgress("Hand is not currently following the pose", debugDisplay);
            }
        }
    }

    /// <summary>
    /// Displays debugging messages
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="textBox"></param>
    public void DisplayHandMovementProgress(string progress, TMP_Text textBox)
    {
        textBox.text = progress;
    }

    /// <summary>
    /// Creates and starts tracking the movement in creation mode
    /// </summary>
    public void RecordCreationMovement()
    {
        isMoving = true;
        currentCoordinatesList.Clear();
        currentCoordinatesList.Add(movementSource.position);
        Debug.Log($"Initial movementSource position: {movementSource.position}");
        DisplayHandMovementProgress($"Initial movementSource position: {movementSource.position}", debugDisplay);
        if (debugCubePrefab)
        {
            GameObject instance = Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
            Destroy(instance, 2.0f); // Destroy after 2 seconds
        }
    }

    /// <summary>
    /// Starts the movement and begins tracking the position of the movement source.
    /// </summary>
    public void RecordMovement(string movementPoseName, XRHandPose requiredPose)
    {
        isMoving = true;
        currentCoordinatesList.Clear();
        currentCoordinatesList.Add(movementSource.position);

        // Debug the initial position of movementSource
        Debug.Log($"Initial movementSource position: {movementSource.position}");
        DisplayHandMovementProgress($"Initial movementSource position: {movementSource.position}", debugDisplay);
        if (debugCubePrefab)
        {
            DisplayHandMovementProgress("Cube produced start", debugDisplay);
            DisplayHandMovementProgress(movementPoseName, debugDisplay);
            GameObject instance = Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
            Destroy(instance, 2.0f); // Destroy after 2 seconds
        }
    }

    /// <summary>
    /// Updates the movement by adding new positions when the movement source moves a significant distance.
    /// </summary>
    public void UpdateMovement()
    {
        if (currentCoordinatesList.Count == 0)
        {
            DisplayHandMovementProgress(currentCoordinatesList.Count.ToString(), debugDisplay);
        }

        Vector3 lastPosition = currentCoordinatesList[currentCoordinatesList.Count - 1];
        float distance = Vector3.Distance(movementSource.position, lastPosition);
        Debug.Log($"Distance: {distance} (Threshold: {newPositionThresholdDistance})"); 

        if (distance > newPositionThresholdDistance)
        {
            currentCoordinatesList.Add(movementSource.position);
            Debug.Log($"New position added: {movementSource.position}");

            if (debugCubePrefab)
            {
                DisplayHandMovementProgress($"New position added: {movementSource.position}", debugDisplay);

                GameObject instance = Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
                Destroy(instance, 2.0f); // Destroy after 2 seconds            
            }
            stillTime = 0f;
        }
        else
        {
            stillTime += Time.deltaTime;

            // Once still for over 1 sec, it will classify it as a static pose instead of a movement pose 
            if (stillTime >= 1.0f)
            {
                isMoving = false;
                if (currentStaticWord != "")
                {
                    DisplayHandMovementProgress(currentStaticWord, debugDisplay);
                    translateSign.SignedWord(currentStaticWord);
                    currentStaticWord = "";
                }
                stillTime = 0f;
            }
        }
    }

    /// <summary>
    /// Processes the the movement gesture done by the user and returns list of coordinates of the movement 
    /// </summary>
    public MovementData GetFinishedMovement()
    {
        isMoving = false;

        Point[] pointArray = new Point[currentCoordinatesList.Count];
        for (int i = 0; i < currentCoordinatesList.Count; i++)
        {
            pointArray[i] = new Point(currentCoordinatesList[i].x, currentCoordinatesList[i].y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);
        return new MovementData(newGesture, pointArray);
    }

    /// <summary>
    /// Ends the movement creation once user stops making the hand pose
    /// </summary>
    public void EndCreationMovement()
    {

        isMoving = false;
        Gesture newGesture = GetFinishedMovement().gesture;
        Point[] pointArray = GetFinishedMovement().points;
        Debug.Log($"Saving new gesture: {newGestureName}");
        newGesture.Name = newGestureName;
        properCoordinatesList.Add(newGesture);
        string fileName = Application.persistentDataPath + "/" + newGestureName + handName +".xml";
        GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        DisplayHandMovementProgress(fileName, debugDisplay);
    }

    /// <summary>
    /// Compares accuracy of the movement to the attached movement file according to movement threshold
    /// </summary>
    /// <param name="movementPoseName"></param>
    /// <param name="requiredPose"></param>
    /// <param name="movementFile"></param>
    /// <returns></returns>
    public bool CompareMovement(string movementPoseName, XRHandPose requiredPose, TextAsset movementFile)
    {
        isMoving = false;
        Gesture newGesture = GetFinishedMovement().gesture;
        Gesture loadedGesture = GestureIO.ReadGestureFromXML(movementFile.text);
        if (loadedGesture == null || newGesture == null)
        {
            DisplayHandMovementProgress("Either loadedGesture or newGesture is null.", debugDisplay);
        }
        PDollarGestureRecognizer.Result result = PointCloudRecognizer.Classify(newGesture, loadedGesture);
        DisplayHandMovementProgress($"Accuracy score = {result.Score}, Gesture = {result.GestureClass}", debugDisplay);
        DisplayHandMovementProgress(result.Score.ToString(), debugDisplay);
        if (result.Score > recognitionThreshold)
        {
            return true;
        }
        else
        {
            DisplayHandMovementProgress("Gesture recognition score below threshold.",debugDisplay);
            return false;
        }
    }
}


