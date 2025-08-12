using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    //public AudioSource src;

    void Start()
    {
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare(); // Preload the video before playing
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play(); // Play only when it's fully prepared
    }
}
