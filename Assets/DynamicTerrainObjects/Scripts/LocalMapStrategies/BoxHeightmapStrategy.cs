using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTerrainObjects {

    class BoxHeightmapStrategy : BoxStrategy {

        public BoxHeightmapStrategy(
            Collider deformerCollider, 
            Area terrainArea, 
            int alphaPadding = 0
        ) : base(deformerCollider, terrainArea, alphaPadding) {}

        public Vertex[,] LocalHeightmap(LocalHeightMap.ColliderFitType heightFitType = LocalHeightMap.ColliderFitType.VertexFit) {
            if (heightFitType == LocalHeightMap.ColliderFitType.ColumnFit) {
                return ColumnFitLocalHeightmap();
            } else {           
                return VertexFitLocalHeightmap();
            }            
        }

        private Vertex[,] ColumnFitLocalHeightmap() {
            return LocalHeightMap.SetVertexNeighbors(VertexFitLocalHeightmap(), terrainArea, false);
        }

        private Vertex[,] VertexFitLocalHeightmap() {
            Vertex[,] localHeightmapVertices = new Vertex[terrainArea.vertSizeZ, terrainArea.vertSizeX];

            foreach (Vertex vertex in this.terrainArea.vertices) {
                Vertex v = Vertex.CloneVertex(vertex, terrain);

                if (PointInOABB(v.WorldPos(), (BoxCollider) this.deformerCollider, this.alphaPadding)) {                    
                    v.localHeightOffsetMax = Coord.WorldHeightToTerrainHeight(deformerCollider.bounds.extents.y, terrain);
                    v.localHeightOffsetMin = -v.localHeightOffsetMax;
                    v.insideDeformerCollider = true;
                }
                else {
                    v.localHeightOffsetMax = 0f;
                    v.localHeightOffsetMin = 0f;
                }

                localHeightmapVertices[v.areaZ, v.areaX] = v;
            }
            return localHeightmapVertices;
        }

        public Column[] LocalAlphamap(LocalHeightMap.ColliderFitType alphaFitType) {                                    
            Column[] localAlphamapColumns = new Column[terrainArea.alphaColumns.Length];
            int i = 0;
            Column c;
            foreach (Column column in terrainArea.alphaColumns) {
                c = Column.CloneColumn(column, terrain);

                if (alphaFitType == LocalHeightMap.ColliderFitType.ColumnFit) {
                    c.insideDeformerCollider = ColumnHasInboundsVertices(c);
                }
                else if (PointInOABB(c.WorldPos(), (BoxCollider)deformerCollider, this.alphaPadding)) {
                    c.insideDeformerCollider = true;
                }

                localAlphamapColumns[i] = c;
                i++;
            }

            return localAlphamapColumns;
        }     

    }

}