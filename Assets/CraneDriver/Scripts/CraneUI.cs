using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraneUI : MonoBehaviour
{
  public const float WIND_SPEED_PERIOD = 2.0f;

  public CraneController craneController;

  [Header("Computer Screen UI")]
  public Text slewText;
  public Text trolleyText;
  public Text hookText;
  public Text speedText;
  public Text loadText;
  public Text maxLoadText;
  public Text windSpeedText;
  public Text maxTrolleyText;

  float lastWindSpeed = 0.0f;
  float lastWindSpeedTime = 0.0f;

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    UpdateWind();

    if (slewText)
      slewText.text = craneController.GetSlewRotation().ToString("F3");

    if (trolleyText)
      trolleyText.text = craneController.GetTrolleyPosition().ToString("F3");

    if (hookText)
      hookText.text = craneController.GetHookPosition().ToString("F3");

    if (speedText)
      speedText.text = "Max";

    if (loadText)
    {
      if (craneController.GetHasLoad())
      {
        loadText.text = "400kg";
      }
      else
      {
        loadText.text = "0kg";
      }
    }

    if (maxLoadText)
      maxLoadText.text = "800kg";

    if (windSpeedText)
      windSpeedText.text = lastWindSpeed.ToString("F1") + "m/s";

    if (maxTrolleyText)
      maxTrolleyText.text = CraneController.MAX_TROLLEY_POSITION.ToString("F3");
  }

  void UpdateWind()
  {
    // every X seconds, update wind speed
    if (Time.time >= lastWindSpeedTime + WIND_SPEED_PERIOD)
    {
      lastWindSpeed = Random.Range(0.0f, 10.0f);
      lastWindSpeedTime = Time.time;
    }
  }
}
