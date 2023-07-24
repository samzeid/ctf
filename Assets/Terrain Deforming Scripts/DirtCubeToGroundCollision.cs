using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtCubeToGroundCollision : MonoBehaviour {

    private Vector3 targetPoint;
    private Vector3 dirtCubeHalfExtents = new Vector3(0.08f, 0.08f, 0.08f);
    private Vector3 checkUpwardsPosition;
    private GameObject BucketTipObject;
    private float cubeVelocitySQR; //we check the velocity of the cube. When it is very low velocity (i.e. not moving) then we want to raycast further. This is to stop bug where cube sits on ground.
    private Vector3 cubePositionLastFramePosition;

    private void Start()
    {
        BucketTipObject = GameObject.FindGameObjectWithTag("VelocityPoint");

        if (BucketTipObject == null)
        {
            Debug.LogError("can't find object with tag 'VelocityPoint'");
        }
        dirtCubeHalfExtents *= 5f; // Increase the size of the box check, so we can see if cube is sitting on bucket
    }
    
    void Update ()
    {
        cubeVelocitySQR = (this.transform.position - cubePositionLastFramePosition).sqrMagnitude / (Time.deltaTime * Time.deltaTime);
        cubePositionLastFramePosition = this.transform.position;

        //Check if cube is outside the bucket
        if (Physics.CheckBox(this.transform.position, dirtCubeHalfExtents, Quaternion.identity, LayerMask.GetMask("Default")) == false)
        {
            RaycastHit hit;

            checkUpwardsPosition = this.transform.position;
            checkUpwardsPosition.y -= 0.2f;

            //cast downwards to detect ground
            if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 0.2f, LayerMask.GetMask("Terrain")))
            {
                targetPoint = this.transform.position;
                targetPoint.y = targetPoint.y - hit.distance;

                this.transform.parent.SendMessage("CreateMound", targetPoint);
                Destroy(this.gameObject);
            }

            //if cube is stationary, then do a longer raycast (due to weird bug with raycasting terrain mesh collider
            if (cubeVelocitySQR < 0.0001 && Physics.Raycast(this.transform.position, Vector3.down, out hit, 0.4f, LayerMask.GetMask("Terrain")))
            {
                targetPoint = this.transform.position;
                targetPoint.y = targetPoint.y - hit.distance;

                this.transform.parent.SendMessage("CreateMound", targetPoint);
                Destroy(this.gameObject);
            }
            //another check, to fix bug where raycast and terrain normal cause issues
            else if (Physics.Raycast(checkUpwardsPosition, Vector3.up, out hit, 0.2f, LayerMask.GetMask("Terrain")))
            {
                targetPoint = checkUpwardsPosition;
                targetPoint.y = targetPoint.y + hit.distance;

                this.transform.parent.SendMessage("CreateMound", targetPoint);
                Destroy(this.gameObject);
            }

            //cast upwards to detect ground (for case where lots of cubes are dropped and terrain is deformed upwards beyond the other falling cubes
            else if (Physics.Raycast(this.transform.position, Vector3.up, out hit, 1000f, LayerMask.GetMask("Terrain")))
            {
                targetPoint = this.transform.position;
                targetPoint.y = targetPoint.y + hit.distance;

                this.transform.parent.SendMessage("CreateMound", targetPoint);
                Destroy(this.gameObject);
            }
            else
            { }
        }
    }
}
