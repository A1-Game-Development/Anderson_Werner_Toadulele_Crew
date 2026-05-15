using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class EndGameTrigger : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad = "EndingScene";

    [Header("Required NPC States")]
    public List<string> requiredNPCs = new List<string>()
    {
        "Stan",
        "Gerson",
        "Shaq",
        "Villager"
    };

    public string requiredState = "Outroduction";

    [Header("Player")]
    public string playerTag = "Player";

    [System.Serializable]
    public class NPCState
    {
        public string npcID;
        public string state;
    }

    [System.Serializable]
    public class NPCStateList
    {
        public List<NPCState> npcStates;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        string path = Path.Combine(Application.persistentDataPath, "npc_states.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning("JSON file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);

        NPCStateList stateList = JsonUtility.FromJson<NPCStateList>(json);

        if (stateList == null || stateList.npcStates == null)
        {
            Debug.LogWarning("Failed to read NPC states.");
            return;
        }

        bool allComplete = true;

        foreach (string npcName in requiredNPCs)
        {
            bool foundCorrectState = false;

            foreach (NPCState npc in stateList.npcStates)
            {
                if (npc.npcID == npcName && npc.state == requiredState)
                {
                    foundCorrectState = true;
                    break;
                }
            }

            if (!foundCorrectState)
            {
                allComplete = false;
                Debug.Log(npcName + " is NOT at state: " + requiredState);
                break;
            }
        }

        if (allComplete)
        {
            Debug.Log("All NPCs completed. Loading scene...");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log("Not all NPCs are ready yet.");
        }
    }
}