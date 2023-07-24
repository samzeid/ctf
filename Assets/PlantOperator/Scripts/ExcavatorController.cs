using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ExcavatorController : MachineController
{
    public int playerId;
    private Player player;

    public float SLEW_RATE = 20.0f;
    public float BOOM_RATE = 10.0f;
    public float ARM_RATE = 40.0f;
    public float BUCKET_RATE = 80.0f;

    public float MIN_BOOM_ROTATION = -85.0f;
    public float MAX_BOOM_ROTATION = -55.0f;
    public float DEFAULT_BOOM_ROTATION = -60.0f;

    // X
    public float MIN_ARM_ROTATION = -130.0f;
    public float MAX_ARM_ROTATION = -20.0f;
    public float DEFAULT_ARM_ROTATION = -90.0f;

    // Z
    public float MIN_BUCKET_ROTATION = -20.0f;
    public float MAX_BUCKET_ROTATION = 125.0f;

    public Transform slew;
    public Transform boom;
    public Transform arm;
    public Transform bucket;

    public Rigidbody bucketEnd;

    [HideInInspector]
    public bool CanIMoveToLeftSide = true;
    [HideInInspector]
    public bool CanIMoveToRightSide = true;
    [HideInInspector]
    public bool CanIMoveToBackSide = true;
    [HideInInspector]
    public bool CanIMoveToUnderneath = true;

    float boom_rotation;
    float arm_rotation;
    float bucket_rotation;

    float joystick1Horz;
    float joystick1Vert;
    float joystick2Horz;
    float joystick2Vert;

    bool joy1Up = false;
    bool joy1Down = false;
    bool joy1Left = false;
    bool joy1Right = false;
    bool joy2Up = false;
    bool joy2Down = false;
    bool joy2Left = false;
    bool joy2Right = false;

    bool fire01 = false;

    // Use this for initialization
    void Start()
    {
        player = ReInput.players.GetPlayer(playerId);

        bucketEnd.transform.SetParent(transform.root);

        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        // get input from joysticks
        joystick1Horz = player.GetAxis("Joystick 1 Horz");
        joystick1Vert = player.GetAxis("Joystick 1 Vert");
        joystick2Horz = player.GetAxis("Joystick 2 Horz");
        joystick2Vert = player.GetAxis("Joystick 2 Vert");

        // translate into movement directions
        joy1Left = joystick1Horz < 0.0f;
        joy1Right = joystick1Horz > 0.0f;
        joy1Up = joystick1Vert < 0.0f;
        joy1Down = joystick1Vert > 0.0f;
        joy2Left = joystick2Horz > 0.0f;
        joy2Right = joystick2Horz < 0.0f;
        joy2Up = joystick2Vert > 0.0f;
        joy2Down = joystick2Vert < 0.0f;

        // keyboard movement
        if (Input.GetKey(KeyCode.A))
        {
            joystick1Horz = -1.0f;
            joy1Left = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            joystick1Horz = 1.0f;
            joy1Right = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            joystick1Vert = -1.0f;
            joy1Down = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            joystick1Vert = 1.0f;
            joy1Up = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            joystick2Horz = 1.0f;
            joy2Left = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            joystick2Horz = -1.0f;
            joy2Right = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            joystick2Vert = 1.0f;
            joy2Up = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            joystick2Vert = -1.0f;
            joy2Down = true;
        }

        if (GetMovementEnabled())
        {
            if (joy1Left && CanIMoveToLeftSide)
            {
                slew_rotation_y += joystick1Horz * SLEW_RATE * Time.deltaTime;
            }

            if (joy1Right && CanIMoveToRightSide)
            {
                slew_rotation_y += joystick1Horz * SLEW_RATE * Time.deltaTime;
            }

            if (joy1Up) //(joy1Up && CanIMoveToBackSide)
            {
                arm_rotation += -joystick1Vert * ARM_RATE * Time.deltaTime;
            }

            if (joy1Down && (CanIMoveToUnderneath || bucket_rotation < 65))
            {
                arm_rotation += -joystick1Vert * ARM_RATE * Time.deltaTime;
            }

            if (joy2Left && CanIMoveToBackSide)
            {
                bucket_rotation += -joystick2Horz * BUCKET_RATE * Time.deltaTime;
            }

            if (joy2Right)
            {
                bucket_rotation += -joystick2Horz * BUCKET_RATE * Time.deltaTime;
            }

            if (joy2Up)
            {
                boom_rotation += joystick2Vert * BOOM_RATE * Time.deltaTime;
            }

            if (joy2Down && CanIMoveToUnderneath)
            {
                boom_rotation += joystick2Vert * BOOM_RATE * Time.deltaTime;
            }

            if (fire01)
            {
                Debug.Log("FIRE01");
            }
        }

        // clamp values to physical bounds
        boom_rotation = Mathf.Clamp(boom_rotation, MIN_BOOM_ROTATION, MAX_BOOM_ROTATION);
        arm_rotation = Mathf.Clamp(arm_rotation, MIN_ARM_ROTATION, MAX_ARM_ROTATION);
        bucket_rotation = Mathf.Clamp(bucket_rotation, MIN_BUCKET_ROTATION, MAX_BUCKET_ROTATION);

        // update positions and rotations
        slew.localRotation = Quaternion.Euler(0.0f, slew_rotation_y, 0.0f);
        boom.localRotation = Quaternion.Euler(0.0f, 0.0f, boom_rotation);
        arm.localRotation = Quaternion.Euler(arm_rotation, -90.0f, 0.0f);
        bucket.localRotation = Quaternion.Euler(0.0f, -90.0f, bucket_rotation);

        bucketEnd.GetComponent<Rigidbody>().MoveRotation(bucket.rotation);
        bucketEnd.GetComponent<Rigidbody>().MovePosition(bucket.position + bucket.rotation * (new Vector3(0, 2.9f, 0) * bucket.lossyScale.x));
    }

    public override bool IsMoving()
    {
        return GetMovementEnabled() && HasAnyInput();
    }

    public override bool HasAnyInput()
    {
        return joy1Left || joy1Right || joy1Up || joy1Down || joy2Left || joy2Right || joy2Up || joy2Down;
    }

    public override void Reset()
    {
        // slew_rotation_y = -20.0f;
        boom_rotation = DEFAULT_BOOM_ROTATION;
        arm_rotation = DEFAULT_ARM_ROTATION;
        bucket_rotation = (MAX_BUCKET_ROTATION + MIN_BUCKET_ROTATION) / 2.0f;
    }

    public Transform GetBucket()
    {
        return bucket;
    }

    public float GetBucketRotation()
    {
        return bucket_rotation;
    }

    public bool GetSlewLeft()
    {
        return joy1Left;
    }

    public bool GetSlewRight()
    {
        return joy1Right;
    }

    public bool GetArmUp()
    {
        return joy1Up;
    }

    public bool GetArmDown()
    {
        return joy1Down;
    }

    public bool GetBucketCurlIn()
    {
        return joy2Left;
    }

    public bool GetBucketCurlOut()
    {
        return joy2Right;
    }

    public bool GetBoomUp()
    {
        return joy2Up;
    }

    public bool GetBoomDown()
    {
        return joy2Down;
    }
}
