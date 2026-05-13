using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        NPCSaveSystem.Load();
    }
}