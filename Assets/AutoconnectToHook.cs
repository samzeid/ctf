using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoconnectToHook : MonoBehaviour
{
  public bool levelComplete = false;
  public static int numLoadsPickedUp = 0;
  public static int numLoadsDroppedOff = 0;

  private CraneTutorial craneTutorial;

  // Use this for initialization
  void Start()
  {
    craneTutorial = GameObject.Find("CraneTutorialManager").GetComponent<CraneTutorial>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public static void Reset()
  {
    numLoadsPickedUp = 0;
    numLoadsDroppedOff = 0;
  }

  void OnTriggerEnter(Collider other)
  {
    if ((other.gameObject.name == "Hook") && (levelComplete == false) && !craneTutorial.GetIsCarrying())
    {
      Destroy(GetComponent<Rigidbody>());
      transform.SetParent(other.transform);
      transform.localPosition = new Vector3(0f, -4.79f, 0f);
      transform.localEulerAngles = new Vector3(0f, 0f, 0f);
      transform.Find("rope").gameObject.SetActive(true);

      craneTutorial.SetIsCarrying(true);

      numLoadsPickedUp += 1;
      craneTutorial.TriggerAchievement("Pick up Load No. " + numLoadsPickedUp);
    }

    if (other.gameObject.name == "LoadEndPosition" && (numLoadsPickedUp > numLoadsDroppedOff) && craneTutorial.GetIsCarrying())
    {
      gameObject.AddComponent(typeof(Rigidbody));
      transform.parent = null;

      levelComplete = true;

      GetComponent<AudioSource>().Play();
      transform.Find("rope").gameObject.SetActive(false);

      craneTutorial.SetIsCarrying(false);

      numLoadsDroppedOff += 1;
      craneTutorial.TriggerAchievement("Deliver Load No. " + numLoadsDroppedOff);
    }
  }
}
