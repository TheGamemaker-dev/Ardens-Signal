using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    GameObject soundPrefab;

    List<AudioSource> playingSources = new List<AudioSource>();

    string[] ambientSounds = { "Fan Whirring", "Dream" };

    public static AudioManager singleton;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySound(string name, bool loop)
    {
        StartCoroutine(PlaySoundAsync(name, loop));
    }

    IEnumerator PlaySoundAsync(string name, bool loop)
    {
        bool isAmbient = false;
        foreach (string sound in ambientSounds)
        {
            if (name == sound)
            {
                isAmbient = true;
                break;
            }
        }
        if (isAmbient)
        {
            StartCoroutine(PlayAmbientSound(name));
        }
        else
        {
            AudioSource source = Instantiate(soundPrefab, transform).GetComponent<AudioSource>();
            source.loop = loop;

            AudioClip clip = null;

            yield return GetAudioClip(name, r => clip = r);

            source.clip = clip;
            playingSources.Add(source);
            source.Play();
            StartCoroutine(DestroySound(source));
        }
    }

    IEnumerator DestroySound(AudioSource source)
    {
        float waitTime = source.clip.length;
        yield return new WaitForSeconds(waitTime);
        playingSources.Remove(source);
        Destroy(source.gameObject);
    }

    public void StopSound(string soundName)
    {
        AudioSource source = playingSources.FirstOrDefault(x => x.clip.name == soundName);
        playingSources.Remove(source);
        Destroy(source.gameObject);
    }

    IEnumerator PlayAmbientSound(string soundName)
    {
        AudioClip startClip = null;
        AudioClip ambientClip = null;

        yield return GetAudioClip(soundName + " (start)", r => startClip = r);
        yield return GetAudioClip(soundName + " (ambient)", r => ambientClip = r);

        AudioSource source = Instantiate(soundPrefab, transform).GetComponent<AudioSource>();
        playingSources.Add(source);
        source.clip = startClip;
        source.volume = 0.25f;

        AudioSource source2 = Instantiate(soundPrefab, transform).GetComponent<AudioSource>();
        playingSources.Add(source2);
        source2.clip = ambientClip;
        source2.loop = true;
        source2.volume = 0.25f;

        source2.PlayScheduled(AudioSettings.dspTime + startClip.length - 1f);
        source.Play();

        yield return new WaitForSeconds(startClip.length);
        playingSources.Remove(source);
        Destroy(source.gameObject);
    }

    public IEnumerator StopAmbientSound(string soundName)
    {
        AudioSource source = null;

        AudioClip ambientClip = null;

        yield return GetAudioClip(soundName + " (ambient)", r => ambientClip = r);

        try
        {
            source = playingSources.First(x => x.clip == ambientClip);
        }
        catch (InvalidOperationException)
        {
            Debug.Log("No source of name " + soundName + " playing");
        }

        if (source != null)
        {
            AudioClip endClip = null;

            yield return GetAudioClip(soundName + " (end)", r => endClip = r);
            AudioSource source1 = Instantiate(soundPrefab, transform).GetComponent<AudioSource>();
            source1.volume = 0.25f;
            source1.clip = endClip;

            source1.PlayScheduled(AudioSettings.dspTime + 1.0f);

            yield return new WaitForSecondsRealtime(1);

            playingSources.Remove(source);
            Destroy(source.gameObject);

            yield return new WaitForSeconds(endClip.length);

            playingSources.Remove(source1);
            Destroy(source1.gameObject);
        }
    }

    void OnDayEnd(string flag)
    {
        if (flag.Contains("Done") && flag.Contains("day"))
        {
            StartCoroutine(StopAmbientSound("Fan Whirring"));
        }
    }

    void OnEnable()
    {
        GameManager.onFlagSet += OnDayEnd;
        SceneManager.activeSceneChanged += delegate
        {
            StopAllAmbientSounds();
        };
    }

    void OnDisable()
    {
        GameManager.onFlagSet -= OnDayEnd;
        SceneManager.activeSceneChanged -= delegate
        {
            StopAllAmbientSounds();
        };
    }

    void StopAllAmbientSounds()
    {
        List<string> sounds = new List<string>();
        foreach (AudioSource source in playingSources)
        {
            sounds.Add(source.clip.name);
        }
        foreach (string sound in ambientSounds)
        {
            if (sounds.Contains(sound + " (ambient)"))
            {
                StartCoroutine(StopAmbientSound(sound));
            }
        }
    }

    IEnumerator GetAudioClip(string name, Action<AudioClip> result)
    {
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(
            "SFX/" + name + ".wav"
        );
        while (!handle.IsDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            result(handle.Result);
        }
        else
        {
            Debug.LogWarning("Could not find sound" + name);
        }
    }
}
