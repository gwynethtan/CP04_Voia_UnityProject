using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FlipPage : MonoBehaviour
{
    // NOTE TO SELF: pg 0 is cover page

    public List<Transform> pagePivots;
    public float flipSpeed;
    public List<float> pageTargetYPositions;
    private int currentPage = 0;
    private bool isFlipping = false;
    public TextMeshProUGUI text;

    public Collider pageFlipTrigger;

    [Header("Prefabs")]
    public GameObject mountain;
    public GameObject wolf;

    // Page 3: Spawn Tree + wolf singing

    public GameObject DB_Sing;
    public GameObject LB_Sing;
    public GameObject LG_Sing;
    public GameObject heroTree;
    public GameObject trees;

    // Page 5: Bottom of the mountain, got 3 wolves + press button and speech bubble
    public GameObject DB_Idle;
    public GameObject LB_Idle;
    public GameObject LG_Idle;

    // Page 6: Tree change texture into angry + rain
    public GameObject rain;

    public int CurrentPage => currentPage;

    public void Start()
    {
     
        Storybook storybook = FindObjectOfType<Storybook>();
        if (storybook != null)
        {
            storybook.flipPage = this;
            storybook.bookPosition = this.transform;
            text.text = "Page" + currentPage;
        }
        else
        {
            text.text = "Fail";
        }
    }

    public void Update()
    {

    }

    // Need to add cannot flip if prev page not done
    private void OnTriggerEnter(Collider other)
    {
        if (!isFlipping && currentPage < pagePivots.Count)
        {
            StartCoroutine(FlipBookPage(pagePivots[currentPage], currentPage));
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

        if (currentPage == 0)
        {
            mountain.SetActive(true);
        }

        isFlipping = false;
        text.text = "Page" + currentPage;

        currentPage++;
    }

    public void Page1Functions()
    {
        wolf.SetActive(true);
        Debug.Log("Page 1 animation triggered.");
    }

    public void Page3Functions()
    {
        // New - Praise
        DB_Sing.SetActive(true);
        LB_Sing.SetActive(true);
        LG_Sing.SetActive(true);
        heroTree.SetActive(true);
        trees.SetActive(true);
        
        Debug.Log("Page 3 animation triggered.");
    }

    public void Page5Functions()
    {
        // New - Praise
        DB_Idle.SetActive(true);
        LB_Idle.SetActive(true);
        LG_Idle.SetActive(true);

        Debug.Log("Page 5 animation triggered.");
    }

    public void Page6Functions()
    {
        // New - Praise
        rain.SetActive(true);

        Debug.Log("Page 6 animation triggered.");
    }
}
