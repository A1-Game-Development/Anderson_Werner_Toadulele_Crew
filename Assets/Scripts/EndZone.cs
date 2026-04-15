using UnityEngine;

public class EndZone : MonoBehaviour
{
    [SerializeField] private SceneTransition transition;
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            transition.LoadScene(nextSceneName);
        }
    }
}