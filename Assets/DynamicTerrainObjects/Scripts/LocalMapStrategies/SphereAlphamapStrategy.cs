using UnityEngine;

namespace DynamicTerrainObjects {

    public class SphereAlphamapStrategy : SphereStrategy {

        public SphereAlphamapStrategy(Collider deformerCollider, Area area, int padding = 0) : base(deformerCollider, area, padding) { }

        public Column[,] LocalAlphamap(LocalAlphaMap.ColliderFitType alphaFitType) {
            Column[,] localAlphamapColumns = new Column[terrainArea.alphaColumns.GetLength(0), terrainArea.alphaColumns.GetLength(1)];

            for (int alphaZ = 0; alphaZ < terrainArea.alphaColumns.GetLength(0); alphaZ++) {
                for (int alphaX = 0; alphaX < terrainArea.alphaColumns.GetLength(1); alphaX++) {
                    Column c = Column.CloneColumn(terrainArea.alphaColumns[alphaZ, alphaX], terrain);

                    if (alphaFitType == LocalAlphaMap.ColliderFitType.ColumnFit) {
                        c.insideDeformerCollider = ColumnHasInboundsVertices(c);
                    } else {
                        c.insideDeformerCollider = Vector3IsInbounds(c.WorldPos());                     
                    }                    

                    localAlphamapColumns[alphaZ, alphaX] = c;
                }
            }

            return localAlphamapColumns;
        }

        // if any of the columns vertices are within the collider then mark 
        // the column as "inside" the collider
        // (ie: at least one of it's vertices is inside the collider)
        protected bool ColumnHasInboundsVertices(Column c) {
            bool result = false;
            foreach (Vertex vertex in c.GridVertices()) {
                Vector3 vertexWorldPos = new Vector3(vertex.WorldPos().x, 0, vertex.WorldPos().z);
                if (Vector3IsInbounds(vertexWorldPos)) {
                    result = true;
                    break;
                }
            }
            return result;
        }

        protected bool Vector3IsInbounds(Vector3 vertexWorldPos) {
            vertexWorldPos.y = 0;
            float distSqrd2d = (this.Collider2dWorldPos - vertexWorldPos).sqrMagnitude;
            float colliderRadiusSquared = Mathf.Pow(colliderScaledRadius, 2);
            
            if (distSqrd2d < colliderRadiusSquared + ((this.alphaPadding * 2))) {
                return true;
            } else {
                return false;
            }
        }
    }
}