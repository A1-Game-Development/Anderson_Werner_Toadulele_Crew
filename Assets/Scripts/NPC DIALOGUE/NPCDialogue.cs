using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NPCDialogue : NPCBase
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue")]
    [Tooltip("Name of the text file inside Resources")]
    public string dialogueFileName = "NPCDialogue";

    [Tooltip("Current dialogue state")]
    public string currentState = "Introduction";

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    [Tooltip("Extra pause after punctuation")]
    public float punctuationPause = 0.15f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound;

    [Tooltip("Delay between typing sounds")]
    public float typingSoundDelay = 0.02f;

    [Header("Smooth Dialogue")]
    public bool useFade = true;
    public CanvasGroup dialogueCanvasGroup;
    public float fadeSpeed = 8f;

    private bool playerInRange = false;
    private bool isTyping = false;
    private bool canPress = true;

    private float lastSoundTime;

    private Dictionary<string, string[]> dialogueLines =
        new Dictionary<string, string[]>();

    private int currentLineIndex = 0;
    private string[] currentLines;

    private Coroutine typingCoroutine;

    void Start()
    {
        LoadDialogue();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 0f;
    }

    // --------------------------------
    // INTERACT
    // --------------------------------
    public override void HandleInteract()
    {
        if (!playerInRange) return;
        if (!canPress) return;

        if (!dialoguePanel.activeSelf)
        {
            StartDialogue();
        }
        else if (isTyping)
        {
            SkipTyping();
        }
        else
        {
            NextLine();
        }

        StartCoroutine(InputCooldown());
    }

    IEnumerator InputCooldown()
    {
        canPress = false;

        yield return new WaitForSeconds(0.15f);

        canPress = true;
    }

    // --------------------------------
    // LOAD DIALOGUE FILE
    // --------------------------------
    void LoadDialogue()
    {
        TextAsset textFile =
            Resources.Load<TextAsset>(dialogueFileName);

        if (textFile == null)
        {
            Debug.LogError("Dialogue file NOT FOUND: " + dialogueFileName);
            return;
        }

        string[] lines = textFile.text.Split('\n');

        foreach (string line in lines)
        {
            if (line.Contains(":"))
            {
                string[] parts = line.Split(':');

                string key = parts[0].Trim();
                string[] values = parts[1].Split('|');

                dialogueLines[key] = values;
            }
        }
    }

    // --------------------------------
    // START DIALOGUE
    // --------------------------------
    void StartDialogue()
    {
        if (!dialogueLines.ContainsKey(currentState))
        {
            Debug.LogError("Missing dialogue state: " + currentState);
            return;
        }

        dialoguePanel.SetActive(true);

        currentLineIndex = 0;
        currentLines = dialogueLines[currentState];

        if (useFade && dialogueCanvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvas(1f));
        }

        typingCoroutine = StartCoroutine(TypeLine());
    }

    // --------------------------------
    // NEXT LINE
    // --------------------------------
    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentLines.Length)
        {
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    // --------------------------------
    // TYPEWRITER EFFECT
    // --------------------------------
    IEnumerator TypeLine()
    {
        isTyping = true;

        dialogueText.text = "";

        string line = currentLines[currentLineIndex];

        foreach (char c in line)
        {
            dialogueText.text += c;

            PlayTypingSound();

            float delay = typingSpeed;

            if (c == '.' || c == ',' || c == '!' || c == '?')
                delay += punctuationPause;

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    // --------------------------------
    // SKIP TYPING
    // --------------------------------
    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentLines[currentLineIndex];

        isTyping = false;
    }

    // --------------------------------
    // TYPING SOUND
    // --------------------------------
    void PlayTypingSound()
    {
        if (typingSound == null || audioSource == null)
            return;

        if (Time.time - lastSoundTime < typingSoundDelay)
            return;

        audioSource.PlayOneShot(typingSound);

        lastSoundTime = Time.time;
    }

    // --------------------------------
    // END DIALOGUE
    // --------------------------------
    void EndDialogue()
    {
        if (useFade && dialogueCanvasGroup != null)
        {
            StartCoroutine(CloseDialogueSmooth());
        }
        else
        {
            dialoguePanel.SetActive(false);
        }
    }

    IEnumerator CloseDialogueSmooth()
    {
        yield return FadeCanvas(0f);

        dialoguePanel.SetActive(false);
    }

    IEnumerator FadeCanvas(float target)
    {
        while (!Mathf.Approximately(dialogueCanvasGroup.alpha, target))
        {
            dialogueCanvasGroup.alpha =
                Mathf.MoveTowards(
                    dialogueCanvasGroup.alpha,
                    target,
                    fadeSpeed * Time.deltaTime
                );

            yield return null;
        }
    }

    // --------------------------------
    // CHANGE STATES ANYTIME
    // --------------------------------
    public void SetState(string newState)
    {
        currentState = newState;

        Debug.Log(name + " state changed to: " + newState);
    }

    // --------------------------------
    // OPTIONAL HELPERS
    // --------------------------------
    public void StartState(string newState)
    {
        SetState(newState);

        if (dialoguePanel.activeSelf)
            EndDialogue();

        StartDialogue();
    }

    // --------------------------------
    // PLAYER DETECTION
    // --------------------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            PlayerController player =
                other.GetComponentInParent<PlayerController>();

            if (player != null)
                player.currentNPC = this;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            PlayerController player =
                other.GetComponentInParent<PlayerController>();

            if (player != null)
                player.currentNPC = null;

            StopAllCoroutines();

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);

                if (dialogueCanvasGroup != null)
                    dialogueCanvasGroup.alpha = 0f;
            }

            isTyping = false;
        }
    }
}