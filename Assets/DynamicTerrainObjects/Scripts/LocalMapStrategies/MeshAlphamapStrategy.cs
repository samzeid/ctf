using UnityEngine;

namespace DynamicTerrainObjects {

    public class MeshAlphamapStrategy : MeshStrategy {

        public MeshAlphamapStrategy(Collider deformerCollider, Area terrainArea, int padding = 0) : base(deformerCollider, terrainArea, padding) {}

        public Column[,] LocalAlphamap(LocalAlphaMap.ColliderFitType alphaFitType) {
            // temporarily scale the deformer object's x and z axis to simulate "padding"
            // when we do the local hit scan
            Vector3 alphaScaling = new Vector3(1, 0, 1) * (this.alphaPadding * 0.1f);
            Transform deformerTransform = this.deformerCollider.gameObject.transform;
            deformerTransform.localScale += alphaScaling;

            Column[,] localAlphamapColumns = new Column[terrainArea.alphaColumns.GetLength(0), terrainArea.alphaColumns.GetLength(1)];

            for (int alphaZ = 0; alphaZ < terrainArea.alphaColumns.GetLength(0); alphaZ++) {
                for (int alphaX = 0; alphaX < terrainArea.alphaColumns.GetLength(1); alphaX++) {
                    Column c = Column.CloneColumn(terrainArea.alphaColumns[alphaZ, alphaX], terrain);

                    foreach (Vertex v in c.GridVertices()) {
                        if (Mathf.Abs(ScanHeight(v, this.colliderTop, Vector3.down)) != 0f) {
                            c.insideDeformerCollider = true;
                        }
                    }
                    localAlphamapColumns[alphaZ, alphaX] = c;
                }
            }

            // return deformer to its previous state
            deformerTransform.localScale -= alphaScaling;

            return localAlphamapColumns;
        }       
    }
}
