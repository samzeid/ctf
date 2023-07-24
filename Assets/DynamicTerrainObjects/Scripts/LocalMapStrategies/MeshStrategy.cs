using UnityEngine;

namespace DynamicTerrainObjects {

    public class MeshStrategy {

        protected Collider deformerCollider;
        protected Area terrainArea;
        protected Terrain terrain;
        protected RaycastHit hit;
        protected float colliderTop;
        protected float colliderBottom;
        protected int alphaPadding;
        protected float colliderCenterTerrainHeight;
        protected Vector3 boundsMin;
        protected Vector3 boundsMax;

        public MeshStrategy(Collider deformerCollider, Area terrainArea, int padding = 0) {
            this.deformerCollider = deformerCollider;
            this.boundsMin = this.deformerCollider.bounds.min;
            this.boundsMax = this.deformerCollider.bounds.max;
            this.terrainArea = terrainArea;
            this.terrain = terrainArea.terrain;
            this.colliderTop = deformerCollider.bounds.max.y + 0.1f;
            this.colliderBottom = deformerCollider.bounds.min.y - 0.1f;
            this.alphaPadding = padding;
            this.colliderCenterTerrainHeight = Coord.WorldHeightToTerrainHeight(
                deformerCollider.bounds.center.y, this.terrain
            );
        }

        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
        // LocalHeightmap() and LocalAlphamap() both implemented in their respective classes:
        // MeshHeightmapStrategy and MeshAlphamapStrategy that both inherit from this class
        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 

        protected float ScanHeight(Vertex vertex, float rayOriginHeight, Vector3 rayDirection, bool debug = false) {
            Vector3 origin = vertex.WorldPos();
            origin.y = rayOriginHeight;
            float offset = 0f;
            Ray ray = new Ray(origin, rayDirection);


            if (VertexWithinDeformerBoundsXZ(vertex)) {

                if (debug) {
                    Vector3 mid = new Vector3(origin.x, deformerCollider.bounds.center.y, origin.z);
                    DebugHelpers.CreateDebugSphere(mid, Color.blue);

                    DebugHelpers.CreateDebugSphere(origin, Color.black);
                    Debug.DrawRay(origin, rayDirection * 2, Color.red, 100f);
                }

                if (deformerCollider.Raycast(ray, out hit, Mathf.Pow(deformerCollider.bounds.size.y, 2f))) {
                    float colliderHeight = Coord.WorldHeightToTerrainHeight(hit.point.y, this.terrain);

                    // the y offset for world x/z position from the center of the collider to the top or bottom
                    // will be positive or negative depending on the direction of the scan (from bottom up or top down)                
                    offset = colliderHeight - colliderCenterTerrainHeight;

                    vertex.insideDeformerCollider = true;

                    if (debug) {
                        origin.y = deformerCollider.bounds.center.y + new Vertex(0, offset, 0, this.terrain).WorldPos().y;
                        DebugHelpers.CreateDebugSphere(origin, Color.red);
                    }
                    return offset;
                }
                else {
                    return 0f;
                }
            }

            return offset;
        }

        private bool VertexWithinDeformerBoundsXZ(Vertex vertex) {
            Vector3 vertexWorld = vertex.WorldPos();

            return vertexWorld.x > boundsMin.x && vertexWorld.x < boundsMax.x &&
                   vertexWorld.z > boundsMin.z && vertexWorld.z < boundsMax.z;
        }
    }
}