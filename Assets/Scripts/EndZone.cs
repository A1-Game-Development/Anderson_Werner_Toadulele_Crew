using UnityEngine;

public class EndZone : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private SceneTransition transition;
    [SerializeField] private string nextSceneName;

    [Header("Spawn Settings")]
    [Tooltip("Leave empty for directional spawn system")]
    [SerializeField] private string spawnOverrideName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            // ?? If override is set ? use exact spawn (houses/caves)
            if (!string.IsNullOrEmpty(spawnOverrideName))
            {
                PlayerController.spawnOverride = spawnOverrideName;
            }
            else
            {
                // ?? Otherwise use directional system
                player.SetExitDirection();
            }
        }

        transition.LoadScene(nextSceneName);
    }
}