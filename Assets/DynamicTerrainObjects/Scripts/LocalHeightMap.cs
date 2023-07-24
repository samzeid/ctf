using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTerrainObjects {
    public class LocalHeightMap {

        #region Properties

        public enum ColliderFitType {
            VertexFit,
            ColumnFit
        }

        /// <summary>
        /// Array of all vertices mapped to the terrain heightmap grid
        /// for a given Area. Vertex will contain the local terrain aligned 
        /// mapping coordinates and the height values (if any) for the top 
        /// and bottom for a given collider (deformer)
        /// </summary>
        public Vertex[,] localHeightmapVertices;

        private GameObject deformerCollider;
        private Area terrainArea;

        #endregion

        #region Constructors

        /// <summary>
        ///     Given a GameObject and its collider of unknown type (sphere, box, or mesh)
        ///     create a mapping of the objects terrain height offsets (ie: how high and low 
        ///     does this object extend in terrain height units for a given terrain coordinate)
        ///     Do the same thing for the alpha mapping
        /// </summary>
        /// <param name='_deformer'>
        /// 	GameObject for which we will introspect the collider and create local mappings
        /// </param>  
        /// <param name='terrainArea'>
        /// 	terrain Area instance we will use to find the size and resolution of our local grid        	
        /// </param>   
        public LocalHeightMap(Collider deformerCollider, Area terrainArea, ColliderFitType heightFitType) {

            if (deformerCollider.GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
                BoxHeightmapStrategy boxHeightStrategy = new BoxHeightmapStrategy(deformerCollider, terrainArea);
                this.localHeightmapVertices = boxHeightStrategy.LocalHeightmap(heightFitType);
            } else
            if (deformerCollider.GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
                SphereHeightmapStrategy sphereHeightmapStrategy = new SphereHeightmapStrategy(deformerCollider, terrainArea);
                this.localHeightmapVertices = sphereHeightmapStrategy.LocalHeightmap(heightFitType);
            } else
            if (deformerCollider.GetComponent<Collider>().GetType() == typeof(MeshCollider)) {
                MeshHeightmapStrategy meshHeightStrategy = new MeshHeightmapStrategy(deformerCollider, terrainArea);
                this.localHeightmapVertices = meshHeightStrategy.LocalHeightmap(heightFitType);
            }                        
        }

        // loop through the verts a second time, this time setting the heights 
        // for all the direct neighbors of verts that were previously given 
        // a height offset - this will make it where any column that had at least 
        // one of its verts set via Vertex fit will now have all of its verts that
        // don't already have an offset gain the average offset of all neighbors
        public static Vertex[,] SetVertexNeighbors(Vertex[,] localHeightMap, Area terrainArea, bool averageNeighborHeight = true) {
            List<Vertex> insideVerts = new List<Vertex>();
            foreach (Vertex vertex in localHeightMap) {
                if (vertex.insideDeformerCollider) {
                    insideVerts.Add(vertex);
                }
            }
            float max;
            float min;

            foreach (Vertex vertex in insideVerts) {
                float sumMax = 0f;
                float sumMin = 0f;

                if (averageNeighborHeight) {
                    for (int nz = -1; nz < 2; nz++) {
                        for (int nx = -1; nx < 2; nx++) {
                            sumMin += localHeightMap[vertex.areaZ + nz, vertex.areaX + nx].localHeightOffsetMin;
                            sumMax += localHeightMap[vertex.areaZ + nz, vertex.areaX + nx].localHeightOffsetMax;
                        }
                    }
                }

                for (int nz = -1; nz < 2; nz++) {
                    for (int nx = -1; nx < 2; nx++) {
                        if (!localHeightMap[vertex.areaZ + nz, vertex.areaX + nx].insideDeformerCollider) {

                            if (averageNeighborHeight) {
                                max = sumMax / 9;
                                min = sumMin / 9;
                            } else {
                                max = localHeightMap[vertex.areaZ, vertex.areaX].localHeightOffsetMax;
                                min = localHeightMap[vertex.areaZ, vertex.areaX].localHeightOffsetMax;
                            }

                            localHeightMap[vertex.areaZ + nz, vertex.areaX + nx].insideDeformerCollider = true;
                            localHeightMap[vertex.areaZ + nz, vertex.areaX + nx].localHeightOffsetMax = max;
                            localHeightMap[vertex.areaZ + nz, vertex.areaX + nx].localHeightOffsetMin = min;
                        }
                    }
                }
            }
          
            return localHeightMap;
        }

        #endregion

    }
}