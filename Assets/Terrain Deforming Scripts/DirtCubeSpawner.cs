using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DirtCubeSpawner : MonoBehaviour {

    [SerializeField] GameObject[] DirtBallPrefabs;

    private bool CanWeSpawnDirt = true;
    private float forceScaler = 0.01f;

  public void SpawnDirtBall(Vector3 _position, Vector3 _forwardDirection, float dirtVolume)
  {
    // randomise position
    _position.x += Random.Range(-0.3f, 0.3f);
    _position.z += Random.Range(-0.3f, 0.3f);

    //spawn 1st ball
    StartCoroutine(SpawnCoroutine(_position, _forwardDirection));

    //spawn 2nd ball
    if (dirtVolume > 0.004f)
    {
      _position.y += 0.15f;
      StartCoroutine(SpawnCoroutine(_position, _forwardDirection));
    }

    //spawn 2nd ball
    if (dirtVolume > 0.008f)
    {
      _position.y += 0.15f;
      StartCoroutine(SpawnCoroutine(_position, _forwardDirection));
    }

    //spawn 2nd ball
    if (dirtVolume > 0.012f)
    {
      _position.y += 0.15f;
      StartCoroutine(SpawnCoroutine(_position, _forwardDirection));
    }

    //spawn 3rd ball
    if (dirtVolume > 0.016f)
    {
      _position.y += 0.15f;
      StartCoroutine(SpawnCoroutine(_position, _forwardDirection));
    }
  }

    IEnumerator SpawnCoroutine(Vector3 _position, Vector3 _forwardDirection)
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.1f));

        GameObject randomPrefab = DirtBallPrefabs[Random.Range(0, DirtBallPrefabs.Length)];

        GameObject dirtBall = Instantiate(randomPrefab, _position, Quaternion.identity) as GameObject;
        dirtBall.transform.parent = this.transform;
        dirtBall.GetComponent<Rigidbody>().AddForce(forceScaler * _forwardDirection, ForceMode.VelocityChange);

        yield return null;
    }
}