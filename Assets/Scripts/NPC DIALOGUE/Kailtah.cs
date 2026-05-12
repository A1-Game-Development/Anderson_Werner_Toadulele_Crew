using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Kailtah : NPCBase
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue")]
    public string dialogueFileName = "Kailtah";

    [Tooltip("Which dialogue block should play first")]
    public string currentState = "Introduction";

    [Header("Settings")]
    public float typingSpeed = 0.03f;
    public AudioSource audioSource;
    public AudioClip typingSound;

    private bool playerInRange = false;
    private bool isTyping = false;
    private bool canPress = true;

    private Dictionary<string, string[]> dialogueLines =
        new Dictionary<string, string[]>();

    private int currentLineIndex = 0;
    private string[] currentLines;

    void Start()
    {
        LoadDialogue();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

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
            StopAllCoroutines();

            dialogueText.text = currentLines[currentLineIndex];
            isTyping = false;
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

        yield return new WaitForSeconds(0.2f);

        canPress = true;
    }

    void LoadDialogue()
    {
        TextAsset textFile =
            Resources.Load<TextAsset>(dialogueFileName);

        if (textFile == null)
        {
            Debug.LogError("Dialogue file NOT FOUND!");
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

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char c in currentLines[currentLineIndex])
        {
            dialogueText.text += c;

            if (typingSound && audioSource)
                audioSource.PlayOneShot(typingSound);

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    // --------------------------------
    // CHANGE STATES ANYTIME
    // --------------------------------
    public void SetState(string newState)
    {
        currentState = newState;

        Debug.Log("NPC state changed to: " + newState);
    }

    // --------------------------------
    // TRIGGERS
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

            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
    }
}