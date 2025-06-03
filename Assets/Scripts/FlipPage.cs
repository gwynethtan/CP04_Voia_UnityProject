using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPage : MonoBehaviour
{
    public List<Transform> pagePivots;
    public float flipSpeed;
    private int currentPage = 0;
    private bool isFlipping = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isFlipping && currentPage < pagePivots.Count)
        {
            StartCoroutine(FlipBookPage(pagePivots[currentPage]));
            currentPage++;
        }
    }

    IEnumerator FlipBookPage(Transform page)
    {
        isFlipping = true;
        float currentX = page.localEulerAngles.x;
        float targetX = -180f;

        // SO IT DOESNT FLIP WRONG WAY
        if (currentX > 180) currentX -= 360;

        while (Mathf.Abs(currentX - targetX) > 0.5f)
        {
            currentX = Mathf.MoveTowards(currentX , targetX, flipSpeed * Time.deltaTime);
            page.localEulerAngles = new Vector3(currentX, 0, 0);
            yield return null;
        }

        page.localEulerAngles = new Vector3(targetX, 0, 0);
        isFlipping = false;
    }
}
