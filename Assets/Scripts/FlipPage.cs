using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPage : MonoBehaviour
{
    public List<Transform> pagePivots;
    public float flipSpeed;
    public List<float> pageTargetYPositions;
    private int currentPage = 0;
    private bool isFlipping = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isFlipping && currentPage < pagePivots.Count)
        {
            StartCoroutine(FlipBookPage(pagePivots[currentPage], currentPage));
            currentPage++;
        }
    }

    IEnumerator FlipBookPage(Transform page, int pageIndex)
    {
        isFlipping = true;
        float currentX = page.localEulerAngles.x;
        float targetX = -180f;

        // SO IT DOESNT FLIP WRONG WAY
        if (currentX > 180)
        {
            currentX -= 360;
        }

        while (Mathf.Abs(currentX - targetX) > 0.5f)
        {
            currentX = Mathf.MoveTowards(currentX , targetX, flipSpeed * Time.deltaTime);
            page.localEulerAngles = new Vector3(currentX, -90, 0);
            yield return null;
        }

        page.localEulerAngles = new Vector3(targetX, -90, 0);

        // put in corrct order after flipping
        if (pageIndex < pageTargetYPositions.Count)
        {
            Vector3 newPos = page.localPosition;
            newPos.y = pageTargetYPositions[pageIndex];
            page.localPosition = newPos;
        }

        isFlipping = false;
    }
}
