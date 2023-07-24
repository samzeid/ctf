using UnityEngine;
using DynamicTerrainObjects;

public class Deformer : MonoBehaviour {

    private float displacementCounter = 0f;

    public float GetDirtDisplacement;

    [Tooltip("How many vertices should we pad out from the objects bounding box? Default is the objects bounding box + 1")]
    public int terrainAreaPadding = 10;    
    
    [Tooltip("Texture to apply when updating the alphamap. Must be added to the Terrain object via the inspector and then referenced by its index 0, 1, 2, etc")]
    public int terrainTextureIndex = 1;

    [Tooltip("What opacity should the new texture be set to - 0.0 to 1.0")]
    [Range(0f, 1f)]
    public float terrainTextureOpacity = 1f;

    [Tooltip("padding of texture area in terrain units")]
    public int alphaPadding = 1;

    [Tooltip("How much should we smooth the terrain?")]
    [Range(0f, 2f)]
    public float smoothAmount = 1f;

    [Tooltip("Should the alphamap be changed for any column that is touching the deformer (column fit) or should we only change for cells completely inside the collider (vertex fit)")]
    public LocalAlphaMap.ColliderFitType AlphaFitType;

    [Tooltip("Should the heightmap be changed for any column that is touching the deformer (column fit) or should we only change for cells completely inside the collider (vertex fit)")]
    public LocalHeightMap.ColliderFitType HeightFitType;

    [Tooltip("When flattening, should we raise/lower to the bottom, middle, or top of the Deformer?")]
    [Range(0f, 1f)]
    public float flattenTo;

    [Tooltip("When flattening, should we flatten the entire Area (Deformer object + area padding) or just the footprint of the Deformer?")]
    public bool flattenArea = false;

    [Tooltip("If checked, any change to the position or rotation of the game object with this script will update the location and size of the Terrain Area so that subsequent terrain modifications will occur in the correct location and in the correct shape. Turning this on will have some performance impacts. deformer.UpdateAreaPosition() should be called manually if needed.")]
    public bool keepTerrainAreaUpdated = true;

    [Tooltip("If true, any changes made to the terrain data during runtime will be restored to its original state when the application exits.")]
    public bool restoreTerrainOnExit = true;

    [Tooltip("If true, the delayed version of SetHeights() will be called, which is faster, but you must call Terrain.ApplyDelayedHeightmapModification manually to update the terrain LOD")]
    public bool useDelayedLOD = true; 

    [Tooltip("The terrain instance you want this object to modify. Defaults to Terrain.activeTerrain if not set.")]
    public Terrain terrain;

    [HideInInspector]
    public Area terrainArea;

    private Collider deformerCollider;
    private LocalHeightMap localHeightMap;
    private LocalAlphaMap localAlphaMap;
    private Quaternion previousRotation;
    private Vector3 previousLocalScale; 
    private int previousAreaPadding;
    private int previousAlphaPadding;
    private bool previouslyOverlappingTerrainBounds;
    private bool forceLocalMapUpdate;

    void Awake() {
        if (this.terrain == null)
            this.terrain = Terrain.activeTerrain; // if not set explicitly, default to activeTerrain

        // Singleton instance that holds a backup of the terrain heightmap data
        // so that it can be restored when the game exits (if restoreTerrainOnExit is set to true)
        BackupHeightmap.Instance.SaveTerrain(this.terrain);      

        Init(); // broken out into separate method so (re)initialization can be done manually.

    }

    public void Init() {
        this.deformerCollider = GetComponent<Collider>();

        // tracks whether the object has changed position, scale, rotation, etc so that updates 
        // can be made automatically (if keepTerrainAreaUpdated is set to true)
        transform.hasChanged = false;

        // track rotation, scale, paddings so the terrain area can be kept up to date
        this.previousRotation = transform.rotation;
        this.previousLocalScale = transform.localScale;
        this.previousAreaPadding = terrainAreaPadding;
        this.previousAlphaPadding = alphaPadding;

        // ensure that opacity is a value between 0f and 1f
        this.terrainTextureOpacity = Mathf.Clamp(this.terrainTextureOpacity, 0f, 1f);

        // initialize a terrain area object which is our interface between this object and the terrain
        this.terrainArea = new Area(gameObject, this.terrain, this.terrainAreaPadding);

        // holds a mapping of this objects collider geometry converted into terrain space values
        this.localHeightMap = new LocalHeightMap(GetComponent<Collider>(), terrainArea, HeightFitType);

        // holds a mapping of which alpha columns should be adjusted when texturing occurs
        this.localAlphaMap = new LocalAlphaMap(GetComponent<Collider>(), terrainArea, AlphaFitType, alphaPadding);

        this.forceLocalMapUpdate = false;    
    }

    void Update() {
        UnityEngine.Profiling.Profiler.BeginSample("Deformer Update");
        // if keepTerrainAreaUpdated is true then the terrain area's coordinates 
        // will be kept updated on every frame. This can be expensive for performance
        // and should only be used if needing to update the terrain for a moving deformer    
        if (this.keepTerrainAreaUpdated && this.terrain) {

            if (!terrainArea.overlappingTerrainBounds && previouslyOverlappingTerrainBounds) {
                this.forceLocalMapUpdate = true;
            }

            // was any part of the terrain area outside the terrain's geometry?
            // used to force local height and alpha maps to be rebuilt when
            // the deformer moves back within the bounds of the terrain
            previouslyOverlappingTerrainBounds = terrainArea.overlappingTerrainBounds;

            if (transform.hasChanged || PaddingChanged()) {
                UpdateAreaPosition();                
            }

            // if rotation, scale, or the terrain area padding changes then the 
            // local heightmaps need to be recalculated. This is considerably 
            // more expensive than a simple position update (especially if using a 
            // mesh collider as that requires raycasts). Also force an update
            // when this deformer's Area had any of its geometry lying outside of the
            // terrain's geometry during the previous frame
            if (RotationScaleOrPaddingChanged() || this.forceLocalMapUpdate) {
                UpdateLocalMaps();
            }

        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    void OnDisable()
  {
    Debug.Log("OnDisable()");
    if (this.restoreTerrainOnExit)
      BackupHeightmap.Instance.RestoreTerrainHeightmap();
  }

    void OnApplicationQuit() {
        if (this.restoreTerrainOnExit)
            BackupHeightmap.Instance.RestoreTerrainHeightmap();        
    }

    #region PublicMethods

    public void AddNow() {        
        AddVolume();
        SetHeights();
    }

    public void Add() {
        AddVolume();
    }

    public void SubtractNow() {
        SubtractVolume();
        SetHeights();
    }

    public void Subtract() {
        SubtractVolume();
    }

    public void SmoothNow() {
        SmoothVolume(this.smoothAmount);
        SetHeights();
    }

    public void Smooth() {
        SmoothVolume(this.smoothAmount);        
    }

    public void TextureNow() {
        UpdateTexture(terrainTextureIndex, terrainTextureOpacity);
        SetAlphas();
    }

    public void Texture() {
        UpdateTexture(terrainTextureIndex, terrainTextureOpacity);
    }

    public void Flatten() {
        FlattenVolume();
    }

    public void FlattenNow() {
        FlattenVolume();
        SetHeights();
    }

    // if the object is above the terrain, raise terrain up and 
    // craddle the bottom of the deformer. if the object is below,
    // hug the object, slightly displacing on the sides    
    public void Embrace() {
        Transform trans = GetComponent<Transform>();
        Vector3 origPos = trans.position;
        Vector3 offset = new Vector3(0, GetComponent<Collider>().bounds.extents.y, 0);
        trans.position = origPos + offset;         
        AddVolume();
        SmoothVolume();
        trans.position = origPos;
        SubtractVolume();
        SmoothVolume();        
        SetHeights();        
    }

    // true Displace coming soon!
    public void Displace() {
        Embrace();        
    }

    // DEBUG helper
    // Create a grid of spheres that help visualize the terrain area
    // and which vertices are inside of the deformer object's collider
    public void ShowAreaVertices() {
        DebugHelpers.DestroyObjects();
        UpdateAreaPosition();
        UpdateLocalMaps();
        DebugHelpers.VisualizeTerrainAreaVertices(this.localHeightMap, this.terrainArea);
    }

    // DEBUG helper
    // Create a grid of boxes that help visualize the terrain area alpha columns
    // and which of those columns are inside of the deformer object's collider
    public void ShowAreaAlphas() {
        DebugHelpers.DestroyObjects();
        UpdateAreaPosition();
        UpdateLocalMaps();
        DebugHelpers.VisualizeTerrainAreaAlphaColumns(this.localAlphaMap, this.terrainArea);
    }

    public void GetVolume()
    {

    }

    public void DestroyDebugObjects() {
        DebugHelpers.DestroyObjects();
    }

    public void UpdateAreaPosition() {
        this.terrainArea.vertPadding = this.terrainAreaPadding;
        this.terrainArea.alphaPadding = this.alphaPadding;
        this.terrainArea.SetAreaPositionFromGameObj();
        transform.hasChanged = false;
    }

    public void UpdateLocalMaps() {
        this.localHeightMap = new LocalHeightMap(GetComponent<Collider>(), terrainArea, HeightFitType);
        this.localAlphaMap = new LocalAlphaMap(GetComponent<Collider>(), terrainArea, AlphaFitType, alphaPadding);
        transform.hasChanged = false;
        this.previousRotation = transform.rotation;
        this.previousLocalScale = transform.localScale;
        this.previousAreaPadding = terrainAreaPadding;
        this.previousAlphaPadding = alphaPadding;
    }

    public void SetHeights() {
        if (this.useDelayedLOD) {
            terrainArea.SetHeightsDelayLOD();
        } else {
            terrainArea.SetHeights();
        }
    }

    public void SetAlphas() {
        terrainArea.SetAlphas();
    }

    #endregion

    #region PrivateMethods

    private bool RotationScaleOrPaddingChanged() {
        return previousRotation != transform.rotation ||
               previousLocalScale != transform.localScale ||
               PaddingChanged() ||
               terrainArea.overlappingTerrainBounds;
    }

    private bool PaddingChanged() {
        return previousAlphaPadding != alphaPadding || previousAreaPadding != terrainAreaPadding;
    }
    
    private void AddVolume() {
        float curTerrainHeight;
        float deformerPointHeight;

        foreach (Vertex localVertex in this.localHeightMap.localHeightmapVertices) {
            if (localVertex.insideDeformerCollider) {
                curTerrainHeight = terrainArea.terrainHeights[localVertex.areaZ, localVertex.areaX];
                deformerPointHeight = (float)TerrainSpacePosition().y + localVertex.localHeightOffsetMax;

                if (deformerPointHeight > curTerrainHeight) {
                    terrainArea.terrainHeights[localVertex.areaZ, localVertex.areaX] = deformerPointHeight;
                }
            }
        }
    }

    private void SubtractVolume() {
        float curTerrainHeight;
        float deformerPointHeight;

        foreach (Vertex localVertex in this.localHeightMap.localHeightmapVertices)
        {
            if (localVertex.insideDeformerCollider)
            {                
                curTerrainHeight = terrainArea.terrainHeights[localVertex.areaZ, localVertex.areaX];
                deformerPointHeight = (float)TerrainSpacePosition().y + localVertex.localHeightOffsetMin;
                
                if (deformerPointHeight < curTerrainHeight)
                {
                    terrainArea.terrainHeights[localVertex.areaZ, localVertex.areaX] = deformerPointHeight;
                    displacementCounter += curTerrainHeight - deformerPointHeight;
                }
            }
        }
        if (displacementCounter > 0f)
        {
            GetDirtDisplacement = displacementCounter;
            displacementCounter = 0f;
        }
    }

    private void UpdateTexture(int textureIndex, float percent) {
        foreach (Column column in this.localAlphaMap.localAlphamapColumns) {
            if (column.insideDeformerCollider) {
                float alphaChangedBy = percent - terrainArea.terrainAlphas[column.areaZ, column.areaX, textureIndex];
                terrainArea.terrainAlphas[column.areaZ, column.areaX, textureIndex] = percent;

                for (int alpha = 0; alpha < terrainArea.terrainAlphas.GetLength(2); alpha++) {
                    if (alpha != textureIndex) {                      
                        float currValue = terrainArea.terrainAlphas[column.areaZ, column.areaX, alpha];
                        float newValue = currValue - (currValue * alphaChangedBy);
                        terrainArea.terrainAlphas[column.areaZ, column.areaX, alpha] = newValue;
                    }
                }
            }
        }
    }
    
    // loop through each vertex within this Area and average its 
    // height with the heights of its immediate neighbors, weighted
    // by smoothAmount
    private void SmoothVolume(float smoothAmount = 1f) {
        float smoothing;        

        foreach (Vertex v in this.terrainArea.vertices) {

            smoothing = smoothAmount;

            // is this vertex sitting along the border of the terrain area?
            bool borderVert = v.areaZ == 0 || v.areaX == 0 || 
                              v.areaZ == terrainArea.vertSizeZ - 1 || 
                              v.areaX == terrainArea.vertSizeX - 1;

            // if its a border vertex dont smooth
            if (!borderVert) {                

                // if its the next vert in, reduce by less to further ease the transition
                // between verts being smoothed and the rest of the terrain
                if (v.distanceFromNearestBorder < 4 ) {
                    smoothing = smoothAmount * (v.distanceFromNearestBorder / 10f);
                }

                float heightSum = 0f;
                float neighborCount = 0f;
                if (terrainArea.IsVertexInBounds(v.areaZ, v.areaX)) {

                    for (int z = -1; z < 2; z++) {
                        for (int x = -1; x < 2; x++) {
                            if (terrainArea.IsVertexInBounds(v.areaZ + z, v.areaX + x)) {
                                heightSum += this.terrainArea.terrainHeights[v.areaZ + z, v.areaX + x];
                                neighborCount++;
                            }
                        }
                    }
                    float groupAverageHeight = heightSum / neighborCount;
                    float heightDifference = groupAverageHeight - this.terrainArea.terrainHeights[v.areaZ, v.areaX];

                    this.terrainArea.terrainHeights[v.areaZ, v.areaX] += (heightDifference * (smoothing));
                }
            }                                         
        }
    }

    private void FlattenVolume() {
        float flattenToY = Mathf.Lerp(deformerCollider.bounds.min.y, deformerCollider.bounds.max.y, flattenTo);   
        float flattenHeight = Coord.WorldHeightToTerrainHeight(flattenToY, terrain);

        foreach (Vertex v in this.localHeightMap.localHeightmapVertices) {
            if (v.insideDeformerCollider || (!v.insideDeformerCollider && this.flattenArea)) {
                if (v.areaZ < this.terrainArea.terrainHeights.GetLength(0) || v.areaX < this.terrainArea.terrainHeights.GetLength(1)) {
                    this.terrainArea.terrainHeights[v.areaZ, v.areaX] = flattenHeight;
                }                
            }
        }
    }

    private Vertex TerrainSpacePosition() {             
        return new Vertex(this.deformerCollider.bounds.center, this.terrainArea.terrain);
    }

    private float EaseInQuad(float start, float end, float value) {
        end -= start;
        return end * value + start;
    }

    #endregion

}
