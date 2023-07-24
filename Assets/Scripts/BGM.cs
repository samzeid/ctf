using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour {

  public AudioClip gameplayBGM;
  public AudioClip highScoreBGM;

  private AudioSource audioSource;
  private static BGM instance = null;

  public static BGM GetInstance()
  {
    return instance;
  }

  void Awake()
  {
    audioSource = GetComponent<AudioSource>();

    if (instance == null)
    {
      instance = this;
    }
    else if (instance != this)
    {
      Destroy(gameObject);
    }
  }

  // Use this for initialization
  void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void PlayGameplayBGM()
  {
    audioSource.clip = gameplayBGM;
    audioSource.Play();
  }

  public void PlayHighScoreBGM()
  {
    Debug.Log("AudioSource" + audioSource);
    Debug.Log("clip" + audioSource.clip);
    audioSource.clip = highScoreBGM;
    audioSource.Play();
  }
}
