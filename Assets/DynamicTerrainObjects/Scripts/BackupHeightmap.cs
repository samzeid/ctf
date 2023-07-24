using UnityEngine;
using System.Collections;

public class BackupHeightmap : MonoBehaviour {

    public Terrain terrain;
    private TerrainData terrainData;

    private int terrainWidth;
    private int terrainHeight;
    private int terrainAlphaWidth;
    private int terrainAlphaHeight;
    private float[,] heightMapBackup;
    private float[,,] alphaMapBackup;
    private bool initialized = false;

    #region Singleton

    static BackupHeightmap instance;

    public static BackupHeightmap Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<BackupHeightmap>();
                if (instance == null) {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    instance = obj.AddComponent<BackupHeightmap>();
                }
            }
            return instance;
        }
    }

    #endregion

    public void SaveTerrain(Terrain terrain) {
        if (this.initialized) { return; }

        terrainData = terrain.terrainData;
        terrainWidth = terrainData.heightmapResolution;
        terrainHeight = terrainData.heightmapResolution;
        terrainAlphaWidth = terrainData.alphamapWidth;
        terrainAlphaHeight = terrainData.alphamapHeight;

        this.heightMapBackup = terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);
        this.alphaMapBackup = terrainData.GetAlphamaps(0, 0, terrainAlphaWidth, terrainAlphaHeight);
        this.initialized = true;
    }

    public void RestoreTerrainHeightmap() {
        terrainData.SetHeights(0, 0, this.heightMapBackup);
        terrainData.SetAlphamaps(0, 0, this.alphaMapBackup);
    }

}
