using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfx2DSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Duplicate SoundManager ditemukan, menghapus instance baru.");
            Destroy(gameObject);
            return;
        }

        Debug.Log("SoundManager tetap ada setelah scene berubah.");
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Pastikan AudioSource tidak null
        if (sfx2DSource == null)
        {
            sfx2DSource = GetComponent<AudioSource>();
            if (sfx2DSource == null)
            {
                Debug.LogError("AudioSource tidak ditemukan di SoundManager!");
            }
        }

        // Pastikan SoundLibrary tidak null
        if (sfxLibrary == null)
        {
            Debug.LogError("SoundLibrary belum di-assign di Inspector!");
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
        else
        {
            Debug.LogWarning("AudioClip untuk PlaySound3D tidak ditemukan!");
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        if (sfxLibrary == null)
        {
            Debug.LogError("SoundLibrary tidak tersedia!");
            return;
        }

        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            PlaySound3D(clip, pos);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' tidak ditemukan dalam SoundLibrary!");
        }
    }

    public void PlaySound2D(string soundName)
    {
        if (sfxLibrary == null)
        {
            Debug.LogError("SoundLibrary tidak tersedia!");
            return;
        }

        if (sfx2DSource == null)
        {
            Debug.LogError("sfx2DSource tidak ditemukan, tidak bisa memainkan suara!");
            return;
        }

        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            sfx2DSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' tidak ditemukan dalam SoundLibrary!");
        }
    }
}
