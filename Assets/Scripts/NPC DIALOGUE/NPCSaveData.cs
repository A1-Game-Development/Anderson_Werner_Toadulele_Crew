using System;
using System.Collections.Generic;

[Serializable]
public class NPCSaveData
{
    public List<NPCStateEntry> npcStates = new List<NPCStateEntry>();
}

[Serializable]
public class NPCStateEntry
{
    public string npcID;
    public string state;
}