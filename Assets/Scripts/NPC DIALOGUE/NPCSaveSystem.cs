using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class NPCSaveSystem
{
    private static string path =>
        Path.Combine(Application.persistentDataPath, "npc_states.json");

    private static NPCSaveData data;

    public static void Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<NPCSaveData>(json);
        }
        else
        {
            data = new NPCSaveData();
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static string GetState(string npcID)
    {
        if (data == null) Load();

        foreach (var entry in data.npcStates)
        {
            if (entry.npcID == npcID)
                return entry.state;
        }

        return null;
    }

    public static void SetState(string npcID, string state)
    {
        if (data == null) Load();

        foreach (var entry in data.npcStates)
        {
            if (entry.npcID == npcID)
            {
                entry.state = state;
                Save();
                return;
            }
        }

        data.npcStates.Add(new NPCStateEntry
        {
            npcID = npcID,
            state = state
        });

        Save();
    }
}