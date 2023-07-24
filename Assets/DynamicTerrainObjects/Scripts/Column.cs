using UnityEngine;
using System;

namespace DynamicTerrainObjects {
    
    public class Column : Coord, IEquatable<Column> {

        public static Column CloneColumn(Column column, Terrain terrain) {
            Column c = new Column(column.x, column.z, terrain, column.Type);
            c.y = column.y;
            c.areaX = column.areaX;
            c.areaZ = column.areaZ;
            return c;
        }

        // get a column from a world position x, z
        public Column(float wx, float wy, float wz, Terrain ter, GridType type = GridType.HeightMap) {
            SetProperties(ter, type); // defined in Coord.cs
            WorldPosToTerrainCoords(wx, wy, wz);
        }

        // get a column from a world position Vector3
        public Column(Vector3 worldPos, Terrain ter, GridType type = GridType.HeightMap) {
            SetProperties(ter, type); // defined in Coord.cs
            WorldPosToTerrainCoords(worldPos.x, worldPos.y, worldPos.z);
        }

        // get a column from a terrain coordinate
        public Column(int tx, int tz, Terrain ter, GridType type = GridType.HeightMap) {
            SetProperties(ter, type); // defined in Coord.cs

            // the float x and z are usually used for storing the non grid aligned
            // translation of a world position. In this case we are starting from
            // a grid aligned point so both the float x,z and int x,z will be identical
            xf = (float) tx;
            zf = (float) tz;
            x = tx;
            z = tz; 
        }

        // take the original position within the column, floor it to the bottom left
        // convert that to world position and then add an offset of half the width of
        // a terrain column to get a world position centered on the column
        // if the height is set then convert it to world space, otherwise find an approximation of 
        // it directly from the terrain data

        // TODO: should height for a column be an average of all of its vertices??
        public Vector3 WorldPos() {
            
            // converting to world position can be expensive... return cached result if found
            if (cache_worldPos != null) { return (Vector3) cache_worldPos; }

            if (y == null) {
                SetWorldHeightToTerrainHeight(terrainData.GetHeight(Mathf.FloorToInt(xf), Mathf.FloorToInt(zf)));
            }

            Vertex center = new Vertex(x, (float) y, z, terrain, Type);      
            Vector3 offset = new Vector3(
                0.5f * ((float)terrainData.size.x / this.mappingResolutionX),
                0,
                0.5f * ((float)terrainData.size.z / this.mappingResolutionZ)
            );
            cache_worldPos = center.WorldPos() + offset;

            return (Vector3) cache_worldPos;
        }

        
        private Vertex[,] gridVertices;

        // returns the four terrain grid vertices adjacent to the given terrain space translated coordinate
        // given . are world positions, o are terrain vertices 
        // given x is the desired world position, return a,b,c,d terrain vertices
        //
        // o . o . o . o
        // . . . . . . .
        // o . b . c . o
        // . . . x . . .
        // o . a . d . o
        // . . . . . . .
        //
        public Vertex[,] GridVertices(bool preloadHeights = false) {
            if (gridVertices != null) {
                return gridVertices;
            }

            gridVertices = new Vertex[2, 2];            

            // front left corner
            gridVertices[0, 0] = new Vertex(
                Mathf.FloorToInt(xf),
                0f,
                Mathf.FloorToInt(zf), 
                terrain
            );
            gridVertices[0, 0].areaX = areaX;
            gridVertices[0, 0].areaZ = areaZ;

            // front right corner
            gridVertices[0, 1] = new Vertex(
                gridVertices[0, 0].x + 1,
                0f,
                gridVertices[0, 0].z, 
                terrain
            );
            gridVertices[0, 1].areaX = areaX + 1;
            gridVertices[0, 1].areaZ = areaZ;

            // back left corner
            gridVertices[1, 0] = new Vertex(
                gridVertices[0, 0].x,
                0f,
                gridVertices[0, 0].z + 1, 
                terrain
            );
            gridVertices[1, 0].areaX = areaX;
            gridVertices[1, 0].areaZ = areaZ + 1;

            // back right corner
            gridVertices[1, 1] = new Vertex(
                gridVertices[0, 0].x + 1,
                0f,
                gridVertices[0, 0].z + 1, 
                terrain
            );
            gridVertices[1, 1].areaX = areaX + 1;
            gridVertices[1, 1].areaZ = areaZ + 1;

            if (preloadHeights) {
                float[,] columnHeights = terrainData.GetHeights(gridVertices[0,0].x, gridVertices[0, 0].z, 2, 2);
                gridVertices[0, 0].y = columnHeights[0, 0];
                gridVertices[0, 1].y = columnHeights[0, 1];
                gridVertices[1, 0].y = columnHeights[1, 0];
                gridVertices[1, 1].y = columnHeights[1, 1];
            }

            return gridVertices;
        }

        // implement the IEquatible interface so this object can be compared 
        // with other instances
        public bool Equals(Column other) {
            return areaX == other.areaX && areaZ == other.areaZ;
        }

        // override object.Equals and have it call the IEquatible interface
        public override bool Equals(object obj) {
            Column col = obj as Column;
            if (col != null) {
                return Equals(col);
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        protected override void WorldPosToTerrainCoords(float wx, float wy, float wz) {
            base.WorldPosToTerrainCoords(wx, wy, wz);
            x = Mathf.FloorToInt(xf);
            z = Mathf.FloorToInt(zf);
        }
    }
}
