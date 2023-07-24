using UnityEngine;
using System.Collections;
namespace DynamicTerrainObjects {
    public class LocalAlphaMap {

        #region Properties

        public enum ColliderFitType {
            VertexFit,
            ColumnFit
        }

        /// <summary>
        /// Array of columns mapped to the alpha grid for a given Area.
        /// Contains the alpha data as well as whether a given column is within
        /// the deformer's collider. Used for updating textures for a deformer's footprint
        /// or other columns affected by the collider (such as during a terrain displacement)
        /// </summary>
        public Column[,] localAlphamapColumns;

        private GameObject deformerCollider;
        private Area terrainArea;

        #endregion

        #region Constructors

        /// <summary>
        ///     Given a GameObject and its collider of unknown type (sphere, box, or mesh)
        ///     create a mapping of the objects terrain alphas and where the object's collider
        ///     intersects with alpha columns
        /// </summary>
        /// <param name='deformerCollider'>
        /// 	GameObject for which we will introspect the collider and create local mappings
        /// </param>  
        /// <param name='terrainArea'>
        /// 	terrain Area instance we will use to find the size and resolution of our local grid        	
        /// </param>   
        public LocalAlphaMap(Collider deformerCollider, Area terrainArea, ColliderFitType alphaFitType, int alphaPadding = 0) {
            if (deformerCollider.GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
                BoxAlphamapStrategy boxAlphaStrategy = new BoxAlphamapStrategy(deformerCollider, terrainArea, alphaPadding);
                this.localAlphamapColumns = boxAlphaStrategy.LocalAlphamap(alphaFitType);
            }
            else
            if (deformerCollider.GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
                SphereAlphamapStrategy sphereAlphamapStrategy = new SphereAlphamapStrategy(deformerCollider, terrainArea, alphaPadding);
                this.localAlphamapColumns = sphereAlphamapStrategy.LocalAlphamap(alphaFitType);
            }
            else
            if (deformerCollider.GetComponent<Collider>().GetType() == typeof(MeshCollider)) {
                MeshAlphamapStrategy meshAlphaStrategy = new MeshAlphamapStrategy(deformerCollider, terrainArea, alphaPadding);
                this.localAlphamapColumns = meshAlphaStrategy.LocalAlphamap(alphaFitType);
            }
        }

        #endregion
    }
}