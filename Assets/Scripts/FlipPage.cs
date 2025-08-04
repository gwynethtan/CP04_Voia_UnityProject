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
    public bool isFlipping = false;
    public TextMeshProUGUI text;

    public Collider pageFlipTrigger;
    public SignDanceManager signDanceManager;
    public Storybook storybook;

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
    public GameObject speechBubbles;

    // Page 6: Tree change texture into angry + rain
    public GameObject rain;
    public GameObject angry;
    public GameObject smiling;
    public GameObject angryEffect;

    // Page 8: Wolf Dancing
    public GameObject mainWolf_Dance;

    //Page 9: Wolf heart
    public GameObject heartbeat;
    public GameObject shockwave;

    //Page 10; Music and dancing
    public GameObject music;
    public GameObject black_notes;

    //fake page for 2 and 4
    public GameObject Cube;

    // Game Interaction
    public Slider slider; 

    public int CurrentPage => currentPage;

    public void Start()
    {
        animator = GetComponent<Animator>();
     
        storybook = FindObjectOfType<Storybook>();
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

        // put in correct order after flipping
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

        // Lock page after flipping
        pageFlipTrigger.enabled = false;

        currentPage++;
        text.text = "Page" + currentPage;
    }

    public int GetCurrentPageIndex()
    {
        return currentPage;
    }


    /// <summary>
    /// Health bar for page index 6 interaction
    /// </summary>
    /// <param name="newHealth"></param>
    public void UpdateHealthBar(int newHealth)
    {
        if (slider != null)
        {
            slider.value = newHealth;
        }
    }

    /// <summary>
    /// Signed word: Wolf
    /// </summary>
    public void Page1Functions()
    {
        wolf.SetActive(true);
        Debug.Log("Page 1 animation triggered.");
    }

    /// <summary>
    /// Signed word: Sing
    /// </summary>
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
        DB_Sing.GetComponent<Animator>().SetBool("isSigned3", true);
        LB_Sing.GetComponent<Animator>().SetBool("isSigned", true);
        LG_Sing.GetComponent<Animator>().SetBool("isSigned4", true);

        Piano.SetActive(true);
        Drum.SetActive(true);

        //currentPage++;

        Debug.Log("Page 3 animation triggered.");
    }

    /// <summary>
    /// Signed word: Talking
    /// </summary>
    public void Page5Functions()
    {
        // New - Praise
        //DB_Idle.SetActive(true);
        //LB_Idle.SetActive(true);
        //LG_Idle.SetActive(true);


        //Verlaine 
        //Show idle
        DB_Sing.GetComponent<Animator>().SetBool("isSigned3", false);
        LB_Sing.GetComponent<Animator>().SetBool("isSigned", false);
        LG_Sing.GetComponent<Animator>().SetBool("isSigned4", false);

        // Call MoveToDestination from NPCMove script

        // Get all NPC Move components
        NPCMove npcDB = DB_Sing.GetComponent<NPCMove>();
        NPCMove npcLB = LB_Sing.GetComponent<NPCMove>();
        NPCMove npcLG = LG_Sing.GetComponent<NPCMove>();

        // 4 Secs delay
        StartCoroutine(MoveAllNPCsAfterDelay(npcDB, npcLB, npcLG));
        StartCoroutine(ShowSpeechBubblesAfterDelay());

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

    private IEnumerator ShowSpeechBubblesAfterDelay()
    {
        yield return new WaitForSeconds(7f);
        speechBubbles.SetActive(true);
    }

    /// <summary>
    /// Signed word: Rain
    /// </summary>
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

        smiling.SetActive(false);
        angry.SetActive(true);
        angryEffect.SetActive(true);
        rain.SetActive(true);

        Debug.Log("Page 6 animation triggered.");
    }

    /// <summary>
    /// Signed word: Climbed
    /// </summary>
    public void Page8Functions()
    {
        // Call the function from NPCMove script

        NPCMove npcMW = wolf.GetComponent<NPCMove>();

        if (npcMW != null) npcMW.MoveToDestination();

        Debug.Log("Page 8 animation triggered.");
    }

    private IEnumerator CallDestinationWithDelay(NPCMove npc)
    {
        yield return new WaitForSeconds(0.4f); 
        npc.MoveToDestination();
    }

    /// <summary>
    /// Signed word: Dance
    /// With Names - from signDanceManager
    /// </summary>
    public void Page10Functions()
    {

        if (signDanceManager != null)
        {
            signDanceManager.promptText.gameObject.SetActive(true);
            signDanceManager.feedbackText.gameObject.SetActive(true);
            signDanceManager.ResetGame();  
            signDanceManager.ShowNextPrompt();
        }

        //verlaine

        wolf.GetComponent<Animator>().SetBool("isSigned2", true);

        // new praise
        // Show health bar
        slider.gameObject.SetActive(true);
        slider.maxValue = 3;
        slider.value = 3;

        Debug.Log("Page 10 animation triggered.");
    }

    /// <summary>
    /// Signed word: Heart
    /// </summary>
    public void Page13Functions()
    {
        heartbeat.SetActive(true);
        shockwave.SetActive(true);


        Debug.Log("Page 13 animation triggered.");

        //currentPage++;
    }

    /// <summary>
    /// Signed word: Happily
    /// </summary>
    public void Page14Functions()
    {
        music.SetActive(true);
        black_notes.SetActive(true);

        Debug.Log("Page 14 animation triggered.");
    }
}
    