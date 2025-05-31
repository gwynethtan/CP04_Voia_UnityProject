using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BubbleMgr : MonoBehaviour
{
    public float displayDuration = 10f;
    public float fadeDuration = 0.001f;
    public float riseDuration = 0.001f;

    public float firstSlotHeight = 0.1857001f;
    public float heightGap = 0.45f;

    private List<BubbleGroup> bubbleGroupList= new List<BubbleGroup>();
    public IEnumerator ActivateBubble(BubbleGroup bubbleGroup, string sentText, bool autoFade)
    {
        Debug.Log("hello");

        bubbleGroup.bubbleText.text = sentText;

        if (bubbleGroup.bubbleActive == false) // ALready active bubbles will remain at their same position
        {
            Vector3 pos = bubbleGroup.bubbleBg.transform.localPosition;
            bubbleGroupList.Add(bubbleGroup);
            pos.y =  firstSlotHeight- bubbleGroupList.IndexOf(bubbleGroup) * heightGap;
            bubbleGroup.bubbleBg.transform.localPosition = pos;

            StartCoroutine(Fade(0f, 1f, bubbleGroup.bubbleBg)); // Increse bubble opacity
            bubbleGroup.bubbleActive = true;
        }
        if (autoFade)
        {
            yield return new WaitForSeconds(displayDuration);
            HandleBubbleDismissal(bubbleGroup);
        }
        else
        {
            yield return null;
        }
    }

    public void HandleBubbleDismissal(BubbleGroup bubbleGroup)
    {
        int removedIndex = bubbleGroupList.IndexOf(bubbleGroup);
        int oriBubbleGroupListCount = bubbleGroupList.Count;
        StartCoroutine(Fade(1f, 0f, bubbleGroup.bubbleBg)); // Decreases bubble opacity
        bubbleGroupList.Remove(bubbleGroup);
        bubbleGroup.bubbleActive = false;

        if (removedIndex != oriBubbleGroupListCount)
        {
            for (int i = removedIndex; i < bubbleGroupList.Count; i++)
            {
                float targetY = firstSlotHeight - (i * heightGap);
                StartCoroutine(MoveBubbleUp(bubbleGroupList[i].bubbleBg, targetY));
            }
        }
    }

    /// <summary>
    /// Function for bubble to increase or decrease bubble opacity
    /// </summary>
    /// <param name="opacityStart"></param>
    /// <param name="opacityEnd"></param>
    /// <param name="bubbleBg"></param>
    /// <returns></returns>
    public IEnumerator Fade(float opacityStart, float opacityEnd, GameObject bubbleBg)
    {
        CanvasGroup cg = bubbleBg.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogWarning("CanvasGroup component not found on " + bubbleBg.name);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(opacityStart, opacityEnd, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = opacityEnd;
    }

    /// <summary>
    /// Move bubble up ONLY when the firstSlotTaken turned from True to False 
    /// </summary>
    /// <param name="yAxisStart"></param>
    /// <param name="yAxisEnd"></param>
    /// <param name="bubbleBg"></param>
    /// <param name="isBubbleShown"></param>
    /// <returns></returns>
    private IEnumerator MoveBubbleUp(GameObject bubbleBg, float targetY)
    {
        Vector3 startPos = bubbleBg.transform.localPosition;
        Vector3 endPos = new Vector3(startPos.x, targetY, startPos.z);

        float elapsed = 0f;
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseDuration;
            bubbleBg.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        bubbleBg.transform.localPosition = endPos;
    }

}
