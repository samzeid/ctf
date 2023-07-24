#region UsingStatements

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace DynamicTerrainObjects {

    public class Area {

        #region PublicVariables

        public Column[,] columns;
        public Vertex[,] vertices;
        public Column[,] alphaColumns;
        public float[,] terrainHeights;
        public float[,,] terrainAlphas;
        public Terrain terrain;
        public TerrainData terrainData;
        public int vertSizeX;
        public int vertSizeZ;
        public int alphaColumnSizeX;
        public int alphaColumnSizeZ;
        public int columnSizeX;
        public int columnSizeZ;
        public bool overlappingTerrainBounds = false;
        public bool outsideTerrainBounds = false;

        #endregion

        #region PrivateVariables

        public int vertPadding;
        public int alphaPadding;

        // geometry to base the size/shape of the Area on
        private Collider deformerCollider;

        // tracks the min and max coords of height columns in the area
        private Column colBase;
        private Column colMax;                

        // tracks the size and shape of the heightmap grid
        private Vertex vertBase;
        private Vertex vertMax;        

        // tracks the size and shape of the alphamap grid
        private Column alphaColumnBase;
        private Column alphaColumnMax;        
        
        #endregion

        #region Constructors

        /// <summary>
        ///     create a new Area based on the world position, size, and dimensions 
        ///     of a gameobject plus optional column padding
        /// </summary>
        /// <param name='_deformer'>
        /// 	GameObject for which the collider bounding box is used to define
        /// 	the size and shape of the Area and the way in which the Area will
        /// 	be modified (deformed)
        /// </param>
        /// <param name='ter'>
        /// 	terrain instance that this Area will describe a subset of
        /// </param>
        /// <param name='pad'>
        /// 	number of columns of terrain to add to the the x,z size of the Area
        /// 	A terrain column is the x,z center of a rectangle of 4 terrain vertices
        /// </param>
        public Area(GameObject deformer, Terrain ter, int pad = 1) {            
            SetProperties(deformer, ter, pad);
            SetAreaPositionFromGameObj();
        }

        #endregion

        #region PublicInstanceMethods

        public void SetAreaPositionFromGameObj() {

            Vector3 padding = new Vector3(this.vertPadding, 0, this.vertPadding);

            Vector3 paddedMin = new Vector3(
                this.deformerCollider.bounds.min.x, 
                this.deformerCollider.bounds.center.y, 
                this.deformerCollider.bounds.min.z
            ) - padding / 2;
            
            Vector3 paddedMax = new Vector3(
                this.deformerCollider.bounds.max.x, 
                this.deformerCollider.bounds.center.y, 
                this.deformerCollider.bounds.max.z
            ) + padding / 2;

            if (SetBoundryObjects(paddedMin, paddedMax)) {
                GetCurrentTerrainHeights();
                InitVertices();
                GetCurrentTerrainAlphamaps();
                InitAlphaColumns();
            }
        }

        public Vertex CenterVertex() {
            Vertex centerVert = this.vertices[vertSizeZ / 2, vertSizeX / 2];
            centerVert.isCenter = true;
            return centerVert;
        }

        public bool IsVertexInBounds(int z, int x) {
            return z >= 0 && x >= 0 && z < vertSizeZ - 1 && x < vertSizeX - 1;
        }

        public void SetHeights() {
            terrainData.SetHeights(vertBase.x, vertBase.z, this.terrainHeights);
        }

        public void SetHeightsDelayLOD() {
            terrainData.SetHeightsDelayLOD(vertBase.x, vertBase.z, this.terrainHeights);
        }

        public void SetAlphas() {
            terrainData.SetAlphamaps(this.alphaColumnBase.x, this.alphaColumnBase.z, this.terrainAlphas);
        }

        /// <summary>
        /// 	returns a 2d array of all columns within this Area
        /// </summary>
        public Column[,] Columns() {
            Column[,] cols = new Column[this.columnSizeZ, this.columnSizeX];

            for (int colZ = 0; colZ < this.columnSizeZ; colZ++) {
                for (int colX = 0; colX < this.columnSizeX; colX++) {
                    cols[colZ, colX] = new Column(
                        this.colBase.x + colX,
                        this.colBase.z + colZ,
                        this.terrain
                    );
                    cols[colZ, colX].areaX = colX;
                    cols[colZ, colX].areaZ = colZ;
                }
            }

            return cols;
        }

        #endregion

        #region PrivateMethods

        private void SetProperties(GameObject deformer, Terrain ter, int pad) {
            this.deformerCollider = deformer.GetComponent<Collider>();
            this.terrain = ter;
            this.terrainData = this.terrain.terrainData;
            this.vertPadding = Mathf.Max(pad, 1); // minimum padding is 1
        }

        /// <summary>
        /// 	Lookup the height data from the terrain object constrained to the 
        /// 	size and shape of this Area object
        /// </summary>
        private void GetCurrentTerrainHeights() {
            this.terrainHeights = new float[this.vertSizeZ, this.vertSizeX];
            this.terrainHeights = terrainData.GetHeights(
                this.vertBase.x, this.vertBase.z, this.vertSizeX, this.vertSizeZ
            );            
        }

        /// <summary>
        /// 	Lookup the height data from the terrain object constrained to the 
        /// 	size and shape of this Area object
        /// </summary>
        private void GetCurrentTerrainAlphamaps() {
            this.terrainAlphas = new float[this.alphaColumnSizeZ, this.alphaColumnSizeX, this.terrain.terrainData.splatPrototypes.Length];
            this.terrainAlphas = terrainData.GetAlphamaps(
                this.alphaColumnBase.x, this.alphaColumnBase.z, this.alphaColumnSizeX, this.alphaColumnSizeZ
            );
        }

        /// <summary>
        /// 	Initializes a 2d array of Vertex objects that mirrors the array returned by
        /// 	TerrainData.GetHeights(). Used to track changes to the terrain vertices and 
        /// 	do conversion to and from world space/terrain space
        /// </summary>
        private void InitVertices() {
            this.vertices = new Vertex[this.vertSizeZ, this.vertSizeX];
            Vector2 center = new Vector2(vertSizeX / 2, vertSizeZ / 2);
            for (int vertZ = 0; vertZ < this.vertSizeZ; vertZ++) {
                for (int vertX = 0; vertX < this.vertSizeX; vertX++) {
                    this.vertices[vertZ, vertX] = new Vertex(
                        this.vertBase.x + vertX, 
                        terrainHeights[vertZ, vertX],
                        this.vertBase.z + vertZ,
                        this.terrain
                    );
                    this.vertices[vertZ, vertX].areaX = vertX;
                    this.vertices[vertZ, vertX].areaZ = vertZ;

                    Vector2 current = new Vector2(vertX, vertZ);

                    if (current == center) {                    
                        this.vertices[vertZ, vertX].isCenter = true;
                    }

                    float distToMinY = Vector2.Distance(current, new Vector2(current.x, 0));
                    float distToMinX = Vector2.Distance(current, new Vector2(0, current.y));
                    float distToMaxY = Vector2.Distance(current, new Vector2(current.x, this.vertSizeZ - 1));
                    float distToMaxX = Vector2.Distance(current, new Vector2(this.vertSizeX - 1, current.y));

                    this.vertices[vertZ, vertX].distanceFromNearestBorder = Mathf.Min(new int[] {
                        (int) distToMinY, (int) distToMinX, (int) distToMaxY, (int) distToMaxX
                    });
                }
            }
        }

        /// <summary>
        /// 	Initializes a 2d array of Column objects that mirror the array returned by
        /// 	TerrainData.GetAlphamaps(). Used to track changes to the terrain splatmaps and 
        /// 	do conversion to and from world space/terrain space
        /// </summary>
        private void InitAlphaColumns() {
            this.alphaColumns = new Column[this.alphaColumnSizeZ, this.alphaColumnSizeX];

            for (int alphaZ = 0; alphaZ < this.alphaColumnSizeZ; alphaZ++) {
                for (int alphaX = 0; alphaX < this.alphaColumnSizeX; alphaX++) {
                    this.alphaColumns[alphaZ, alphaX] = new Column(
                        this.alphaColumnBase.x + alphaX,
                        this.alphaColumnBase.z + alphaZ,
                        this.terrain,
                        GridType.AlphaMap
                    );
                    this.alphaColumns[alphaZ, alphaX].areaX = alphaX;
                    this.alphaColumns[alphaZ, alphaX].areaZ = alphaZ;                    
                }
            }
        }

        /// <summary>
        /// 	Out of an List of Vertex objects, what is the shortest distance to a given origin Vertex?
        /// </summary>
        private float GetClosestModifiedVertexDistance(Vertex originVert, List<Vertex> modifiedVertices) {
            Vector3 originV3 = originVert.WorldPos();
            float candidateDistance;
            float currentShortestDistance = Mathf.Infinity;

            foreach (Vertex impactedVert in modifiedVertices) {
                candidateDistance = (originV3 - impactedVert.WorldPos()).sqrMagnitude;
                if (candidateDistance < currentShortestDistance) {
                    currentShortestDistance = candidateDistance;
                }
            }

            return Mathf.Sqrt(currentShortestDistance);
        }

        /// <summary>
        /// 	Set both Column, heightmap Vertex, and alphamap Vertex objects that  
        /// 	represent the "corners" (min/max) of this Area object
        /// </summary>
        private bool SetBoundryObjects(Vector3 worldMin, Vector3 worldMax) {

            // array with the 8 points that make up the corners of the overall terrain
            Vertex[] terrainBounds = TerrainHelpers.TerrainBounds(terrain);
            Vector3 terrainMin = terrainBounds[0].WorldPos();
            Vector3 terrainMax = terrainBounds[5].WorldPos();

            float heightmapWorldEdgeDist = TerrainHelpers.HeightmapEdgeLength(terrain);

            this.outsideTerrainBounds = IsOutsideTerrainBounds(terrainMin, terrainMax, worldMin, worldMax, heightmapWorldEdgeDist);

            if (this.outsideTerrainBounds) {
                this.overlappingTerrainBounds = false;
                return false;
            } 

            this.overlappingTerrainBounds = IsOverlappingTerrainBounds(terrainMin, terrainMax, worldMin, worldMax);                        

            // if the vertex Area coords would be outside of the terrain then use 0
            Vector3 terrainVertexMin = new Vector3(
                Mathf.Max(terrainBounds[0].WorldPos().x + heightmapWorldEdgeDist, worldMin.x),
                worldMin.y,
                Mathf.Max(terrainBounds[0].WorldPos().z + heightmapWorldEdgeDist, worldMin.z)
            );

            // if the Area coords would be outside the terrain then use the max size
            Vector3 terrainVertexMax = new Vector3(
                Mathf.Min(terrainBounds[5].WorldPos().x - heightmapWorldEdgeDist, worldMax.x),
                worldMax.y,
                Mathf.Min(terrainBounds[5].WorldPos().z - heightmapWorldEdgeDist, worldMax.z)
            );

            this.colBase = new Column(terrainVertexMin, terrain);
            this.colMax = new Column(terrainVertexMax, terrain);
            this.columnSizeZ = (colMax.z - colBase.z) + 1;
            this.columnSizeX = (colMax.x - colBase.x) + 1;

            this.vertBase = colBase.GridVertices()[0, 0];
            this.vertMax = colMax.GridVertices()[1, 1];
            this.vertSizeZ = (vertMax.z - vertBase.z);
            this.vertSizeX = (vertMax.x - vertBase.x);

            float alphamapWorldEdgeDist = TerrainHelpers.AlphamapEdgeLength(terrain) * 2;
            
            // if the vertex Area coords would be outside of the terrain then use 0
            Vector3 terrainAlphaMin = new Vector3(
                Mathf.Max(terrainBounds[0].WorldPos().x + alphamapWorldEdgeDist, worldMin.x),
                worldMin.y,
                Mathf.Max(terrainBounds[0].WorldPos().z + alphamapWorldEdgeDist, worldMin.z)
            );

            // if the Area coords would be outside the terrain then use the max size
            Vector3 terrainAlphaMax = new Vector3(
                Mathf.Min(terrainBounds[5].WorldPos().x - alphamapWorldEdgeDist, worldMax.x),
                worldMax.y,
                Mathf.Min(terrainBounds[5].WorldPos().z - alphamapWorldEdgeDist, worldMax.z)
            );

            this.alphaColumnBase = new Column(terrainAlphaMin, terrain, GridType.AlphaMap);
            this.alphaColumnMax = new Column(terrainAlphaMax, terrain, GridType.AlphaMap);
            this.alphaColumnSizeZ = (alphaColumnMax.z - alphaColumnBase.z) + 1;
            this.alphaColumnSizeX = (alphaColumnMax.x - alphaColumnBase.x) + 1;

            return true;
        }

        private bool IsOverlappingTerrainBounds(Vector3 terrainMin, Vector3 terrainMax, Vector3 worldMin, Vector3 worldMax) {
            return !this.outsideTerrainBounds && (
                       terrainMin.x > worldMin.x || 
                       terrainMin.z > worldMin.z || 
                       terrainMax.x < worldMax.x || 
                       terrainMax.z < worldMax.z
                   );
        }

        private bool IsOutsideTerrainBounds(Vector3 terrainMin, Vector3 terrainMax, Vector3 worldMin, Vector3 worldMax, float oneTerrainUnit) {            
            return worldMin.x > terrainMax.x - oneTerrainUnit ||
                   worldMin.z > terrainMax.z - oneTerrainUnit ||
                   worldMax.x < terrainMin.x + oneTerrainUnit ||
                   worldMax.z < terrainMin.z + oneTerrainUnit; 
        }

        #endregion

    }
}