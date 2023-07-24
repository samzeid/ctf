using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TutorialManager {

  private List<TutorialEvent> events;
  private int eventIndex;
  private bool isComplete;

  public const float PROXIMITY_SENSOR_TIMEOUT = 5.0f;

  public TutorialManager() {
    events = new List<TutorialEvent>();
    eventIndex = -1;
    isComplete = false;
	}
	
	// Update is called once per frame
	public void Update (GameObject gameObject) {
    if (isComplete)
      return;

    if (events == null)
      return;

    if (eventIndex == -1)
      return;

    TutorialEvent te = events[eventIndex];

    // if current event is now complete, move to the next event
		if (te.IsComplete(gameObject))
    {
      NextEvent(gameObject);
    }
	}

  public void AddEvent(TutorialEvent tutorialEvent)
  {
    events.Add(tutorialEvent);
  }

  public void Restart(GameObject gameObject)
  {
    Debug.Log("Restart()");

    isComplete = false;
    eventIndex = 0;
    events[0].Begin(gameObject);
  }

  public void NextEvent(GameObject gameObject)
  {
    Debug.Log("NextEvent() " + (eventIndex + 1));
    events[eventIndex].End(gameObject);

    if (eventIndex < events.Count - 1)
    {
      eventIndex += 1;
      events[eventIndex].Begin(gameObject);
    }
    else
    {
      Debug.Log("Reached end of events!");
      isComplete = true;
    }
  }

  public bool IsComplete()
  {
    return isComplete;
  }
}
