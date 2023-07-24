using UnityEngine;
using System;

namespace DynamicTerrainObjects {

    public class Vertex : Coord, IEquatable<Vertex> {

        #region Constructors

        // vertex from terrain space 
        public Vertex(int tx, float ty, int tz, Terrain ter, GridType type = GridType.HeightMap) {
            SetProperties(ter, type); // defined in Coord.cs
            x = tx; y = ty; z = tz;            
        }

        // vertex from world space vector3
        public Vertex(Vector3 wV3, Terrain ter, GridType type = GridType.HeightMap) {
            SetProperties(ter, type); // defined in Coord.cs
            WorldPosToTerrainCoords(wV3.x, wV3.y, wV3.z);
        }

        #endregion

        #region PublicMethods

        public static Vertex CloneVertex(Vertex vertex, Terrain terrain) {
            //return vertex;
            Vertex v = new Vertex(vertex.x, (float)vertex.y, vertex.z, terrain);
            v.areaX = vertex.areaX;
            v.areaZ = vertex.areaZ;
            v.insideDeformerCollider = vertex.insideDeformerCollider;
            return v;
        }

        // return Vector3 in world space snapped to terrain grid point
        public Vector3 WorldPos() {
            return new Vector3(
                TerrainXToWorldX(x),
                TerrainHeightToWorldHeight((float) y),
                TerrainZToWorldZ(z)
            );
        }

        // return all columns this vertex could be a part of
        // does not check if columns are out of the bounds of the terrain
        public Column[,] Columns() {
            Column[,] columns = new Column[2, 2];

            columns[0, 0] = new Column(x - 1, z - 1, this.terrain); // bottom left
            columns[0, 1] = new Column(x - 1, z, this.terrain);     // bottom right
            columns[1, 1] = new Column(x, z, this.terrain);         // top right
            columns[1, 0] = new Column(x, z - 1, this.terrain);     // top left

            return columns;
        }

        // implement the IEquatible interface so this object can be compared 
        // with other instances
        public bool Equals(Vertex other) {
            return this.areaX == other.areaX && this.areaZ == other.areaZ;
        }

        // override object.Equals and have it call the IEquatible interface
        public override bool Equals(object obj) {
            Vertex col = obj as Vertex;
            if (col != null) {
                return Equals(col);
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        #endregion

        #region ProtectedMethods

        protected override void WorldPosToTerrainCoords(float wx, float wy, float wz) {
            base.WorldPosToTerrainCoords(wx, wy, wz);
            this.x = Mathf.RoundToInt(xf);
            this.z = Mathf.RoundToInt(zf);
        }

        #endregion
    }
}
