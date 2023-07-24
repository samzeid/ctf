using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DynamicTerrainObjects;

public class DebugHelpers : MonoBehaviour {

    protected List<GameObject> debugObjects;

    #region Singleton

    static DebugHelpers instance;

    public static DebugHelpers Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<DebugHelpers>();
                if (instance == null) {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    instance = obj.AddComponent<DebugHelpers>();
                    instance.debugObjects = new List<GameObject>();
                }
            }
            return instance;
        }
    }

    #endregion
    
	public static void CreateDebugSphere(Vector3 pos, Color color, float scale = 0.2f) {
		GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
        ScaleAndPositionDebugObject(sphere, pos, color, scale);
		Destroy(sphere.GetComponent<SphereCollider>());
        Instance.debugObjects.Add(sphere);
	}

    public static void CreateDebugCube(Vector3 pos, Color color, float scale = 0.2f) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ScaleAndPositionDebugObject(cube, pos, color, scale);
        Destroy(cube.GetComponent<BoxCollider>());
        Instance.debugObjects.Add(cube);
    }

    private static void ScaleAndPositionDebugObject(GameObject obj, Vector3 pos, Color color, float scale) {
        obj.transform.localScale = new Vector3(scale, scale, scale);
        obj.transform.position = pos;

        Renderer rend = obj.GetComponent<Renderer>();
        rend.material.color = color;
        rend.receiveShadows = false;
        rend.useLightProbes = false;
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public static void VisualizeTerrainAreaAlphaColumns(GameObject gameObject, LocalAlphaMap.ColliderFitType fitType = LocalAlphaMap.ColliderFitType.VertexFit) {
        Area tArea = new Area(gameObject, Terrain.activeTerrain, 1);
        LocalAlphaMap localAlphaMap = new LocalAlphaMap(
            gameObject.GetComponent<Collider>(), tArea, fitType
        );
        VisualizeTerrainAreaAlphaColumns(localAlphaMap, tArea);
    }

    public static void VisualizeTerrainAreaAlphaColumns(LocalAlphaMap localAlphaMap, Area tArea) {
        
        foreach (Column column in tArea.alphaColumns) {
            Color c;
            if (localAlphaMap.localAlphamapColumns[column.areaZ, column.areaX].insideDeformerCollider) {
                c = Color.red;
            } else if (
                (column.areaX == 0 && column.areaZ == 0) ||
                (column.areaX == tArea.alphaColumnSizeX - 1 && column.areaZ == tArea.alphaColumnSizeZ - 1)
            ) {
                c = Color.blue;
            } else {
                c = Color.green;
            }
            DebugHelpers.CreateDebugCube(column.WorldPos(), c, 0.2f);
        }
    }


    public static void VisualizeTerrainAreaVertices(GameObject gameObject, LocalHeightMap.ColliderFitType fitType = LocalHeightMap.ColliderFitType.VertexFit) {
        Area tArea = new Area(gameObject, Terrain.activeTerrain, 1);
        LocalHeightMap localHeightMap = new LocalHeightMap(gameObject.GetComponent<Collider>(), tArea, fitType);
        VisualizeTerrainAreaVertices(localHeightMap, tArea);       
    }

    public static void VisualizeTerrainAreaVertices(LocalHeightMap localHeightMap, Area tArea) {
        foreach (Vertex v in tArea.vertices) {
            Color c;

            bool isFirstOrLastVert = (v.areaX == 0 && v.areaZ == 0) ||
                (v.areaX == tArea.vertSizeX - 1 && v.areaZ == tArea.vertSizeZ - 1);

            if (v.distanceFromNearestBorder == 0 && !isFirstOrLastVert) {
                c = new Color32(171, 123, 113, 1);
            } else
            if (v.distanceFromNearestBorder == 1) {
                c = new Color32(100, 165, 151, 1);
            }
            else
            if (v.isCenter) {
                c = Color.black;
            } else if (localHeightMap.localHeightmapVertices[v.areaZ, v.areaX].insideDeformerCollider) {
                c = Color.red;            
            } else if (isFirstOrLastVert) {
                c = Color.blue;
            } else {
                c = Color.gray;
            }
            DebugHelpers.CreateDebugSphere(v.WorldPos(), c);
        }
    }

	public static void CreateDebugSpheresForTerrainBounds(Terrain terrain) {
		foreach (Vertex pos in TerrainHelpers.TerrainBounds(terrain)) {
			CreateDebugSphere(pos.WorldPos(), Color.green);
		}
	}

    public static void DestroyObjects() {
        foreach (GameObject debugObj in Instance.debugObjects) {
            DestroyImmediate(debugObj);
        }
    }

    public static void CreateTextMesh(Transform textMesh, Vector3 position, float value) {
        Transform txtMeshTransform = (Transform)Instantiate(textMesh);
        txtMeshTransform.position = position;
        TextMesh txtMesh = txtMeshTransform.GetComponent<TextMesh>();
        txtMesh.text = GetTwoFractionalDigitString(value);
        txtMesh.color = Color.red; // Set the text's color to red
    }

    private static string GetTwoFractionalDigitString(float input) {
        // Parse exponential-notation string to find exponent (e.g. 1.2E-004)
        float absValue = Mathf.Abs(input);
        float fraction = (absValue - Mathf.Floor(absValue));
        string s1 = fraction.ToString("E1");
        // parse exponent peice (starting at 6th character)
        int exponent = int.Parse(s1.Substring(5)) + 1;

        string s = input.ToString("F" + exponent.ToString());

        return s;
    }
}
