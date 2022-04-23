using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource [] musicSource = new AudioSource[2];
    int musicIndex;
    AudioSource sfxSource;
    List<AudioSource> sfxSources = new List<AudioSource>();
    Queue<int> freeSfxSources = new Queue<int>();

    EventManager em;
    [SerializeField] AudioClip[] musicClipPerPhase;
    [SerializeField] AudioClip startClip;

    public void Awake(){
        DontDestroyOnLoad(this.gameObject);

        musicSource[0] = this.gameObject.AddComponent<AudioSource>();
        musicSource[1] = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < 2; i++){
            sfxSources.Add(this.gameObject.AddComponent<AudioSource>());
            sfxSources[i].loop = true;
            freeSfxSources.Enqueue(i);
        }

        musicSource[0].loop = true;
        musicSource[1].loop = true;

        musicSource[musicIndex].clip = startClip;
        musicSource[musicIndex].Play();
    }
    
    public void Activate(){
        em = FindObjectOfType<EventManager>();
        em.OnTimeChange += UpdateOnTimeChange;
    }

    private void UpdateOnTimeChange(float x, int phase){
        if (musicClipPerPhase[phase] != null)PlayFadeMusic(musicClipPerPhase[phase], 1.5f);
    }
    
    public void PlayMusic(AudioClip musicClip){
        AudioSource activeSource = musicSource[(musicIndex^=1)];
        activeSource.clip = musicClip;
        activeSource.volume = 1;
        activeSource.Play();
    }

    public void PlayFadeMusic(AudioClip musicClip, float transitionTime = 1.0f){
        AudioSource activeSource = musicSource[(musicIndex^=1)];
        StartCoroutine(FadeMusic(musicClip, transitionTime));
    }
    private IEnumerator FadeMusic(AudioClip audioClip, float transitionTime){
        musicSource[musicIndex^1].clip = audioClip;
        musicSource[musicIndex^1].volume = 0;
        musicSource[musicIndex^1].Play();
        for (float t = 0; t < transitionTime; t+= Time.deltaTime){
            musicSource[musicIndex].volume = 1 - (t/transitionTime);
            musicSource[musicIndex^1].volume = t/transitionTime;
            yield return null;
        }
        musicSource[musicIndex].Stop();
        musicIndex ^= 1;
    }

    public void PlaySFX(AudioClip sfx){
        sfxSource.volume = 1;
        sfxSource.PlayOneShot(sfx);
    }

    public void PlaySFX(AudioClip sfx, float volume){
        sfxSource.volume = volume;
        sfxSource.PlayOneShot(sfx);
    }

    public int PlayConstantSFX(AudioClip newClip, float newVolume = 1.0f){
        if (freeSfxSources.Count <= 0){
            sfxSources.Add(this.gameObject.AddComponent<AudioSource>());
            sfxSources[sfxSources.Count-1].loop = true;
            freeSfxSources.Enqueue(sfxSources.Count-1);
        }
        int freeIndex = freeSfxSources.Dequeue();
        AudioSource source = sfxSources[freeIndex];
        source.clip = newClip;
        source.volume = newVolume;
        source.Play();
        return freeIndex;
    }

    public void StopConstantSFX(int index){
        sfxSources[index].Stop();
        freeSfxSources.Enqueue(index);
    }
}
