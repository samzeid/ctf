using UnityEngine;

namespace DynamicTerrainObjects {

    public class BoxStrategy {

        protected Collider deformerCollider;
        protected Area terrainArea;
        protected Terrain terrain;
        protected int alphaPadding;

        public BoxStrategy(Collider deformerCollider, Area terrainArea, int alphaPadding = 0) {
            this.deformerCollider = deformerCollider;
            this.terrainArea = terrainArea;
            this.terrain = terrainArea.terrain;
            this.alphaPadding = alphaPadding;
        }

        protected bool PointInOABB(Vector3 point, BoxCollider box, int padding = 0) {
            point = box.transform.InverseTransformPoint(point) - box.center;

            // 0.5f would be exact, but given how coarse terrain resolutions 
            // usually are, adding a 0.1f padding yields better fitting results
            float halfX = (box.size.x * 0.5f) + padding;
            float halfY = (box.size.y * 0.5f);
            float halfZ = (box.size.z * 0.5f) + padding;

            if (point.x < halfX && point.x > -halfX &&
               point.y < halfY && point.y > -halfY &&
               point.z < halfZ && point.z > -halfZ)
                return true;
            else
                return false;
        }

        // if any of the columns vertices are within the collider then mark 
        // the column as "inside" the collider
        // (ie: at least one of it's vertices is inside the collider)
        protected bool ColumnHasInboundsVertices(Column c) {
            bool result = false;
            foreach (Vertex vertex in c.GridVertices(true)) {
                if (PointInOABB(vertex.WorldPos(), (BoxCollider) this.deformerCollider)) {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}