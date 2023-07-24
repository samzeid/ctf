using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSignal : MonoBehaviour
{
    //Version 0.3 (Wednesday 06/12/2017)

    [Header("GameObject Links")]
    [SerializeField] GameObject Controller;
    [SerializeField] GameObject HeadObject;
    [SerializeField] GameObject CraneBaseObject;

    private float HoistAngularVelocityThreshold = 160f; //degrees per second. Lower value = user spins hand less to trigger hoist action.
    private float HoistUpwardsAngleThreshold = 60f; //degrees. Higher value means more leniency checking if user is pointing to sky
    private float HoistHandHeightThreshold = 0.20f; //metres. Higher value means user can have hand lower than head and still trigger hoist action
    private float HoistHandDistanceThreshold = 0.0f; //metres. Lower value means user can have hand closer to body (in horizontal plane) and still trigger hoist action
    private float HoistActivationLimit = 0.5f; //seconds. Higher means user will need to do IsHoist action for longer before it will activate/trigger
    private float HoistExpiryLimit = 0.3f; //seconds. Higher means IsHoist will stay active for more time after user stops hoist action. 

    // old IsLower parameters removed 6/12/2017
    /*
    private float LowerAngularVelocityThreshold = 160f; //degrees per second. Lower value = user spins hand less to trigger lower action.
    private float LowerDownwardsAngleThreshold = 60f; //degrees. Higher value means more leniency checking if user is pointing to the ground
    private float LowerHandDistanceThreshold = 0.10f; //metres. Lower value means user can have hand closer to body (in horizontal plane) and still trigger lower action
    private float LowerActivationLimit = 0.5f; //seconds. Higher means user will need to do IsLower action for longer before it will activate/trigger
    private float LowerExpiryLimit = 0.3f; //seconds. Higher means IsLower will stay active for more time after user stops lower action. 
    */
    
    // new IsLower parameters added 6/12/2017
    private float LowerVelocityThreshold = 0.6f; //metres per second. Lower value means user can move hand slower in vertical direction and still trigger IsLower
    private float LowerHandHeightThreshold = 0.3f;// metres. Higher value means more leniency for how high or low the user can have their hand when signalling IsLower.
    private float LowerHandDistanceThreshold = 0.45f; //metres. Higher value means user has to hold hand out further before triggering lower action. (If set too low, user can use right hand to point left across their body and trigger lower)
    private float LowerPerpendicularAngleThreshold = 50f; //degrees. Higher value means more leniency checking if user is pointing perpendicular to crane base (ie. arm is sideways and not too high and low)
    private float LowerActivationLimit = 0.1f; //seconds. Higher means user will need to do IsLower action for longer before it will activate/trigger
    private float LowerExpiryLimit = 0.3f; //seconds. Higher means IsLower will stay active for more time after user stops lower action. 

    private float SwingAngularVelocityThreshold = 100f; //degrees per second. Lower value = user has to keep hand more still in order to trigger swing action.
    private float SwingVelocityThreshold = 0.5f; //metres per second. Lower value means user has to hold hand steadier before IsSwing will trigger
    private float SwingHandDistanceThreshold = 0.45f; //metres. Higher value means user has to hold hand out further before triggering swing action. (If set too low, user can use right hand to point left across their body and trigger swing)
    private float SwingPointPerpendicularAngleThreshold = 50f; //degrees. Higher value means more leniency checking if user is pointing perpendicular to crane base (ie. arm is sideways and not too high and low)
    private float SwingPositionPerpendicularAngleThreshold = 50f; //degrees. Higher value means more leniency checking if user is holding controller to their side, perpendicular to the crane base
    private float SwingHandHeightThreshold = 0.3f;// metres. Higher value means more leniency for how high or low the user can have their hand when signalling swing.
    private float SwingActivationLimit = 0.4f; //seconds. Higher means user will need to do IsSwing action for longer before it will activate/trigger
    private float SwingExpiryLimit = 0.2f; //seconds. Higher means IsSwingLeft will stay active for more time after user stops swing left action. 

    // old stop parameters removed 6/12/2017
    /*
    private float StopTimerLeftQuadrant1 = 0f;
    private float StopTimerLeftQuadrant2 = 0f;
    private float StopTimerLeftQuadrant3 = 0f;
    private float StopTimerRightQuadrant1 = 0f;
    private float StopTimerRightQuadrant2 = 0f;
    private float StopTimerRightQuadrant3 = 0f;
    private float StopHandDistanceThreshold = 0.5f; //metres. Higher value means user needs to swing their hand out further to trigger stop
    private float QuadrantTimeLimit = 0.2f; //This is how quick the user must move their hand through the quadrants to signal IsStop. Higher means user can move hand slower and still trigger is stop
    private float StopExpiryLimit = 0.7f; //seconds. This is how long the IsStop bool stays on for, after signalling a stop.
    */

    private float StopHandHeightThreshold = 0.3f; //metres.Higher value means more leniency for how high or low the user can have their hand when signalling stop.
    private float StopHandDistanceThreshold = 0.45f; //metres. Higher value means user has to hold hand out further before triggering stop action. (If set too low, user can have very bent elbow, holding hand infront of face and trigger stop)
    private float StopForwardAngleThreshold = 50f; //degrees. Higher value means more leniency checking if user has hand in front of them, between suer and crane.(ie. arm is forwards and not too high, low or sidewars)
    private float StopVelocityThreshold = 0.4f; //metres per second. Lower value means user has to hold hand steadier before IsStop will trigger
    private float StopActivationLimit = 0.4f; //seconds. Higher means user will need to do IsStop action for longer before it will activate/trigger
    private float StopExpiryLimit = 0.2f; //seconds. Higher means IsStop will stay active for more time after user stops the stop gesture. 

    private Quaternion Quaternion_DeltaRot;
    private Quaternion Quaternion_LastRot;
    private Vector3 Vector3_EulerRot;
    private Vector3 Vector3_WeaponAngularVelocity;
    private Vector3 HorizontalPlaneHand;
    private Vector3 HorizontalPlaneHead;
    private Vector3 HorizontalPlaneCraneBase;
    private Vector3 HandPositionRelativeToHead;
    private Vector3 HandPositionLastFrame;
    private Vector3 HandVelocity;

    private float HoistActivateTimer = 0f;
    private float LowerActivateTimer = 0f;
    private float SwingLeftActivateTimer = 0f;
    private float SwingRightActivateTimer = 0f;
    private float StopActivateTimer = 0f;
    private float AngleOfHandRelativeToHead; //0 to 360degrees, rotating clockwise. This is the angle of where the hand is, in relation to the head. where 0degrees is hand is exactly between head and crane

    private float HoistExpiryTimer = 0f;
    private float LowerExpiryTimer = 0f;
    private float SwingLeftExpiryTimer = 0f;
    private float SwingRightExpiryTimer = 0f;
    private float StopExpiryTimer = 0f;

    private bool HoistBool = false;
    private bool LowerBool = false;
    private bool SwingLeftBool = false;
    private bool SwingRightBool = false;
    private bool StopBool = false;

    private bool HoistCheck1 = false;
    private bool HoistCheck2 = false;
    private bool HoistCheck3 = false;
    private bool HoistCheck4 = false;

    private bool LowerCheck1 = false;
    private bool LowerCheck2 = false;
    private bool LowerCheck3 = false;
    private bool LowerCheck4 = false;

    private bool SwingLeftCheck1 = false;
    private bool SwingLeftCheck2 = false;
    private bool SwingLeftCheck3 = false;
    private bool SwingLeftCheck4 = false;
    private bool SwingLeftCheck5 = false;
    private bool SwingLeftCheck6 = false;

    private bool SwingRightCheck1 = false;
    private bool SwingRightCheck2 = false;
    private bool SwingRightCheck3 = false;
    private bool SwingRightCheck4 = false;
    private bool SwingRightCheck5 = false;
    private bool SwingRightCheck6 = false;

    private bool StopCheck1 = false;
    private bool StopCheck2 = false;
    private bool StopCheck3 = false;
    private bool StopCheck4 = false;

    private GameObject GameObjectForCalculatingStopMotion; //this gameobject sits exactly on the players head, but faces towards the point in space at the cranebase and at the same height as player's head.
    private Vector3 LookDirectionForCalculatingStopMotion = new Vector3(0f, 0f, 0f);


    void Start()
    {
        Quaternion_LastRot = Controller.transform.rotation;
        HandPositionLastFrame = Controller.transform.position;
        GameObjectForCalculatingStopMotion = new GameObject("GameObjectForCalculatingStopMotion");
        GameObjectForCalculatingStopMotion.transform.position = HeadObject.transform.position;
        GameObjectForCalculatingStopMotion.transform.parent = HeadObject.transform;
    }

    void Update()
    {
        CalculateAngularVelocity();
        CalculateVelocity();
        CheckIfHoist();
        CheckIfLower();
        CheckIfSwingLeft();
        CheckIfSwingRight();
        CheckIfStop();

        //this is the old check stop code, removed 6/12/2017
        //CalculateHandAngleRelativeToHead();
    }

    private void CalculateAngularVelocity()
    {
        Quaternion_DeltaRot = Controller.transform.rotation * Quaternion.Inverse(Quaternion_LastRot);
        Vector3_EulerRot = new Vector3(Mathf.DeltaAngle(0, Quaternion_DeltaRot.eulerAngles.x), Mathf.DeltaAngle(0, Quaternion_DeltaRot.eulerAngles.y), Mathf.DeltaAngle(0, Quaternion_DeltaRot.eulerAngles.z));
        Vector3_WeaponAngularVelocity = Vector3_EulerRot / Time.deltaTime;
        //Vector3_WeaponAngularVelocity = Controller.transform.InverseTransformPoint(Vector3_WeaponAngularVelocity + Controller.transform.position); //local rotation
        Quaternion_LastRot = Controller.transform.rotation;
    }

    private void CalculateVelocity()
    {
        HandVelocity = (Controller.transform.position - HandPositionLastFrame) / Time.deltaTime;
        HandPositionLastFrame = Controller.transform.position;
    }

    //this is the old check stop code, removed 6/12/2017
    /*
    private void CalculateHandAngleRelativeToHead()
    {
        //This gets a target look point which is at the crane base, but at the same height as the player's head (horizontal plane)
        LookDirectionForCalculatingStopMotion.x = CraneBaseObject.transform.position.x;
        LookDirectionForCalculatingStopMotion.y = GameObjectForCalculatingStopMotion.transform.position.y;
        LookDirectionForCalculatingStopMotion.z = CraneBaseObject.transform.position.z;

        //This makes the helper gameobject (locked to player's head position) face the crane (only in the horizontal plane, as look target is always at player's head height)
        GameObjectForCalculatingStopMotion.transform.LookAt(LookDirectionForCalculatingStopMotion); 

        //We then calculate the position of the hand relative to this helper gameobject.
        HandPositionRelativeToHead = GameObjectForCalculatingStopMotion.transform.InverseTransformPoint(Controller.transform.position);

        //We then use this relative position to calculate the angle between two lines. Where line one is player's head to crane, and line two is players head to controller. 
        //I.e. when angle = 0f, then hand is directly between the head and the crane, in horizontal plane. When 90 degrees, this user is extending their hand to the right (the crane driver's left) and is independant of where the head is looking.
        //I.e. imagine user's chest is always facing crane. This angle we're calculating is angle around the body where the hand is, relative to the forward direction of the chest. (range 0 to 360, i.e. just like compass bearings)
        if (HandPositionRelativeToHead.x == 0 && HandPositionRelativeToHead.z == 0) { AngleOfHandRelativeToHead = 0f; }
        else if (HandPositionRelativeToHead.x == 0 && HandPositionRelativeToHead.z > 0) { AngleOfHandRelativeToHead = 0f; }
        else if (HandPositionRelativeToHead.x > 0 && HandPositionRelativeToHead.z == 0) { AngleOfHandRelativeToHead = 90f; }
        else if (HandPositionRelativeToHead.x == 0 && HandPositionRelativeToHead.z < 0) { AngleOfHandRelativeToHead = 180f; }
        else if (HandPositionRelativeToHead.x < 0 && HandPositionRelativeToHead.z == 0) { AngleOfHandRelativeToHead = 270f; }
        else if (HandPositionRelativeToHead.x > 0 && HandPositionRelativeToHead.z > 0) { AngleOfHandRelativeToHead = 57.2958f * Mathf.Atan(HandPositionRelativeToHead.x / HandPositionRelativeToHead.z); }
        else if (HandPositionRelativeToHead.x > 0 && HandPositionRelativeToHead.z < 0) { AngleOfHandRelativeToHead = 90f + 57.2958f * Mathf.Atan(Mathf.Abs(HandPositionRelativeToHead.z) / Mathf.Abs(HandPositionRelativeToHead.x)); }
        else if (HandPositionRelativeToHead.x < 0 && HandPositionRelativeToHead.z < 0) { AngleOfHandRelativeToHead = 180f + 57.2958f * Mathf.Atan(Mathf.Abs(HandPositionRelativeToHead.x) / Mathf.Abs(HandPositionRelativeToHead.z)); }
        else if (HandPositionRelativeToHead.x < 0 && HandPositionRelativeToHead.z > 0) { AngleOfHandRelativeToHead = 270f + 57.2958f * Mathf.Atan(Mathf.Abs(HandPositionRelativeToHead.z) / Mathf.Abs(HandPositionRelativeToHead.x)); }
        else { }

        StopTimerLeftQuadrant1 += Time.deltaTime;
        StopTimerLeftQuadrant2 += Time.deltaTime;
        StopTimerLeftQuadrant3 += Time.deltaTime;
        StopTimerRightQuadrant1 += Time.deltaTime;
        StopTimerRightQuadrant2 += Time.deltaTime;
        StopTimerRightQuadrant3 += Time.deltaTime;

        //left quadrant 1: 315 to 30 degrees
        if (AngleOfHandRelativeToHead >= 315f || AngleOfHandRelativeToHead <= 30f)
        {
            StopTimerLeftQuadrant1 = 0f;
        }

        //left quadrant 2: 285 to 315 degrees
        if (AngleOfHandRelativeToHead >= 285f && AngleOfHandRelativeToHead <= 315f)
        {
            StopTimerLeftQuadrant2 = 0f;
        }

        //left quadrant 3: 240 to 285 degrees
        if (AngleOfHandRelativeToHead >= 240f && AngleOfHandRelativeToHead <= 285f)
        {
            StopTimerLeftQuadrant3 = 0f;
        }

        //right quadrant 1: 270 to 45 degrees
        if (AngleOfHandRelativeToHead >= 270f || AngleOfHandRelativeToHead <= 45f)
        {
            StopTimerRightQuadrant1 = 0f;
        }

        //right quadrant 2: 45 to 75 degrees
        if (AngleOfHandRelativeToHead >= 45f && AngleOfHandRelativeToHead <= 75f)
        {
            StopTimerRightQuadrant2 = 0f;
        }

        //right quadrant 3: 75 to 120 degrees
        if (AngleOfHandRelativeToHead >= 75f && AngleOfHandRelativeToHead <= 120f)
        {
            StopTimerRightQuadrant3 = 0f;
        }

        if (StopCheck1 && ((StopTimerLeftQuadrant1 < QuadrantTimeLimit && StopTimerLeftQuadrant2 < QuadrantTimeLimit && StopTimerLeftQuadrant3 < QuadrantTimeLimit) || (StopTimerRightQuadrant1 < QuadrantTimeLimit && StopTimerRightQuadrant2 < QuadrantTimeLimit && StopTimerRightQuadrant3 < QuadrantTimeLimit))) 
        {
            StopBool = true;
        }

        HorizontalPlaneHand.x = Controller.transform.position.x;
        HorizontalPlaneHand.y = 0f;
        HorizontalPlaneHand.z = Controller.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        if (Vector3.SqrMagnitude(HorizontalPlaneHand - HorizontalPlaneHead) > (StopHandDistanceThreshold * StopHandDistanceThreshold)) //Use SQR mag as more efficient/faster
        {
            StopCheck1 = true;
        }
        else
        {
            StopCheck1 = false;
        }

        if (StopBool == true)
        {
            StopExpiryTimer += Time.deltaTime;

            if (StopExpiryTimer > StopExpiryLimit)
            {
                StopBool = false;
                StopExpiryTimer = 0f;
            }
        }
    }
    */

    private void CheckIfHoist()
    {
        //Hoist Check 1: Is angular velocity greater than threshold?
        if (Vector3_WeaponAngularVelocity.sqrMagnitude > (HoistAngularVelocityThreshold * HoistAngularVelocityThreshold)) //Use SQR mag as more efficient/faster
        {
            HoistCheck1 = true;
        }
        else
        {
            HoistCheck1 = false;
        }

        //Hoist Check 2: Is controller pointing towards the sky?
        if (Vector3.Angle(Controller.transform.forward, Vector3.up) < HoistUpwardsAngleThreshold)
        {
            HoistCheck2 = true;
        }
        else
        {
            HoistCheck2 = false;
        }

        //Hoist Check 3: is controller near head height or higher?
        if (Controller.transform.position.y > (HeadObject.transform.position.y - HoistHandHeightThreshold))
        {
            HoistCheck3 = true;
        }
        else
        {
            HoistCheck3 = false;
        }

        //Hoist Check 4: is controller far enough away from head

        HorizontalPlaneHand.x = Controller.transform.position.x;
        HorizontalPlaneHand.y = 0f;
        HorizontalPlaneHand.z = Controller.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        if (Vector3.SqrMagnitude(HorizontalPlaneHand - HorizontalPlaneHead) > (HoistHandDistanceThreshold * HoistHandDistanceThreshold)) //Use SQR mag as more efficient/faster
        {
            HoistCheck4 = true;
        }
        else
        {
            HoistCheck4 = false;
        }

        //Check if we can set IsHoist to true
        if (HoistCheck1 && HoistCheck2 && HoistCheck3 && HoistCheck4)
        {
            HoistActivateTimer += Time.deltaTime;

            //Checks if user has been doing action for long enough before activating/triggering
            if (HoistActivateTimer >= HoistActivationLimit)
            {
                HoistBool = true; //Set IsHoist to true
                HoistExpiryTimer = 0f;
                HoistActivateTimer = HoistActivationLimit + Time.deltaTime; //Don't let timer get too big.
            }
            else { }

            if (HoistBool == true)
            {
                HoistActivateTimer = HoistActivationLimit + Time.deltaTime;
            }
            else { }
        }
        else
        {
            HoistActivateTimer -= Time.deltaTime;
            HoistExpiryTimer += Time.deltaTime;

            if (HoistActivateTimer < 0f)
            {
                HoistActivateTimer = 0f;
            }
        }

        //Sets IsHoist to false after x seconds of not satisfying hoist criteria
        if (HoistBool && HoistExpiryTimer > HoistExpiryLimit)
        {
            HoistBool = false; 
        }

    }

    //This was for the first Lower gesture, which we changed 6/12/2017 to a new gesture

    /*
    private void CheckIfLower()
    {
        //Lower Check 1: Is angular velocity greater than threshold?
        if (Vector3_WeaponAngularVelocity.sqrMagnitude > (LowerAngularVelocityThreshold * LowerAngularVelocityThreshold)) //Use SQR mag as more efficient/faster
        {
            LowerCheck1 = true;
        }
        else
        {
            LowerCheck1 = false;
        }

        //Lower Check 2: Is controller pointing towards the ground?
        if (Vector3.Angle(Controller.transform.forward, Vector3.down) < LowerDownwardsAngleThreshold)
        {
            LowerCheck2 = true;
        }
        else
        {
            LowerCheck2 = false;
        }

        //Lower Check 3: is controller near head height or higher?
        if (Controller.transform.position.y < (HeadObject.transform.position.y))
        {
            LowerCheck3 = true;
        }
        else
        {
            LowerCheck3 = false;
        }

        //Lower Check 4: is controller far enough away from head

        HorizontalPlaneHand.x = Controller.transform.position.x;
        HorizontalPlaneHand.y = 0f;
        HorizontalPlaneHand.z = Controller.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        if (Vector3.SqrMagnitude(HorizontalPlaneHand - HorizontalPlaneHead) > (LowerHandDistanceThreshold * LowerHandDistanceThreshold)) //Use SQR mag as more efficient/faster
        {
            LowerCheck4 = true;
        }
        else
        {
            LowerCheck4 = false;
        }

        //Check if we can set IsLower to true
        if (LowerCheck1 && LowerCheck2 && LowerCheck3 && LowerCheck4)
        {
            LowerActivateTimer += 2f * Time.deltaTime;

            //Checks if user has been doing action for long enough before activating/triggering
            if (LowerActivateTimer >= LowerActivationLimit)
            {
                LowerBool = true; //Set IsLower to true
                LowerExpiryTimer = 0f;
                LowerActivateTimer = LowerActivationLimit + Time.deltaTime; //Don't let timer get too big.
            }

            if (LowerBool == true)
            {
                LowerActivateTimer = LowerActivationLimit + Time.deltaTime;
            }
        }
        else
        {
            LowerActivateTimer -= Time.deltaTime;
            LowerExpiryTimer += Time.deltaTime;

            if (LowerActivateTimer < 0f)
            {
                LowerActivateTimer = 0f;
            }
        }

        //Sets IsLower to false after x seconds of not satisfying lower criteria
        if (LowerBool && LowerExpiryTimer > LowerExpiryLimit)
        {
            LowerBool = false;
        }
    }
    */

    //this is the new lower gesture, created 6/12/2017
    private void CheckIfLower()
    {
        //Lower Check 1: is vertical velocity (absolute) greater than threshold?
        if ( Mathf.Abs(HandVelocity.y) > LowerVelocityThreshold)
        {
            LowerCheck1 = true;
        }
        else
        {
            LowerCheck1 = false;
        }

        //Lower Check 2: is user's arm within a certain height from head?
        if (Controller.transform.position.y < (HeadObject.transform.position.y - 0.20 + LowerHandHeightThreshold) && (Controller.transform.position.y > (HeadObject.transform.position.y - 0.15 - LowerHandHeightThreshold)))
        {
            LowerCheck2 = true;
        }
        else
        {
            LowerCheck2 = false;
        }

        //Lower Check 3: is controller far enough away from body?
        HorizontalPlaneHand.x = Controller.transform.position.x;
        HorizontalPlaneHand.y = 0f;
        HorizontalPlaneHand.z = Controller.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        if (Vector3.SqrMagnitude(HorizontalPlaneHand - HorizontalPlaneHead) > (LowerHandDistanceThreshold * LowerHandDistanceThreshold)) //Use SQR mag as more efficient/faster
        {
            LowerCheck3 = true;
        }
        else
        {
            LowerCheck3 = false;
        }

        //Lower Check 4: is user's arm and controller held out sideways (and perpendicular from the line extending from user to crane base, hence crane operator can clearly see arm held out sideways)

        HorizontalPlaneCraneBase.x = CraneBaseObject.transform.position.x;
        HorizontalPlaneCraneBase.y = 0f;
        HorizontalPlaneCraneBase.z = CraneBaseObject.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        //Get line from user to crane base (in horizontal plane). Then rotate that 90degrees anti-clockwise about y axis. Then get angle between that line (user's sideways direction) and controller point direction, then check the angle between those.
        if ( (Vector3.Angle((Quaternion.Euler(0, -90, 0) * (HorizontalPlaneCraneBase - HorizontalPlaneHead)), Controller.transform.forward) < LowerPerpendicularAngleThreshold) || (Vector3.Angle((Quaternion.Euler(0, 90, 0) * (HorizontalPlaneCraneBase - HorizontalPlaneHead)), Controller.transform.forward) < LowerPerpendicularAngleThreshold))
        {
            LowerCheck4 = true;
        }
        else
        {
            LowerCheck4 = false;
        }

        //Check if we can set IsLower to true
        if (LowerCheck1 && LowerCheck2 && LowerCheck3 && LowerCheck4)
        {
            LowerActivateTimer += 2f * Time.deltaTime;

            //Checks if user has been doing action for long enough before activating/triggering
            if (LowerActivateTimer >= LowerActivationLimit)
            {
                LowerBool = true; //Set IsLower to true
                LowerExpiryTimer = 0f;
                LowerActivateTimer = LowerActivationLimit + Time.deltaTime; //Don't let timer get too big.
            }

            if (LowerBool == true)
            {
                LowerActivateTimer = LowerActivationLimit + Time.deltaTime;
            }
        }
        else
        {
            LowerActivateTimer -= Time.deltaTime;
            LowerExpiryTimer += Time.deltaTime;

            if (LowerActivateTimer < 0f)
            {
                LowerActivateTimer = 0f;
            }
        }

        //Sets IsLower to false after x seconds of not satisfying lower criteria
        if (LowerBool && LowerExpiryTimer > LowerExpiryLimit)
        {
            LowerBool = false;
        }
    }
        

    private void CheckIfSwingLeft()
    {
        //Swing Left Check 1: is angular velocity low enough?
        if (Vector3_WeaponAngularVelocity.sqrMagnitude < (SwingAngularVelocityThreshold * SwingAngularVelocityThreshold)) //Use SQR mag as more efficient/faster
        {
            SwingLeftCheck1 = true;
        }
        else
        {
            SwingLeftCheck1 = false;
        }

        //Swing Left Check 2: is controller far enough away from body?
        HorizontalPlaneHand.x = Controller.transform.position.x;
        HorizontalPlaneHand.y = 0f;
        HorizontalPlaneHand.z = Controller.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        if (Vector3.SqrMagnitude(HorizontalPlaneHand - HorizontalPlaneHead) > (SwingHandDistanceThreshold * SwingHandDistanceThreshold)) //Use SQR mag as more efficient/faster
        {
            SwingLeftCheck2 = true;
        }
        else
        {
            SwingLeftCheck2 = false;
        }

        //Swing Left Check 3: is user's controller pointing out sideways (and pointing perpendicular from the line extending from user to crane base)

        HorizontalPlaneCraneBase.x = CraneBaseObject.transform.position.x;
        HorizontalPlaneCraneBase.y = 0f;
        HorizontalPlaneCraneBase.z = CraneBaseObject.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        //Get line from user to crane base (in horizontal plane). Then rotate that 90degrees anti-clockwise about y axis. Then get angle between that line (user's sideways direction) and controller point direction, then check the angle between those.
        if (Vector3.Angle( (Quaternion.Euler(0, -90, 0) * (HorizontalPlaneCraneBase - HorizontalPlaneHead)), Controller.transform.forward) < SwingPointPerpendicularAngleThreshold)
        {
            SwingLeftCheck3 = true;
        }
        else
        {
            SwingLeftCheck3 = false;
        }

        //Swing Left Check 4: is user's arm within a certain height from head?
        if (Controller.transform.position.y < (HeadObject.transform.position.y - 0.20 + SwingHandHeightThreshold) && (Controller.transform.position.y > (HeadObject.transform.position.y - 0.15 - SwingHandHeightThreshold)))
        {
            SwingLeftCheck4 = true;
        }
        else
        {
            SwingLeftCheck4 = false;
        }

        //Swing Left Check 5: is user's arm and controller held out sideways (and perpendicular from the line extending from user to crane base, hence crane operator can clearly see arm held out sideways)

        HorizontalPlaneCraneBase.x = CraneBaseObject.transform.position.x;
        HorizontalPlaneCraneBase.y = 0f;
        HorizontalPlaneCraneBase.z = CraneBaseObject.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        //Get line from user to crane base (in horizontal plane). Then rotate that 90degrees anti-clockwise about y axis. Then get angle between that line (user's sideways direction) and controller point direction, then check the angle between those.
        if (Vector3.Angle((Quaternion.Euler(0, -90, 0) * (HorizontalPlaneCraneBase - HorizontalPlaneHead)), (Controller.transform.position - HeadObject.transform.position)) < SwingPositionPerpendicularAngleThreshold)
        {
            SwingLeftCheck5 = true;
        }
        else
        {
            SwingLeftCheck5 = false;
        }

        //Swing Left Check 6: is vertical velocity (absolute) less than threshold?
        if (Mathf.Abs(HandVelocity.sqrMagnitude) < (SwingVelocityThreshold * SwingVelocityThreshold))
        {
            SwingLeftCheck6 = true;
        }
        else
        {
            SwingLeftCheck6 = false;
        }

        //Check if we can set IsSwingLeft to true
        if (SwingLeftCheck1 && SwingLeftCheck2 && SwingLeftCheck3 && SwingLeftCheck4 && SwingLeftCheck5 && SwingLeftCheck6)
        {
            SwingLeftActivateTimer += Time.deltaTime;

            //Checks if user has been doing action for long enough before activating/triggering
            if (SwingLeftActivateTimer >= SwingActivationLimit)
            {
                SwingLeftBool = true; //Set IsSwingLeft to true
                SwingLeftActivateTimer = SwingActivationLimit + Time.deltaTime; //Don't let timer get too big.
                SwingLeftExpiryTimer = 0f;
            }

            if (SwingLeftBool == true)
            {
                SwingLeftActivateTimer = SwingActivationLimit + Time.deltaTime;
            }
        }
        else
        {
            SwingLeftActivateTimer -= Time.deltaTime;
            SwingLeftExpiryTimer += Time.deltaTime;

            if (SwingLeftActivateTimer < 0f)
            {
                SwingLeftActivateTimer = 0f;
            }
        }

        //Sets IsSwingLeft to false after x seconds of not satisfying SwingLeft criteria
        if (SwingLeftBool && SwingLeftExpiryTimer > SwingExpiryLimit)
        {
            SwingLeftBool = false;
        }
    }

    private void CheckIfSwingRight()
    {
        //Swing Right Check 1: is angular velocity low enough?
        if (Vector3_WeaponAngularVelocity.sqrMagnitude < (SwingAngularVelocityThreshold * SwingAngularVelocityThreshold)) //Use SQR mag as more efficient/faster
        {
            SwingRightCheck1 = true;
        }
        else
        {
            SwingRightCheck1 = false;
        }

        //Swing Right Check 2: is controller far enough away from body?
        HorizontalPlaneHand.x = Controller.transform.position.x;
        HorizontalPlaneHand.y = 0f;
        HorizontalPlaneHand.z = Controller.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        if (Vector3.SqrMagnitude(HorizontalPlaneHand - HorizontalPlaneHead) > (SwingHandDistanceThreshold * SwingHandDistanceThreshold)) //Use SQR mag as more efficient/faster
        {
            SwingRightCheck2 = true;
        }
        else
        {
            SwingRightCheck2 = false;
        }

        //Swing Right Check 3: is user's controller pointing out sideways (and pointing perpendicular from the line extending from user to crane base)
        HorizontalPlaneCraneBase.x = CraneBaseObject.transform.position.x;
        HorizontalPlaneCraneBase.y = 0f;
        HorizontalPlaneCraneBase.z = CraneBaseObject.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        //Get line from user to crane base (in horizontal plane). Then rotate that 90degrees clockwise about y axis. Then get angle between that line (user's sideways direction) and controller point direction, then check the angle between those.
        if (Vector3.Angle((Quaternion.Euler(0, 90, 0) * (HorizontalPlaneCraneBase - HorizontalPlaneHead)), Controller.transform.forward) < SwingPointPerpendicularAngleThreshold)
        {
            SwingRightCheck3 = true;
        }
        else
        {
            SwingRightCheck3 = false;
        }

        //Swing Right Check 4: is user's arm within a certain height from head?
        if (Controller.transform.position.y < (HeadObject.transform.position.y - 0.20 + SwingHandHeightThreshold) && (Controller.transform.position.y > (HeadObject.transform.position.y - 0.15 - SwingHandHeightThreshold)))
        {
            SwingRightCheck4 = true;
        }
        else
        {
            SwingRightCheck4 = false;
        }

        //Swing Right Check 5: is user's arm and controller held out sideways (and perpendicular from the line extending from user to crane base, hence crane operator can clearly see arm held out sideways)
        HorizontalPlaneCraneBase.x = CraneBaseObject.transform.position.x;
        HorizontalPlaneCraneBase.y = 0f;
        HorizontalPlaneCraneBase.z = CraneBaseObject.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        //Get line from user to crane base (in horizontal plane). Then rotate that 90degrees clockwise about y axis. Then get angle between that line (user's sideways direction) and controller point direction, then check the angle between those.
        if (Vector3.Angle((Quaternion.Euler(0, 90, 0) * (HorizontalPlaneCraneBase - HorizontalPlaneHead)), (Controller.transform.position - HeadObject.transform.position)) < SwingPositionPerpendicularAngleThreshold)
        {
            SwingRightCheck5 = true;
        }
        else
        {
            SwingRightCheck5 = false;
        }

        //Swing Right Check 6: is vertical velocity (absolute) less than threshold?
        if (Mathf.Abs(HandVelocity.sqrMagnitude) < (SwingVelocityThreshold * SwingVelocityThreshold))
        {
            SwingRightCheck6 = true;
        }
        else
        {
            SwingRightCheck6 = false;
        }

        //Check if we can set IsSwingRight to true
        if (SwingRightCheck1 && SwingRightCheck2 && SwingRightCheck3 && SwingRightCheck4 && SwingRightCheck5 && SwingLeftCheck6)
        {
            SwingRightActivateTimer += Time.deltaTime;

            //Checks if user has been doing action for long enough before activating/triggering
            if (SwingRightActivateTimer >= SwingActivationLimit)
            {
                SwingRightBool = true; //Set IsSwingRight to true
                SwingRightActivateTimer = SwingActivationLimit + Time.deltaTime; //Don't let timer get too big.
                SwingRightExpiryTimer = 0f;
            }

            if (SwingRightBool == true)
            {
                SwingRightActivateTimer = SwingActivationLimit + Time.deltaTime;
            }
        }
        else
        {
            SwingRightActivateTimer -= Time.deltaTime;
            SwingRightExpiryTimer += Time.deltaTime;

            if (SwingRightActivateTimer < 0f)
            {
                SwingRightActivateTimer = 0f;
            }
        }

        //Sets IsSwingRight to false after x seconds of not satisfying SwingRight criteria
        if (SwingRightBool && SwingRightExpiryTimer > SwingExpiryLimit)
        {
            SwingRightBool = false;
        }
    }

    private void CheckIfStop()
    {
        //Stop Check 1: is user's arm within a certain height from head?
        if (Controller.transform.position.y < (HeadObject.transform.position.y + StopHandHeightThreshold) && (Controller.transform.position.y > (HeadObject.transform.position.y - 0.15 - StopHandHeightThreshold)))
        {
            StopCheck1 = true;
        }
        else
        {
            StopCheck1 = false;
        }

        //Stop Check 2: is controller far enough away from body?
        HorizontalPlaneHand.x = Controller.transform.position.x;
        HorizontalPlaneHand.y = 0f;
        HorizontalPlaneHand.z = Controller.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        if (Vector3.SqrMagnitude(HorizontalPlaneHand - HorizontalPlaneHead) > (StopHandDistanceThreshold * StopHandDistanceThreshold)) //Use SQR mag as more efficient/faster
        {
            StopCheck2 = true;
        }
        else
        {
            StopCheck2 = false;
        }

        //Stop Check 3: is user's arm and controller held out forward (sitting on the line extending from user to crane base, hence crane operator can clearly see arm held out forward)

        HorizontalPlaneCraneBase.x = CraneBaseObject.transform.position.x;
        HorizontalPlaneCraneBase.y = 0f;
        HorizontalPlaneCraneBase.z = CraneBaseObject.transform.position.z;

        HorizontalPlaneHead.x = HeadObject.transform.position.x;
        HorizontalPlaneHead.y = 0f;
        HorizontalPlaneHead.z = HeadObject.transform.position.z;

        //Get line from user to crane base (in horizontal plane).  Then get angle between that line and the line from user to controller, then check the angle between those.
        if (Vector3.Angle((Quaternion.Euler(0, 0, 0) * (HorizontalPlaneCraneBase - HorizontalPlaneHead)), (Controller.transform.position - HeadObject.transform.position)) < StopForwardAngleThreshold)
        {
            StopCheck3 = true;
        }
        else
        {
            StopCheck3 = false;
        }

        //Stop Check 4: is vertical velocity (absolute) less than threshold?f
        if (Mathf.Abs(HandVelocity.sqrMagnitude) < (StopVelocityThreshold* StopVelocityThreshold))
        {
            StopCheck4 = true;
        }
        else
        {
            StopCheck4 = false;
        }

        //Check if we can set IsStop to true
        if (StopCheck1 && StopCheck2 && StopCheck3 && StopCheck4)
        {
            StopActivateTimer += Time.deltaTime;

            //Checks if user has been doing action for long enough before activating/triggering
            if (StopActivateTimer >= StopActivationLimit)
            {
                StopBool = true; //Set IsSwingRight to true
                StopActivateTimer = StopActivationLimit + Time.deltaTime; //Don't let timer get too big.
                StopExpiryTimer = 0f;
            }

            if (StopBool == true)
            {
                StopActivateTimer = StopActivationLimit + Time.deltaTime;
            }
        }
        else
        {
            StopActivateTimer -= Time.deltaTime;
            StopExpiryTimer += Time.deltaTime;

            if (StopActivateTimer < 0f)
            {
                StopActivateTimer = 0f;
            }
        }

        //Sets IsStop to false after x seconds of not satisfying IsStop criteria
        if (StopBool && StopExpiryTimer > StopExpiryLimit)
        {
            StopBool = false;
        }

    }

    public bool CheckHoist
    {
        get
        {
            return HoistBool;
        }
    }
    public bool CheckLower
    {
        get
        {
            return LowerBool;
        }
    }
    public bool CheckSwingLeft
    {
        get
        {
            return SwingLeftBool;
        }
    }
    public bool CheckSwingRight
    {
        get
        {
            return SwingRightBool;
        }
    }
    public bool CheckStop
    {
        get
        {
            return StopBool;
        }
    }
}
