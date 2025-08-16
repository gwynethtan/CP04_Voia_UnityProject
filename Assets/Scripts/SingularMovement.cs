/*
 * Author: Tan Ting Yu Gwyneth
 * Date: 21/5/2025
 * Description: This script manages hand movement for a single hand
 */

using UnityEngine.XR.Hands.Gestures;
using UnityEngine;

public class SingularMovement : MonoBehaviour
{
    /// <summary>
    /// Name of the movement pose
    /// </summary>
    public string movementPoseName;

    /// <summary>
    /// Pose for the movement
    /// </summary>
    public XRHandPose requiredPose;

    /// <summary>
    /// File of the coordinates for expected movement
    /// </summary>
    public TextAsset movementFile;

    /// <summary>
    /// Reference to movement mgr script 
    /// </summary>
    public MovementMgr movementMgr;

    /// <summary>
    /// Checks if the movement is done already
    /// </summary>
    public bool movementDone;

    /// <summary>
    /// Reference to the other hand movement for dual movement pose 
    /// </summary>
    public SingularMovement otherMovement;

    /// <summary>
    /// The word if the hand remains still 
    /// </summary>
    public string staticWord;

    /// <summary>
    /// Handles logic when the hand starts moving 
    /// </summary>
    public void StartMovement()
    {
        movementMgr.creationPose = requiredPose;
        movementMgr.currentStaticWord = staticWord;
        movementMgr.RecordMovement(movementPoseName, requiredPose);
    }

    /// <summary>
    /// Handles logic when the hand stops moving 
    /// </summary>
    public void EndMovement()
    {
        movementMgr.DisplayHandMovementProgress("Comparing", movementMgr.debugDisplay);
        movementDone = movementMgr.CompareMovement(movementPoseName, requiredPose, movementFile);
        if (movementDone)
        {
            if (otherMovement != null) // For dual hand movements
            {
                // Resets the tracking movement boolean to false
                if (otherMovement.movementDone)
                {
                    movementDone = false;
                    otherMovement.movementDone = false;
                }
                // Produces the sign 
                else
                {
                    movementMgr.translateSign.SignedWord(movementPoseName);
                }
            }
            else // For single hand movements
            {
                movementMgr.DisplayHandMovementProgress(movementPoseName, movementMgr.debugDisplay);
                movementMgr.translateSign.SignedWord(movementPoseName);
                movementDone = false;
            }
        }
    }
}
