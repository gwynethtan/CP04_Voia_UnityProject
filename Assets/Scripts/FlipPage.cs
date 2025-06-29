using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.UI;


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


    //To access animator
    public Animator animator;

    // Page 3: Spawn Tree + wolf singing

    public GameObject DB_Sing;
    public GameObject LB_Sing;
    public GameObject LG_Sing;
    public GameObject heroTree;
    public GameObject trees;
    public GameObject Piano;
    public GameObject Drum;

    // Page 5: Bottom of the mountain, got 3 wolves + press button and speech bubble
    public GameObject DB_Idle;
    public GameObject LB_Idle;
    public GameObject LG_Idle;

    // Page 6: Tree change texture into angry + rain
    public GameObject rain;
    public GameObject Angry;
    public GameObject Smiling;

    // Page 8: Wolf Dancing
    public GameObject MainWolf_Dance;

    //Page 9: Wolf heart
    public GameObject Heartbeat;
    public GameObject Shockwave;

    //Page 10; Music and dancing
    public GameObject Music;
    public GameObject Black_notes;

    //fake page for 2 and 4
    public GameObject Cube;

    // Game Interaction
    public Slider slider; 
    public int health = 5;
    public bool isPage8Active = false;

    public int CurrentPage => currentPage;

    public void Start()
    {

        animator = GetComponent<Animator>();
     
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

    // Game Interaction
    public void updateHealthbar()
    {

    }
    public void Page1Functions()
    {
        wolf.SetActive(true);
        Debug.Log("Page 1 animation triggered.");
    }

    //Testing Only 
    public void Page2Functions()
    {
        Cube.SetActive(true);
        Cube.GetComponent<VisualEffect>().Play();
        Debug.Log("Page 2  animation triggered.");
    }

    public void Page3Functions()
    {
        // New - Praise
        DB_Sing.SetActive(true);
        LB_Sing.SetActive(true);
        LG_Sing.SetActive(true);
        //heroTree.SetActive(true);
        //trees.SetActive(true);

        //Verlaine 

        //Show the animation of singing
        DB_Sing.GetComponent<Animator>().SetBool("isSigned", true);
        LB_Sing.GetComponent<Animator>().SetBool("isSigned", true);
        LG_Sing.GetComponent<Animator>().SetBool("isSigned", true);

        Piano.SetActive(true);
        Drum.SetActive(true);

        //currentPage++;

        Debug.Log("Page 3 animation triggered.");
    }

    public void Page4Functions()
    {
        Cube.SetActive(false);
        Cube.GetComponent<VisualEffect>().Stop();
        Debug.Log("Page 4  animation triggered.");
    }
    public void Page5Functions()
    {
        // New - Praise
        //DB_Idle.SetActive(true);
        //LB_Idle.SetActive(true);
        //LG_Idle.SetActive(true);


        //Verlaine 
        //Show idle
        DB_Sing.GetComponent<Animator>().SetBool("isSigned", false);
        LB_Sing.GetComponent<Animator>().SetBool("isSigned", false);
        LG_Sing.GetComponent<Animator>().SetBool("isSigned", false);

        // Call MoveToDestination from NPCMove script

        // Get all NPC Move components
        NPCMove npcDB = DB_Sing.GetComponent<NPCMove>();
        NPCMove npcLB = LB_Sing.GetComponent<NPCMove>();
        NPCMove npcLG = LG_Sing.GetComponent<NPCMove>();

        // 4 Secs delay
        StartCoroutine(MoveAllNPCsAfterDelay(npcDB, npcLB, npcLG));

        //currentPage++;

        Debug.Log("Page 5 animation triggered.");
  
    }
    private IEnumerator MoveAllNPCsAfterDelay(NPCMove npcDB, NPCMove npcLB, NPCMove npcLG)
    {
        // Wait for 4 seconds
        yield return new WaitForSeconds(0f);

        // Move all NPCs simultaneously
        if (npcDB != null) npcDB.MoveToDestination();
        if (npcLB != null) npcLB.MoveToDestination();
        if (npcLG != null) npcLG.MoveToDestination();
    } 

    public void Page6Functions()
    {
        // New - Praise
        //rain.SetActive(true);

        //Verlaine
        //rain.GetComponent<VisualEffect>().Play();
        //Angry.SetActive(true);
        //Smiling.SetActive(false);

        // Get the NPCMove components
        NPCMove npcDB = DB_Sing.GetComponent<NPCMove>();
        NPCMove npcLB = LB_Sing.GetComponent<NPCMove>();
        NPCMove npcLG = LG_Sing.GetComponent<NPCMove>();

        // Call MoveToDestination2 on each NPC
        if (npcDB != null) npcDB.MoveToDestination2();
        if (npcLB != null) npcLB.MoveToDestination2();
        if (npcLG != null) npcLG.MoveToDestination2();

        Debug.Log("Page 6 animation triggered.");
    }

    public void Page7Functions()
    {
        // Activate the wolf
        wolf.SetActive(true);

        // Call the function from NPCMove script

        NPCMove npcMW = wolf.GetComponent<NPCMove>();

        if (npcMW != null) npcMW.MoveToDestination();

        Debug.Log("Page 7 animation triggered.");

        //currentPage++;
    }

    private IEnumerator CallDestinationWithDelay(NPCMove npc)
    {
        yield return new WaitForSeconds(0.4f); 
        npc.MoveToDestination();
    } 

    public void Page8Functions()
    {

        //verlaine

        wolf.GetComponent<Animator>().SetBool("isSigned2", true);
        Angry.SetActive(false);
        rain.GetComponent<VisualEffect>().Stop();

        // new praise
        // Show health bar
        slider.gameObject.SetActive(true);
        slider.maxValue = 5;
        slider.value = 5;

        health = 5;
        isPage8Active = true;

        Debug.Log("Page 8 animation triggered.");
    }

    public void Page9Functions()
    {
        Heartbeat.SetActive(true);
        Shockwave.SetActive(true);
        Heartbeat.GetComponent<VisualEffect>().Play();
        Shockwave.GetComponent<VisualEffect>().Play();

        Debug.Log("Page 9 animation triggered.");

        //currentPage++;
    }

    public void Page10Functions()
    {
        Music.SetActive(true);
        Black_notes.SetActive(true);
        Music.GetComponent<VisualEffect>().Play();
        Black_notes.GetComponent<VisualEffect>().Play();
        Debug.Log("Page 10 animation triggered.");
    }
}
    