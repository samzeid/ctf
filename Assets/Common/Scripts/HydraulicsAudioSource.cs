using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicsAudioSource : MonoBehaviour {

  const float MOTOR_ON_PITCH = 1.1f;
  const float MOTOR_OFF_PITCH = 0.9f;

  public float motorOnVolume;
  public float motorOffVolume;

  [Header("Hydraulics SFX")]
  public AudioClip start;
  public AudioClip loop;
  public AudioClip stop;

  private MachineController machineController;

  private AudioSource startStop;
  private AudioSource motor;

  private float motorVolume;
  private float motorPitch = MOTOR_OFF_PITCH;
  private bool moving = false;

  // Use this for initialization
  void Start () {
    machineController = GetComponent<MachineController>();

    motorVolume = motorOffVolume;
    motor = GetComponents<AudioSource>()[1];
    startStop = GetComponents<AudioSource>()[0];
  }

  // Update is called once per frame
  void Update()
  {
    // check if still moving
    bool stillMoving = machineController.IsMoving();

    // update hydraulics sound
    if (moving && !stillMoving)
    {
      //Debug.Log("End");
      motorVolume = motorOffVolume;
      motorPitch = MOTOR_OFF_PITCH;
      startStop.PlayOneShot(stop);
    }
    if (!moving && stillMoving)
    {
      //Debug.Log("Start");
      motorVolume = motorOnVolume;
      motorPitch = MOTOR_ON_PITCH;
      startStop.PlayOneShot(start);
    }
    moving = stillMoving;

    motor.volume = Mathf.Lerp(motor.volume, motorVolume, 0.1f);
    motor.pitch = Mathf.Lerp(motor.pitch, motorPitch, 0.025f);
  }
}
