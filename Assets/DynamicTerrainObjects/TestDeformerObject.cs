using UnityEngine;

public class TestDeformerObject : MonoBehaviour {

    // How fast should the object move around?
    public float speed = 10f;

    // How far should the object move around the terrain?
    public float moveRadius = 50f;


    // how often should the object make a modification to the terrain?
    public float terrainUpdateFrequency = 0.5F;

    // how often should the object change directions?
    public float updateDestinationFrequency = 2f;

    // declare a variable to reference the Deformer instance on this object
    private Deformer deformer;

    // Where was this object positioned when it was first initialized?
    private Vector3 startPosition;

    // Where should the object move towards?
    private Vector3 targetPosition;

    // Use this for initialization
    void Start() {
        // set the reference to the Deformer instance
        this.deformer = GetComponent<Deformer>();

        // set the start position
        this.startPosition = transform.position;

        // Every terrainUpdateFrequency number of seconds, randomly call one of the terrain Deformation functions
        InvokeRepeating("RandomTerrainOperation", 2, this.terrainUpdateFrequency);

        // Every updateDestinationFrequency number of seconds, set a new random position on the terrain to move towards
        InvokeRepeating("SetRandomDestination", 0, this.updateDestinationFrequency);
    }

    // Update is called once per frame
    void Update() {
        float step = this.speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, this.targetPosition, step);
    }

    #region PrivateMethods

    // Set a targetPosition randomly within a circular radius of the object's start position 
    private void SetRandomDestination() {
        // get a random offset within the move radius constrained to the x,z plane
        Vector2 randomCircularOffset = Random.insideUnitCircle * this.moveRadius;

        // create a new targetPosition and set just the x and z axis
        Vector3 newTargetOffset = Vector3.zero;
        newTargetOffset.x = this.startPosition.x + randomCircularOffset.x;
        newTargetOffset.y = this.startPosition.y;
        newTargetOffset.z = this.startPosition.z + randomCircularOffset.y;

        // update the current targetPosition
        this.targetPosition = newTargetOffset;
    }

    private void RandomTerrainOperation() {
        int randOperation = Random.Range(0, 5);
        //int randOperation = 3;

        switch (randOperation) {

            // Add to the terrain any part of the Sphere
            // that is currently positioned higher than
            // the intersecting terrain
            // Commit the changes immediately
            case 0:
                this.deformer.AddNow();
                break;

            // Subtract from the terrain any part of the 
            // Sphere currently lower than the terrain
            // Commit the changes immediately
            case 1:
                this.deformer.SubtractNow();
                break;

            // Smooth the terrain at the current positioned
            // of the Sphere and the area immediately 
            // surrounding it
            // Commit the changes immediately
            case 2:
                this.deformer.SmoothNow();
                break;

            // Set the texture of the area of terrain where
            // the Sphere is currently positioned
            // Commit the changes immediately
            case 3:
                this.deformer.TextureNow();
                break;

            // Add, Smooth, and Texture the terrain, but don't
            // commit the changes until the end. It is far faster
            // from a performance standpoint to batch together terrain
            // updates when doing more than one to the same position 			
            case 4:
                this.deformer.Add();
                this.deformer.Smooth();
                this.deformer.Texture();
                this.deformer.SetHeights();
                this.deformer.SetAlphas();
                break;

            // Same as case 4, but Subtract instead of Add
            case 5:
                this.deformer.Subtract();
                this.deformer.Smooth();
                this.deformer.Texture();
                this.deformer.SetHeights();
                this.deformer.SetAlphas();
                break;
        }
    }

    #endregion
}
