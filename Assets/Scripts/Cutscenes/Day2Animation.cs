using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Day2Animation : MonoBehaviour
{
    VideoPlayer player;

    void Start()
    {
        player = GetComponent<VideoPlayer>();
    }

    public void Play()
    {
        player.Play();
    }
}
