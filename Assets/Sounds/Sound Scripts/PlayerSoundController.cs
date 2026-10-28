using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip moveSound;
    public AudioClip takingDamageSound;
    public AudioClip deathSound;

    public void playJump()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void playMove()
    {
        audioSource.PlayOneShot(moveSound);
    }

    public void playDamage()
    {
        audioSource.PlayOneShot(takingDamageSound);
    }

    public void playDeath()
    {
        audioSource.PlayOneShot(deathSound);
    }
}
