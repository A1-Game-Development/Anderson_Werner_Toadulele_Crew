using UnityEngine;

public class NPCStateTrigger : MonoBehaviour
{
    [Header("Target NPC")]
    public string npcID;

    [Header("New State")]
    public string newState;

    [Header("Optional")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            NPCDialogue.SetNPCState(npcID, newState);
            hasTriggered = true;
        }
    }
}