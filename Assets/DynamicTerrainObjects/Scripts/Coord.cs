using UnityEngine;

namespace DynamicTerrainObjects {

    public enum GridType {
        HeightMap,
        AlphaMap
    }

    // represents a world position that has been translated to a terrain position
    // parent class for Vertex and Column     
    public class Coord : System.Object {

        public bool insideDeformerCollider = false;

        public float localHeightOffsetMax;
        public float localHeightOffsetMin;

        public bool isCenter = false;
        public int distanceFromNearestBorder;

        // non-quantized terrain space value of a world position, 
        public float xf; 
        public float zf;

        // terrain height is always a float and always stored in terrain space
        // allow to be null... must be cast to float when used
        public float? y;

        // quantized (aligned to the int grid) terrain point
        public int x;
        public int z;

        // stores the indexes local to a terrain Area object
        public int areaX;
        public int areaZ;

        protected Terrain terrain;
        protected TerrainData terrainData;

        protected int mappingResolutionX;
        protected int mappingResolutionZ;

        protected Vector3? cache_worldPos;

        public GridType Type { get; set; }

        public void SetWorldHeightToTerrainHeight(float wy) {
            this.y = WorldHeightToTerrainHeight(wy);
        }

        public string ToDictKey() {
            return string.Format("{0}-{1}", x, z);
        }

        public override string ToString() {
            return string.Format("Z:{0}X:{1} insidecol: {2} offsetmax: {3} offsetmin: {4}", areaZ, areaX, insideDeformerCollider, localHeightOffsetMax, localHeightOffsetMin);
            //return string.Format("Terrain Quantized Coordinates: {0} {1} {2}", x, y, z) + "\r\n" +
            //       string.Format("Terrain Exact Coordinates: {0} {1} {2}", xf, y, zf);
        }

        // world to terrain
        protected float WorldXToTerrainX(float wx) {
            return (wx - terrain.transform.position.x) / (terrain.terrainData.size.x / this.mappingResolutionX);
        }

        protected float WorldZToTerrainZ(float wz) {
            return (wz - terrain.transform.position.z) / (terrain.terrainData.size.z / this.mappingResolutionZ);
        }
        
        // alias for the static version of this method below to conveniently use internally or externally
        protected float WorldHeightToTerrainHeight(float wy) {
            return WorldHeightToTerrainHeight(wy, this.terrain);
        }
        
        public static float WorldHeightToTerrainHeight(float wy, Terrain terrain) {
            return Mathf.Clamp((wy - terrain.transform.position.y) / (terrain.terrainData.size.y / 1), 0.0f, 1.0f);
        }

        protected virtual void WorldPosToTerrainCoords(float wx, float wy, float wz) {
            this.xf = WorldXToTerrainX(wx);
            this.y = WorldHeightToTerrainHeight(wy);
            this.zf = WorldZToTerrainZ(wz);
        }

        // terrain to world
        protected float TerrainXToWorldX(float tx) {
            return tx * ((float)terrainData.size.x / this.mappingResolutionX) + terrain.transform.position.x;
        }

        protected float TerrainZToWorldZ(float tz) {
            return tz * ((float)terrainData.size.z / this.mappingResolutionZ) + terrain.transform.position.z;
        }

        protected float TerrainHeightToWorldHeight(float ty) {
            return ((float) ty * terrainData.size.y) - terrain.transform.position.y;
        }

        protected void SetProperties(Terrain ter, GridType type = GridType.HeightMap) {
            this.terrain = ter;
            this.terrainData = ter.terrainData;
            this.Type = type;

            if (type == GridType.HeightMap) {
                this.mappingResolutionX = terrainData.heightmapResolution - 1;
                this.mappingResolutionZ = terrainData.heightmapResolution - 1;
            }
            else {
                this.mappingResolutionX = terrainData.alphamapWidth;
                this.mappingResolutionZ = terrainData.alphamapHeight;
            }
        }
    }
}