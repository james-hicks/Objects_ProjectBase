using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    public static AudioManager Instance;

    private void Awake()
    {
        if (!(audioSource = GetComponent<AudioSource>()))
        {
            Debug.Log($"No Audio Source found");
            return;
        }

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlaySound(string soundName)
    {
        soundName = soundName.ToLower();
        switch (soundName)
        {
            case "menu":
                audioSource.Stop();
                audioSource.loop = true;
                audioSource.clip = audioClips[0];
                audioSource.Play();
                break;
            case "gamemusic":
                audioSource.Stop();
                audioSource.loop = true;
                audioSource.clip = audioClips[1];
                audioSource.Play();
                break;
            case "playerdied":
                audioSource.Stop();
                audioSource.PlayOneShot(audioClips[2]);
                break;
            case "shoot":
                audioSource.PlayOneShot(audioClips[3]);
                break;
            case "explosion":
                audioSource.PlayOneShot(audioClips[4]);
                break;
            default:
                Debug.LogWarning($"Sounds not found: {soundName}");
                break;
        }
    }

    public void StopBackgroundMusic() 
    {
        if (audioSource.isPlaying) 
        {
            audioSource.Stop();
        }
    }

}
