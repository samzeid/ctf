using System.Linq;
using ByteSprite.OpenXR;
using UnityEngine;

namespace Common.Scripts {
  public class LoadController : MonoBehaviour {
    [SerializeField]
    FixedJointSlot[] socketInteractors;

    public int GetNumRopesConnected()
    {
      int result = socketInteractors.Count(s => s.isConnected);

      Debug.Log("GetNumRopesConnected() : " + result);

      if (result == 4) {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
      }

      return result;
    }
  }
}
