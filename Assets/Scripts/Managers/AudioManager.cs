using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------Audio Sources--------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------Audio Clips------")]
    public AudioClip gameStart;
    public AudioClip gameOver;
    public AudioClip enemyKill;
    public AudioClip nukeCollect;
    public AudioClip nuke;
    public AudioClip healCollect;

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

}
