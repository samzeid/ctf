using UnityEngine;

namespace DynamicTerrainObjects {

    public class BoxAlphamapStrategy : BoxStrategy {
        public BoxAlphamapStrategy(
            Collider deformerCollider,
            Area terrainArea,
            int alphaPadding = 0
        ) : base(deformerCollider, terrainArea, alphaPadding) { }

        public Column[,] LocalAlphamap(LocalAlphaMap.ColliderFitType alphaFitType) {
            Column[,] localAlphamapColumns = new Column[terrainArea.alphaColumns.GetLength(0), terrainArea.alphaColumns.GetLength(1)];
            Column c;

            int zLength = terrainArea.alphaColumns.GetLength(0);
            int xLength = terrainArea.alphaColumns.GetLength(1);

            // init min and max to dead center in the 2d array
            int[] mapMin = new int[] { zLength / 2, xLength / 2 };
            int[] mapMax = new int[] { zLength / 2, xLength / 2 };

            for (int alphaZ = 0; alphaZ < zLength; alphaZ++) {
                for (int alphaX = 0; alphaX < xLength; alphaX++) {

                    // clone the column object
                    c = Column.CloneColumn(terrainArea.alphaColumns[alphaZ, alphaX], terrain);

                    // check to see if the column is inside the collider based on which
                    // fit type is given
                    if (alphaFitType == LocalAlphaMap.ColliderFitType.ColumnFit) {
                        c.insideDeformerCollider = ColumnHasInboundsVertices(c);
                    }
                    else if (PointInOABB(c.WorldPos(), (BoxCollider)deformerCollider)) {
                        c.insideDeformerCollider = true;
                    }

                    // track the min and max of the columns found to be inside the collider
                    // so that padding can be applied next
                    if (this.alphaPadding > 0 && c.insideDeformerCollider) {
                        if (alphaZ * alphaX < mapMin[0] * mapMin[1]) {
                            mapMin[0] = alphaZ; mapMin[1] = alphaX;
                        }
                        if (alphaZ * alphaX > mapMax[0] * mapMax[1]) {
                            mapMax[0] = alphaZ; mapMax[1] = alphaX;
                        }
                    }

                    localAlphamapColumns[alphaZ, alphaX] = c;
                }
            }

            // apply alpha padding if specified
            if (this.alphaPadding > 0) {
                int zMin = Mathf.Clamp(mapMin[0] - this.alphaPadding, 0, zLength - 1);
                int xMin = Mathf.Clamp(mapMin[1] - this.alphaPadding, 0, xLength - 1);

                int zMax = Mathf.Clamp(mapMax[0] + this.alphaPadding, 0, zLength - 1);                
                int xMax = Mathf.Clamp(mapMax[1] + this.alphaPadding, 0, xLength - 1);

                for (int alphaZ = zMin; alphaZ <= zMax; alphaZ++) {
                    for (int alphaX = xMin; alphaX <= xMax; alphaX++) {
                        localAlphamapColumns[alphaZ, alphaX].insideDeformerCollider = true;
                    }
                }
            }

            return localAlphamapColumns;
        }

    }
}