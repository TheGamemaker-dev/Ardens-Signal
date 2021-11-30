using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject soundPrefab;
    [SerializeField] Dictionary<string, AudioClip> sfxs = new Dictionary<string, AudioClip>();

    List<AudioSource> playingSources = new List<AudioSource>();

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
        AudioSource source = Instantiate(soundPrefab, transform).GetComponent<AudioSource>();
        string[] clipParams = clipName.Split('|');
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
}
