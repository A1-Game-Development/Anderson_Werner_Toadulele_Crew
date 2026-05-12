using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareTrigger : MonoBehaviour
{
    public string playerTag = "Player";

    public Image jumpscareImage;
    public float flashDuration = 0.2f;
    public float fadeDuration = 2f;

    public AudioSource audioSource;
    public AudioClip jumpscareSound;

    private bool isPlaying = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isPlaying)
        {
            StartCoroutine(PlayJumpscare());
        }
    }

    IEnumerator PlayJumpscare()
    {
        isPlaying = true;

        // Play sound
        if (audioSource != null && jumpscareSound != null)
        {
            audioSource.PlayOneShot(jumpscareSound, 1f);
        }

        // Show image
        Color color = jumpscareImage.color;
        color.a = 1f;
        jumpscareImage.color = color;

        yield return new WaitForSeconds(flashDuration);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            jumpscareImage.color = color;
            yield return null;
        }

        color.a = 0f;
        jumpscareImage.color = color;

        isPlaying = false;
    }
}