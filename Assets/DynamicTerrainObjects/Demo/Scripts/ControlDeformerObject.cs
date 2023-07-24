using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ControlDeformerObject : MonoBehaviour {

    public GameObject buttons;
    public Vector3 maxScale = new Vector3(10, 10, 10);
    public Vector3 minScale = new Vector3(1, 1, 1);

    private Terrain terrain;  
    private Vector3 origScale;
    private float maxScaleDistance;
    private float minScaleDistance;
    private Dictionary<string, GameObject> canvasObjects = new Dictionary<string, GameObject>();

    void Start() {
        if (this.terrain == null)
            this.terrain = Terrain.activeTerrain;
                
        origScale = transform.localScale;
        maxScaleDistance = Vector3.Distance(origScale, maxScale);
        minScaleDistance = Vector3.Distance(Vector3.zero, minScale);

        foreach (Transform t in buttons.transform) {
            canvasObjects.Add(t.name, t.gameObject);
        }
    }

    void Update() {
        if(EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {           
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                UpdateLocalScale();
            } else {
                transform.Translate(Vector3.up * Input.GetAxis("Mouse ScrollWheel"));
            }
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                UpdateLocalScale();
            } else {
                transform.Translate(Vector3.down * -Input.GetAxis("Mouse ScrollWheel"));
            }
        }

        if (Input.GetKeyDown("1")) {
            canvasObjects["Add"].GetComponent<Button>().onClick.Invoke();
            DebugHelpers.DestroyObjects();
        }

        if (Input.GetKeyDown("2")) {
            canvasObjects["Subtract"].GetComponent<Button>().onClick.Invoke();
            DebugHelpers.DestroyObjects();
        }

        if (Input.GetKeyDown("3")) {
            canvasObjects["Embrace"].GetComponent<Button>().onClick.Invoke();
            DebugHelpers.DestroyObjects();
        }

        if (Input.GetKeyDown("4")) {
            canvasObjects["Smooth"].GetComponent<Button>().onClick.Invoke();
            DebugHelpers.DestroyObjects();
        }

        if (Input.GetKeyDown("5")) {
            canvasObjects["Texture"].GetComponent<Button>().onClick.Invoke();
            DebugHelpers.DestroyObjects();
        }

        if (Input.GetKeyDown("6")) {
            DebugHelpers.DestroyObjects();
            canvasObjects["Flatten"].GetComponent<Button>().onClick.Invoke();
        }

        if (Input.GetKeyDown("7")) {
            DebugHelpers.DestroyObjects();
            canvasObjects["Show Area Vertices"].GetComponent<Button>().onClick.Invoke();
        }

        if (Input.GetKeyDown("8")) {
            DebugHelpers.DestroyObjects();
            canvasObjects["Show Alpha Columns"].GetComponent<Button>().onClick.Invoke();
        }

        if (Input.GetKeyDown("9")) {
            canvasObjects["Reset"].GetComponent<Button>().onClick.Invoke();
        }
    }

    void OnMouseDrag() {        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (terrain.GetComponent<TerrainCollider>().Raycast(ray, out hit, Mathf.Infinity)) {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }        
    }

    private void UpdateLocalScale() {
        float scrollVal = Input.GetAxis("Mouse ScrollWheel");
        Vector3 targetScale = transform.localScale * scrollVal * Time.deltaTime * 20;
        float currDistance = Vector3.Distance(Vector3.zero, transform.localScale);

        if (
            currDistance > maxScaleDistance && scrollVal < 0 ||
            currDistance < minScaleDistance && scrollVal > 0 ||
            currDistance < maxScaleDistance && currDistance > minScaleDistance
           ) {
            transform.localScale += targetScale;
        }
    }

}


