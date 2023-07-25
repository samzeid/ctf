using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LoadController : MonoBehaviour {
  [SerializeField]
  XRSocketInteractor[] socketInteractors;

  public int GetNumRopesConnected()
  {
    int result = socketInteractors.Count(s => s.hasSelection);

    Debug.Log("GetNumRopesConnected() : " + result);

    return result;
  }
}
