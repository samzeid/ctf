using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputData : MonoBehaviour
{
    public bool usingQuest;

    public InputDevice rightController;
    public InputDevice leftController;
    


    // Update is called once per frame
    void Update()
    {
        if (usingQuest)
        {
            if (!rightController.isValid || !leftController.isValid)
                InitializeControllers();

        }
    }
    
    private void InitializeControllers()
    {
        if(!rightController.isValid)
            InitializeQuestDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref rightController);
    
        if (!leftController.isValid)
            InitializeQuestDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, ref leftController);
    }

    private void InitializeQuestDevice(InputDeviceCharacteristics inputCharacteristics, ref InputDevice inputDevice)
    {
        List<InputDevice> devices = new List<InputDevice>();
    
        InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, devices);
    
        if (devices.Count > 0)
        {
            inputDevice = devices[0];
        }
    }
}
