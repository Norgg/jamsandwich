using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfigGamepad : MonoBehaviour
{
    public enum Assignment {
        Automatic = 0,
        Gamepad1 = 1,
        Gamepad2 = 2,
        Gamepad3 = 3,
        Gamepad4 = 4,
    }

    public enum Axis {
        Unassigned = -1,
        LeftStickX = 0,
        LeftStickY = 1,
        RightStickX = 2,
        RightStickY = 3,
        LeftTrigger = 4,
        RightTrigger = 5,
    }

    public enum Button {
        Unassigned = -1,
        A = 0,
        B = 1,
        X = 2,
        Y = 3,
        DPadLeft = 4,
        DPadRight = 5,
        DPadUp = 6,
        DPadDown = 7,
        LeftShoulder = 8,
        RightShoulder = 9,
        LeftStick = 10,
        RightStick = 11,
        Start = 12,
        Back = 13,
        Center = 14,
    }


    // FIXME - need to monitor Input.GetJoystickNames() to see if devices chnaged
    // FIXME - because devices get renumbered when they change, we need to 'disabled' each of them and wait for
    //         them to be 'enabled' again with the Xbox button I guess?

    // public static string[] GetJoystickNames();
    // The position of a joystick in this array corresponds to the joystick number, i.e. the name
    // in position 0 of this array is for the joystick that feeds data into 'Joystick 1' in the Input
    // Manager, the name in position 1 corresponds to 'Joystick 2', and so on. Note that some entries
    // in the array may be blank if no device is connected for that joystick number.

    public Assignment _gamepadAssignment;
    [NonSerialized] public int _joystickIndex;

    // FIXME
    // public event System.Action<VfigGamepad> OnConnected;
    // public event System.Action<VfigGamepad> OnDisconnected;

    public VfigGamepad()
    {
    }

    public void OnEnable()
    {
        VfigGamepadManager.Instance.Register(this);
    }

    public void OnDisable()
    {
        VfigGamepadManager.Instance.Unregister(this);
    }

    public float GetAxis(Axis axis)
    {
        return VfigGamepadManager.Instance.GetAxis(_joystickIndex, axis);
    }

    public bool GetButton(Button button)
    {
        return VfigGamepadManager.Instance.GetButton(_joystickIndex, button);
    }

    public bool GetButtonDown(Button button)
    {
        return VfigGamepadManager.Instance.GetButtonDown(_joystickIndex, button);
    }

    public bool GetButtonUp(Button button)
    {
        return VfigGamepadManager.Instance.GetButtonUp(_joystickIndex, button);
    }
}
