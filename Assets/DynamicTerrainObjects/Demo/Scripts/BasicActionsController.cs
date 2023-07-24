using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BasicActionsController : MonoBehaviour {

	public UnityEngine.UI.Dropdown objectDropdown;
    public UnityEngine.UI.Dropdown alphaIndexDropdown;
    public UnityEngine.UI.Toggle toggle;
    public UnityEngine.UI.Slider smoothAmountSlider;
    public UnityEngine.UI.Slider flattenToSlider;
    public UnityEngine.UI.Toggle flattenAreaToggle;

    public UnityEngine.UI.Slider alphaOpacitySlider;
    public UnityEngine.UI.InputField areaPaddingInput;
    public UnityEngine.UI.InputField alphaPaddingInput;    

    protected Dictionary<GameObject, Vector3> defaultPositions;
    protected Dictionary<GameObject, Vector3> defaultScales;

    void Start() {        
        defaultPositions = new Dictionary<GameObject, Vector3>();
        defaultScales = new Dictionary<GameObject, Vector3>();

        foreach (Transform child in transform) {
            defaultPositions.Add(child.gameObject, child.position);
            defaultScales.Add(child.gameObject, child.localScale);
        }

        objectDropdown.value = 0;
        ChangeTestObject();

        flattenAreaToggle.isOn = false;

        smoothAmountSlider.value = 1f;
        UpdateSmoothAmount();

        alphaIndexDropdown.value = 1;
        UpdateAlphaIndex();

        alphaOpacitySlider.value = 1f;
        UpdateAlphaOpacity();

        alphaPaddingInput.text = "0";
        UpdateAlphaPadding(alphaPaddingInput.text);

        areaPaddingInput.text = "10";
        UpdateAreaPadding(areaPaddingInput.text);

        flattenToSlider.value = 0.5f;
        UpdateFlattenTo();
    }

    public void ChangeTestObject() {
        DebugHelpers.DestroyObjects();
        foreach (Transform child in transform) {
            if (objectDropdown.captionText.text == child.name) {
                child.gameObject.SetActive(true);
            } else {
                child.gameObject.SetActive(false);
            }
        }        
	}

    public void RenderTestObjMesh() {
        foreach (Transform child in transform) {
            child.GetComponent<Renderer>().enabled = toggle.isOn;
        }
    }

    public void FlattenArea() {
        foreach (Transform child in transform) {
            child.GetComponent<Deformer>().flattenArea = flattenAreaToggle.isOn;
        }
    }
    
    public void UpdateAreaPadding(string value) {
        DebugHelpers.DestroyObjects();
        int intValue = 0;
        if (int.TryParse(value, out intValue)) {
            foreach (Transform child in transform) {
                child.GetComponent<Deformer>().terrainAreaPadding = intValue;
            }
        }        
    }

    public void UpdateSmoothAmount() {
        DebugHelpers.DestroyObjects();
        foreach (Transform child in transform) {
            child.GetComponent<Deformer>().smoothAmount = smoothAmountSlider.value;
        }
    }

    public void UpdateAlphaPadding(string value) {
        DebugHelpers.DestroyObjects();
        int intValue = 0;
        if (int.TryParse(value, out intValue)) {
            foreach (Transform child in transform) {
                child.GetComponent<Deformer>().alphaPadding = intValue;
            }
        }
    }

    public void UpdateAlphaIndex() {
        DebugHelpers.DestroyObjects();
        foreach (Transform child in transform) {
            child.GetComponent<Deformer>().terrainTextureIndex = alphaIndexDropdown.value;
        }        
    }

    public void UpdateAlphaOpacity() {
        DebugHelpers.DestroyObjects();
        foreach (Transform child in transform) {
            child.GetComponent<Deformer>().terrainTextureOpacity = alphaOpacitySlider.value;
        }        
    }

    public void Reset() {
        BackupHeightmap.Instance.RestoreTerrainHeightmap();
        DebugHelpers.DestroyObjects();
        foreach (Transform child in transform) {
            child.position = defaultPositions[child.gameObject];
            child.localScale = defaultScales[child.gameObject];
        }
    }

    public void UpdateFlattenTo() {
        DebugHelpers.DestroyObjects();
        foreach (Transform child in transform) {
            child.GetComponent<Deformer>().flattenTo = flattenToSlider.value;
        }
    }
}
