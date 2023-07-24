using UnityEngine;

namespace DynamicTerrainObjects {

    // if a mesh collider is given, use raycasting to scan upwards and 
    // downwards from each local terrain vertex and find its upward and dowwnward
    // height offsets - this is obviously the most resource intensive method, but provides 
    // the most exact terrain game object based deformation and is only done once on Start()
    // and is cached in the Deformer class from there on.
    class MeshHeightmapStrategy : MeshStrategy {


        public MeshHeightmapStrategy(Collider deformerCollider, Area terrainArea, int padding = 0) : base(deformerCollider, terrainArea, padding) {}

        /// <summary>
        /// 	Raycast from each terrain Vertex that falls within the deformer's collider
        /// 	bounding box so we can handle arbitrary collider shapes
        /// </summary>
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
                terrainArea
            );            
        }

        private Vertex[,] VertexFitLocalHeightmap() {
            Vertex[,] localHeightmapVertices = new Vertex[terrainArea.vertSizeZ, terrainArea.vertSizeX];
            for (int z = 0; z < terrainArea.vertices.GetLength(0); z++) {
                for (int x = 0; x < terrainArea.vertices.GetLength(1); x++) {
                    Vertex v = Vertex.CloneVertex(terrainArea.vertices[z, x], terrain);                    
                    v.localHeightOffsetMax = ScanHeight(v, this.colliderTop, Vector3.down);
                    v.localHeightOffsetMin = ScanHeight(v, this.colliderBottom, Vector3.up);
                    localHeightmapVertices[z, x] = v;
                }
            }                            

            return localHeightmapVertices;
        }
    }
}