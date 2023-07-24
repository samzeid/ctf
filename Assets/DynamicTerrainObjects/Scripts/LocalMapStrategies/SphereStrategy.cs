using UnityEngine;

namespace DynamicTerrainObjects {

    public class SphereStrategy {

        protected Collider deformerCollider;
        protected Area terrainArea;
        protected Terrain terrain;
        protected int alphaPadding;
        protected Vector3 Collider2dWorldPos { get; set; }
        protected float colliderScaledRadius;

        public SphereStrategy(Collider deformerCollider, Area area, int padding = 0) {
            this.deformerCollider = deformerCollider;
            this.terrainArea = area;
            this.terrain = area.terrain;
            this.colliderScaledRadius = Mathf.Max(new float[] {
                deformerCollider.bounds.size.x,
                deformerCollider.bounds.size.y,
                deformerCollider.bounds.size.z
            }) / 2;
            this.alphaPadding = padding;
            this.Collider2dWorldPos = new Vector3(this.deformerCollider.transform.position.x, 0, this.deformerCollider.transform.position.z);
        }

    }

}
