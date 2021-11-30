using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject soundPrefab;
    [SerializeField] Dictionary<string, AudioClip> sfxs = new Dictionary<string, AudioClip>();

    List<AudioSource> playingSources = new List<AudioSource>();

    string[] ambientSounds = { "Fan Whirring" };

    void Awake()
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Sound Effects");
        foreach (AudioClip clip in audioClips)
        {
            sfxs.Add(clip.name, clip);
        }
    }

    public void PlaySound(string clipName)
    {
        string[] clipParams = clipName.Split('|');

        if (Array.IndexOf(ambientSounds, clipParams[0]) == -1)
        {
            AudioSource source = Instantiate(soundPrefab, transform).GetComponent<AudioSource>();
            if (clipParams.Length == 2)
            {
                if (clipParams[1] == "loop")
                {
                    source.loop = true;
                }
            }
            else
            {
                source.loop = false;
            }
            source.clip = sfxs[clipParams[0]];
            playingSources.Add(source);
            source.Play();
            StartCoroutine(DestroySound(source));
        }
        else
        {
            StartCoroutine(PlayAmbientSound(clipParams[0]));
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
        AudioClip startClip = sfxs[soundName + " (start)"];
        AudioClip ambientClip = sfxs[soundName + " (ambient)"];
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
        AudioSource source = playingSources.First(x => x.clip == sfxs[soundName + " (ambient)"]);
        source.volume = 0.25f;
        AudioClip endClip = sfxs[soundName + " (end)"];

        AudioSource source1 = Instantiate(soundPrefab, transform).GetComponent<AudioSource>();
        source1.volume = 0.25f;
        source1.clip = endClip;

        source1.PlayScheduled(AudioSettings.dspTime + 1.0f);

        yield return new WaitForSeconds(1.0f);

        playingSources.Remove(source);
        Destroy(source.gameObject);

        yield return new WaitForSeconds(endClip.length);

        playingSources.Remove(source1);
        Destroy(source1.gameObject);
    }

    void OnDayEnd(string flag)
    {
        if (flag.Contains("Done"))
        {
            StartCoroutine(StopAmbientSound("Fan Whirring"));
        }
    }

    void OnEnable()
    {
        GameManager.onFlagSet += OnDayEnd;
    }

    void OnDisable()
    {
        GameManager.onFlagSet -= OnDayEnd;
    }
}
