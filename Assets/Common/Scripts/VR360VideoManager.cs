using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using Valve.VR;

public class VR360VideoManager : MonoBehaviour {

  public VideoPlayer videoPlayer;
  public AudioSource audioSource;

  public GameObject secondaryDisplay;

  private string[] videos;
  private int videoIndex = 0;

  private bool playing;
  private float lastProximityTime = 0;
  private readonly float INACTIVITY_TIMEOUT_DURATION = 3.0f;

  // Use this for initialization
  void Start () {

    FindVideos();

    Debug.Assert(videos.Length > 0);
    videoIndex = 0;

    videoPlayer.loopPointReached += VideoLoopPointReached;
    videoPlayer.prepareCompleted += VideoPrepareCompleted;

    playing = false;
	}

  void VideoLoopPointReached(VideoPlayer vp)
  {
    Debug.Log("LoopPointReached() video " + videoIndex);

    NextVideo();
  }

  void VideoPrepareCompleted(VideoPlayer vp)
  {
    Debug.Log("VideoPrepareCompleted() video " + videoIndex);

    videoPlayer.Play();
  }

  void PlayVideo(int index)
  {
    Debug.Log("PlayVideo(" + index + ")");

    string filePath = Path.Combine(Application.streamingAssetsPath, videos[index]);

    videoPlayer.source = VideoSource.Url;
    videoPlayer.url = filePath;

    videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

    videoPlayer.EnableAudioTrack(1, true);
    videoPlayer.SetTargetAudioSource(0, audioSource);

    videoPlayer.Prepare();
  }

  void NextVideo()
  {
    videoIndex = (videoIndex + 1) % videos.Length;
    Debug.Log("advance videoIndex to:" + videoIndex);
    PlayVideo(videoIndex);
  }

	// Update is called once per frame
	void Update () {
    //TOFIX: Compilation placeholder
    bool proximitySensor = ByteSprite.VR.IsHeadsetWorn(); //headset.GetPress(Valve.VR.EVRButtonId.k_EButton_ProximitySensor);

    if (proximitySensor)
    {
      lastProximityTime = Time.time;

      if (!playing)
      {
        EnableSecondaryDisplay(false);
        Debug.Log("Play");
        NextVideo();
        playing = true;
      }
    }
    else
    {
      if (playing)
      {
        Debug.Log("Stop");
        videoPlayer.Stop();
        playing = false;
      }
    }

    if (!proximitySensor && Time.time > lastProximityTime + INACTIVITY_TIMEOUT_DURATION)
    {
      EnableSecondaryDisplay(true);
    }
  }

  private void FindVideos()
  {
    string path = Path.Combine(Application.streamingAssetsPath, "Videos");
    string[] files = Directory.GetFiles(path, "*.mp4");
    videos = new string[files.Length];

    Debug.Log("Found " + files.Length + " files in source");

    for(int i = 0; i < files.Length; i++)
    {
      videos[i] = files[i];
    }
  }

  private void EnableSecondaryDisplay(bool isEnabled)
  {
    if (secondaryDisplay != null)
    {
      Debug.Log("EnableSecondaryDisplay(): " + isEnabled);
      secondaryDisplay.SetActive(isEnabled);
    }
    else
    {
      Debug.LogWarning("Missing secondary display GameObject");
    }
  }
}
