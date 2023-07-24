using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DynamicTerrainObjects {

    class SphereHeightmapStrategy : SphereStrategy {
        public SphereHeightmapStrategy(Collider deformerCollider, Area area, int padding = 0) : base(deformerCollider, area, padding) { }

        public Vertex[,] LocalHeightmap(LocalHeightMap.ColliderFitType heightFitType) {
            if (heightFitType == LocalHeightMap.ColliderFitType.ColumnFit) {
                return ColumnFitLocalHeightmap();
            } else {
                return VertexFitLocalHeightmap();
            }            
        }

        private Vertex[,] ColumnFitLocalHeightmap() {
            return LocalHeightMap.SetVertexNeighbors(
                VertexFitLocalHeightmap(), 
                this.terrainArea
            );
        }

        private Vertex[,] VertexFitLocalHeightmap() {
            Vertex[,] localHeightmapVertices = new Vertex[terrainArea.vertSizeZ, terrainArea.vertSizeX];

            // "flatten" 2d array with foreach 
            foreach (Vertex vertex in terrainArea.vertices) {
                localHeightmapVertices[vertex.areaZ, vertex.areaX] = SetHeightOffsetsForVertex(vertex);
            }          

            return localHeightmapVertices;
        }

        private Vertex SetHeightOffsetsForVertex(Vertex vertex) {
            Vertex v = Vertex.CloneVertex(vertex, this.terrain);

            // square root operations are expensive and multiplication is relatively cheap 
            // so use the squared mag and compare to a squared radius
            Vector3 vertexWorldPos = new Vector3(v.WorldPos().x, 0, v.WorldPos().z);
            
            float distSqrd2d = (this.Collider2dWorldPos - vertexWorldPos).sqrMagnitude;
            float colliderRadiusSquared = Mathf.Pow(colliderScaledRadius, 2);

            if (distSqrd2d < colliderRadiusSquared) {
                v.localHeightOffsetMax = TerrainHeightOffsetOfSphere(colliderRadiusSquared, vertexWorldPos);
                v.localHeightOffsetMin = -v.localHeightOffsetMax;
                v.insideDeformerCollider = true;
            } else {
                v.localHeightOffsetMax = 0f;
                v.localHeightOffsetMin = 0f;
            }

            return v;
        }

        private float TerrainHeightOffsetOfSphere(float colliderRadiusSquared, Vector3 vertexWorldPos) {
            return Coord.WorldHeightToTerrainHeight(
                Mathf.Sqrt(Mathf.Abs(
                    colliderRadiusSquared -
                    Mathf.Pow(vertexWorldPos.x - this.Collider2dWorldPos.x, 2) -
                    Mathf.Pow(vertexWorldPos.z - this.Collider2dWorldPos.z, 2)
                )),
                this.terrain
            );            
        }

    }
}