using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AnimationObjectController : MonoBehaviour {

    public Toggle deformerToggle;

    protected Dictionary<GameObject, Vector3> defaultPositions;
    protected Dictionary<GameObject, Vector3> defaultScales;

    void Start() {
        deformerToggle.isOn = false;

        // turn off any deformer objects until they are enabled via button
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        defaultPositions = new Dictionary<GameObject, Vector3>();
        defaultScales = new Dictionary<GameObject, Vector3>();

        foreach (Transform child in transform) {
            defaultPositions.Add(child.gameObject, child.position);
            defaultScales.Add(child.gameObject, child.localScale);
        }
    }

    public void Reset() {
        StopAllCoroutines();
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
            child.position = defaultPositions[child.gameObject];
            child.localScale = defaultScales[child.gameObject];
            child.gameObject.SetActive(false);
        }
        BackupHeightmap.Instance.RestoreTerrainHeightmap();
    }

    public void CreateHill() {
        Reset();
        GameObject sphere = EnableAnimationObject("Hill");
        Vector3 goalPos = sphere.transform.position + new Vector3(0, 5, 0);
        Vector3 goalScale = sphere.transform.localScale + new Vector3(2, 2, 2);        
        StartCoroutine(MoveAndScaleLinear(sphere, goalPos, goalScale, 5f, new string[] { "Add", "Smooth" }));
    }

    public void CreateSinkhole() {
        Reset();
        GameObject sphere = EnableAnimationObject("Sinkhole");
        Vector3 goalPos = sphere.transform.position + new Vector3(0, -5, 0);
        Vector3 goalScale = sphere.transform.localScale + new Vector3(2, 2, 2);
        Deformer deformer = sphere.GetComponent<Deformer>();
        deformer.TextureNow();
        StartCoroutine(MoveAndScaleLinear(sphere, goalPos, goalScale, 5f, new string[] { "Subtract", "Smooth" }));
    }

    public void CreateMole() {
        Reset();
        GameObject sphere = EnableAnimationObject("Mole");
        Deformer deformer = sphere.GetComponent<Deformer>();
        deformer.terrainTextureIndex = 3;
        deformer.smoothAmount = 0.1f;
        Vector3 goalDirection = Vector3.forward;
        float goalDistance = 70f;
        Vector3 goalScale = sphere.transform.localScale;
        StartCoroutine(MoveAndScaleParabolic(sphere, goalDirection, goalDistance, goalScale, 5f, new string[] { "Add", "Smooth", "Texture" }));        
    }

    public void CreateFissure() {
        Reset();
        GameObject fissure = EnableAnimationObject("Fissure");
        Vector3 goalPosition = fissure.transform.position + (Vector3.down * 15f);
        Vector3 goalScale = fissure.transform.localScale;
        Deformer deformer = fissure.GetComponent<Deformer>();
        StartCoroutine(MoveAndScaleLinear(fissure, goalPosition, goalScale, 5f, new string[] { "Subtract" }));        
        deformer.terrainTextureIndex = 3;
        DeformTerrain(deformer, "Texture");
        deformer.SetAlphas();
    }

    public void RenderTestObjMesh() {
        foreach (Transform child in transform) {
            child.GetComponent<Renderer>().enabled = deformerToggle.isOn;
        }
    }

    protected GameObject EnableAnimationObject(string objName) {
        GameObject targetObject = null;

        foreach (Transform child in transform) {
            if (child.name == objName) {
                child.gameObject.SetActive(true);
                targetObject = child.gameObject;
                targetObject.GetComponent<Deformer>().Init();
            }
            else {
                child.gameObject.SetActive(false);
            }
        }

        return targetObject;
    }

    protected IEnumerator MoveAndScaleLinear(GameObject obj, Vector3 goalPos, Vector3 goalScale, float moveSpeed, string[] commands) {
        Deformer deformer = obj.GetComponent<Deformer>();
        while (true) {
            if (!obj.activeSelf)
                continue;

            Vector3 startPos = obj.transform.position;
            Vector3 startScale = obj.transform.localScale;

            if (startPos == goalPos)
                break;

            obj.transform.position = Vector3.MoveTowards(startPos, goalPos, Time.deltaTime * moveSpeed);
            obj.transform.localScale = Vector3.MoveTowards(startScale, goalScale, Time.deltaTime * moveSpeed);

            foreach (string command in commands) { DeformTerrain(deformer, command); }

            deformer.SetHeights();

            yield return null;
        }
    }

    protected IEnumerator MoveAndScaleParabolic(GameObject obj, Vector3 goalPos, float goalDistance, Vector3 goalScale, float moveSpeed, string[] commands) {
        Vector3 origPosition = obj.transform.position;
        Deformer deformer = obj.GetComponent<Deformer>();

        while (true) {
            if (!obj.activeSelf)
                continue;

            Vector3 startPos = obj.transform.position;
            Vector3 startScale = obj.transform.localScale;

            if (Vector3.Distance(origPosition, startPos) > goalDistance)
                break;            

            float frequency = 2.0f;  // Speed of sine movement
            float magnitude = 0.3f;   // Size of sine movement

            startPos += transform.right * Time.deltaTime * moveSpeed;
            obj.transform.position = startPos + goalPos.normalized * Mathf.Sin(Time.time * frequency) * magnitude;
            obj.transform.localScale = Vector3.MoveTowards(startScale, goalScale, Time.deltaTime * moveSpeed);

            foreach (string command in commands) { DeformTerrain(deformer, command); }

            deformer.SetHeights();

            yield return null;
        }
    }

    protected void DeformTerrain(Deformer deformer, string command) {
        switch (command) {
            case "Add":
                deformer.GetComponent<Deformer>().Add();
                break;
            case "Smooth":
                deformer.GetComponent<Deformer>().Smooth();
                break;
            case "Subtract":
                deformer.GetComponent<Deformer>().Subtract();
                break;
            case "Texture":
                deformer.GetComponent<Deformer>().Texture();
                break;
            default:
                break;
        }
    }
}
