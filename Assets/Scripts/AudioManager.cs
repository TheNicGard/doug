using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds = null;
    
    [SerializeField]
    public AudioMixer mixer;

    void Awake()
    {
        if (instance != null)
            Debug.LogError("AudioManager: More than one AudioManager in the scene!");
        else
            instance = this;

        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>(), mixer);
        }
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }

        Debug.LogError("AudioManager: Sound \"" + _name + "\" not found!");
    }

    public void PlayRandomSound(string[] names)
    {
        string soundName = names[Random.Range(0, names.Length)];
        for (int k = 0; k < sounds.Length; k++)
        {
            if (sounds[k].name == soundName)
            {
                sounds[k].Play();
                return;
            }
        }
        
        Debug.LogError("AudioManager: Sound \"" + soundName + "\" not found!");
    }
    
    public bool IsPlaying(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
            if (sounds[i].name == _name)
                return sounds[i].IsPlaying();

        Debug.LogError("AudioManager: Sound \"" + _name + "\" not found!");
        return false;
    }

    public void StopMusic()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].isMusic)
                sounds[i].Stop();
        }
    }

    public void ToggleSoundFXVolume(bool toggle)
    {
        mixer.SetFloat("Sound FX Volume", (toggle) ? 0f : -80f);
    }

    public void ToggleMusicVolume(bool toggle)
    {
        mixer.SetFloat("Music Volume", (toggle) ? 0f : -80f);
    }
}


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0f, 2f)]
    public float pitch = 1f;
    [Range(0f, 1f)]
    public float volumeAdjustment = 0f;
    [Range(0f, 2f)]
    public float pitchAdjustment = 0f;
    public bool isSound = true;
    public bool isMusic = false;
    public bool loops = false;
    private AudioSource source;

    public void SetSource(AudioSource _source, AudioMixer mixer)
    {
        source = _source;
        source.clip = clip;
        source.loop = loops;

        if (isMusic)
            source.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        else
            source.outputAudioMixerGroup = mixer.FindMatchingGroups("Sound FX")[0];
    }

    public void Play()
    {
        if ((isSound && PlayerPrefs.GetInt("soundEnabled") == 1) || (isMusic && PlayerPrefs.GetInt("musicEnabled") == 1))
        {
            source.volume = volume + Random.Range(-volumeAdjustment, volumeAdjustment);
            source.pitch = pitch + Random.Range(-pitchAdjustment, pitchAdjustment);
            source.Play();
        }
    }

    public bool IsPlaying()
    {
        return source.isPlaying;
    }

    public void Stop()
    {
        source.Stop();
    }
}