/*
 * Author: Jacie Thoo Yixuan, Hoo Ying Qi Praise, Verlaine Ong Xin Yi
 * Date: 3/6/2025
 * Description: This Script handles the storybook pages; events, flipping 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.UI;


public class PageManager : MonoBehaviour
{
    [Header("Storybook")]
    /// <summary>
    /// Stores each page pivots
    /// </summary>
    public List<Transform> pagePivots;

    /// <summary>
    /// Stores flip page speed for animation
    /// </summary>
    public float flipSpeed;

    /// <summary>
    /// Stores the target y positions for each book page
    /// </summary>
    public List<float> pageTargetYPositions;

    /// <summary>
    /// Stores current page index
    /// </summary>
    private int currentPage = 0;

    /// <summary>
    /// Whether the book is currently being flipped
    /// </summary>
    public bool isFlipping = false;

    /// <summary>
    /// Reference to trigger to flip book page
    /// </summary>
    public Collider pageFlipTrigger;

    [Header("References")]
    /// <summary>
    /// Reference to SignDanceManager script
    /// </summary>
    public SignDanceManager signDanceManager;

    /// <summary>
    /// Reference to Storybook script
    /// </summary>
    public Storybook storybook;

    /// <summary>
    /// Reference to access animator
    /// </summary>
    public Animator animator;

    [Header("Page 1")]
    /// <summary>
    /// Reference to main wolf
    /// </summary>
    public GameObject wolf;

    // Page 3: Spawn Tree + wolf singing
    [Header("Page 3")]
    /// <summary>
    /// Reference to wolf 1
    /// </summary>
    public GameObject DB_Sing;

    /// <summary>
    /// Reference to wolf 2
    /// </summary>
    public GameObject LB_Sing;

    /// <summary>
    /// Reference to wolf 3
    /// </summary>
    public GameObject LG_Sing;

    /// <summary>
    /// Reference to piano
    /// </summary>
    public GameObject Piano;

    /// <summary>
    /// Reference to drums
    /// </summary>
    public GameObject Drum;

    /// <summary>
    /// Reference to spawn effects
    /// </summary>
    public GameObject spawnEffects;

    // Page 5: Bottom of the mountain, got 3 wolves + press button and speech bubble
    [Header("Page 5")]
    /// <summary>
    /// Reference to speech bubbles
    /// </summary>
    public GameObject speechBubbles;

    // Page 6: Tree change texture into angry + rain
    [Header("Page 6")]
    /// <summary>
    /// Reference to rain VFX
    /// </summary>
    public GameObject rain;

    /// <summary>
    /// Reference to angry face
    /// </summary>
    public GameObject angry;

    /// <summary>
    /// Reference to smiling VFX
    /// </summary>
    public GameObject smiling;

    /// <summary>
    /// Reference to angry VFX
    /// </summary>
    public GameObject angryEffect;

    //Page 13: Wolf heart
    [Header("Page 13")]

    /// <summary>
    /// Reference to heartbeat VFX
    /// </summary>
    public GameObject heartbeat;

    /// <summary>
    /// Reference to shockwave VFX
    /// </summary>
    public GameObject shockwave;

    //Page 14: Music and dancing
    [Header("Page 14")]
    /// <summary>
    /// Reference to music VFX
    /// </summary>
    public GameObject music;

    /// <summary>
    /// Reference to black notes VFX
    /// </summary>
    public GameObject black_notes;

    [Header("Mini Game")]
    /// <summary>
    /// Reference to slider for mini game interaction
    /// </summary>
    public Slider slider;

    /// <summary>
    /// Whether "dance" has been signed; for mini game 
    /// </summary>
    public bool danceSigned = false;

    /// <summary>
    /// Gets the current page number of the book
    /// </summary>
    public int CurrentPage => currentPage;

    /// <summary>
    /// Initializes references
    /// </summary>
    public void Start()
    {
        animator = GetComponent<Animator>();
     
        storybook = FindObjectOfType<Storybook>();
        if (storybook != null)
        {
            storybook.pageManager = this;
        }

        else
        {
            Debug.Log("Storybook is null");
        }
    }

    /// <summary>
    /// Flip page when trigger is entered
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!isFlipping && currentPage < pagePivots.Count)
        {
            StartCoroutine(FlipBookPage(pagePivots[currentPage], currentPage));
        }
    }

    /// <summary>
    /// Flips a book page with animation
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    IEnumerator FlipBookPage(Transform page, int pageIndex)
    {
        isFlipping = true;
        float currentX = page.localEulerAngles.x;
        float targetX = -180f;

        // Flips book in right position
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

        // Put in correct order after flipping
        if (pageIndex < pageTargetYPositions.Count)
        {
            Vector3 newPos = page.localPosition;
            newPos.y = pageTargetYPositions[pageIndex];
            page.localPosition = newPos;
        }

        isFlipping = false;

        // Lock page after flipping
        pageFlipTrigger.enabled = false;

        //Increase page index
        currentPage++;
    }

    /// <summary>
    /// Gets the current page index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentPageIndex()
    {
        return currentPage;
    }

    /// <summary>
    /// Health bar for page index 6 interaction (mini game)
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
    /// Handles events for page 1 of book
    /// Signed word: Wolf
    /// </summary>
    public void Page1Functions()
    {
        // Show main wolf
        wolf.SetActive(true);

        Debug.Log("Page 1 animation triggered.");
    }

    /// <summary>
    /// Handles events for page 3 of book
    /// Signed word: Sing
    /// </summary>
    public void Page3Functions()
    {
        // Spawn VFX
        spawnEffects.SetActive(true);

        // Praise
        DB_Sing.SetActive(true);
        LB_Sing.SetActive(true);
        LG_Sing.SetActive(true);

        // Verlaine 
        // Show the animation of singing
        DB_Sing.GetComponent<Animator>().SetBool("isSigned3", true);
        LB_Sing.GetComponent<Animator>().SetBool("isSigned", true);
        LG_Sing.GetComponent<Animator>().SetBool("isSigned4", true);

        // Show instruments
        Piano.SetActive(true);
        Drum.SetActive(true);

        Debug.Log("Page 3 animation triggered.");
    }

    /// <summary>
    /// Handles events for page 5 of book
    /// Signed word: Talking
    /// </summary>
    public void Page5Functions()
    {
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

        StartCoroutine(MoveAllNPCsAfterDelay(npcDB, npcLB, npcLG));
        StartCoroutine(ShowSpeechBubblesAfterDelay());

        Debug.Log("Page 5 animation triggered.");
    }

    /// <summary>
    /// Moves NPC to destination after a delay, call in pge 5 functions
    /// </summary>
    /// <param name="npcDB"></param>
    /// <param name="npcLB"></param>
    /// <param name="npcLG"></param>
    /// <returns></returns>
    private IEnumerator MoveAllNPCsAfterDelay(NPCMove npcDB, NPCMove npcLB, NPCMove npcLG)
    {
        yield return new WaitForSeconds(0f);

        // Move all NPCs simultaneously
        if (npcDB != null) npcDB.MoveToDestination();
        if (npcLB != null) npcLB.MoveToDestination();
        if (npcLG != null) npcLG.MoveToDestination();
    }

    /// <summary>
    /// Show speech bubbles after a delay, call in page 5 functions
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowSpeechBubblesAfterDelay()
    {
        yield return new WaitForSeconds(7f);
        speechBubbles.SetActive(true);
    }

    /// <summary>
    /// Handles events for page 6 of book
    /// Signed word: Rain
    /// </summary>
    public void Page6Functions()
    {
        // Get the NPCMove components
        NPCMove npcDB = DB_Sing.GetComponent<NPCMove>();
        NPCMove npcLB = LB_Sing.GetComponent<NPCMove>();
        NPCMove npcLG = LG_Sing.GetComponent<NPCMove>();

        // Call MoveToDestination2 on each NPC
        if (npcDB != null) npcDB.MoveToDestination2();
        if (npcLB != null) npcLB.MoveToDestination2();
        if (npcLG != null) npcLG.MoveToDestination2();

        // Hide speech bubbles and make tree angry, rain
        speechBubbles.SetActive(false);
        smiling.SetActive(false);
        angry.SetActive(true);
        angryEffect.SetActive(true);
        rain.SetActive(true);

        Debug.Log("Page 6 animation triggered.");
    }

    /// <summary>
    /// Handles events for page 8 of book
    /// Signed word: Climbed
    /// </summary>
    public void Page8Functions()
    {
        // Call the function from NPCMove script
        NPCMove npcMW = wolf.GetComponent<NPCMove>();

        if (npcMW != null)
        {
            npcMW.MoveToDestination();
        }

        Debug.Log("Page 8 animation triggered.");
    }

    /// <summary>
    /// Handles events for page 10 of book
    /// Signed word: Dance
    /// With Names - from signDanceManager
    /// </summary>
    public void Page10Functions()
    {
        // Starts mini game
        if (signDanceManager != null)
        {
            Debug.Log("Showing next prompt");
            signDanceManager.namePromptUI.gameObject.SetActive(true);
            signDanceManager.ResetGame();
            danceSigned = true;
        }
        else
        {
            Debug.LogWarning("SignDanceManager is null");
        }

        // Verlaine
        wolf.GetComponent<Animator>().SetBool("isSigned2", true);

        // Praise
        // Show health bar and set values
        slider.gameObject.SetActive(true);
        slider.maxValue = 3;
        slider.value = 3;

        Debug.Log("Page 10 animation triggered.");
    }

    /// <summary>
    /// Handles events for page 13 of book
    /// Signed word: Heart
    /// </summary>
    public void Page13Functions()
    {
        // Show VFX
        heartbeat.SetActive(true);
        shockwave.SetActive(true);

        Debug.Log("Page 13 animation triggered.");
    }

    /// <summary>
    /// Handles events for page 14 of book
    /// Signed word: Happily
    /// </summary>
    public void Page14Functions()
    {
        // Show VFX
        music.SetActive(true);
        black_notes.SetActive(true);

        Debug.Log("Page 14 animation triggered.");
    }
} 