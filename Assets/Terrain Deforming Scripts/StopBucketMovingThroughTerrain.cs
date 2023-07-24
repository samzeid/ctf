using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBucketMovingThroughTerrain : MonoBehaviour {

    [SerializeField] ExcavatorController excavatorController;
    [SerializeField] GameObject[] LeftSideRaycastObjects;
    [SerializeField] GameObject[] RightSideRaycastObjects;
    [SerializeField] GameObject[] BackSideRaycastObjects;
    [SerializeField] GameObject[] UnderneathRaycastObjects;

    private bool CanIMoveToLeftSide = true;
    private bool CanIMoveToRightSide = true;
    private bool CanIMoveToBackSide = true;
    private bool CanIMoveToUnderneath = true;
	
	void Update ()
    {
        SetMovementBoolsToTrue();

        CheckLeftSide();
        CheckRightSide();
        CheckBackSide();
        CheckUnderneath();

        SetBoolsOnExcavatorController();
    }

    void SetMovementBoolsToTrue()
    {
        CanIMoveToLeftSide = true;
        CanIMoveToRightSide = true;
        CanIMoveToBackSide = true;
        CanIMoveToUnderneath = true;
    }

    void CheckLeftSide()
    {
        foreach (GameObject raycastobject in LeftSideRaycastObjects)
        {
            if (Physics.Raycast(raycastobject.transform.position, raycastobject.transform.forward, 0.1f, LayerMask.GetMask("Terrain")))
            {
                CanIMoveToLeftSide = false;
            }
        }
    }

    void CheckRightSide()
    {
        foreach (GameObject raycastobject in RightSideRaycastObjects)
        {
            if (Physics.Raycast(raycastobject.transform.position, raycastobject.transform.forward, 0.1f, LayerMask.GetMask("Terrain")))
            {
                CanIMoveToRightSide = false;
            }
        }
    }

    void CheckBackSide()
    {
        foreach (GameObject raycastobject in BackSideRaycastObjects)
        {
            if (Physics.Raycast(raycastobject.transform.position, raycastobject.transform.forward, 0.1f, LayerMask.GetMask("Terrain")))
            {
                CanIMoveToBackSide = false;
            }
        }
    }

    void CheckUnderneath()
    {
        foreach (GameObject raycastobject in UnderneathRaycastObjects)
        {
            if (Physics.Raycast(raycastobject.transform.position, raycastobject.transform.forward, 0.1f, LayerMask.GetMask("Terrain")))
            {
                CanIMoveToUnderneath = false;
            }
        }
    }

    void SetBoolsOnExcavatorController()
    {
        excavatorController.CanIMoveToLeftSide = CanIMoveToLeftSide;
        excavatorController.CanIMoveToRightSide = CanIMoveToRightSide;
        excavatorController.CanIMoveToBackSide = CanIMoveToBackSide;
        excavatorController.CanIMoveToUnderneath = CanIMoveToUnderneath;

        if (CanIMoveToLeftSide == false)
        {
           // Debug.Log("Contact Left Side!!!");
        }

        if (CanIMoveToRightSide == false)
        {
           // Debug.Log("Contact Right Side!!!");
        }

        if (CanIMoveToBackSide == false)
        {
           // Debug.Log("Contact Back Side!!!");
        }

        if (CanIMoveToUnderneath == false)
        {
           // Debug.Log("Contact Undearneath!!!");
        }


    }
}
