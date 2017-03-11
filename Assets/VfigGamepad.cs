using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfigGamepad : MonoBehaviour
{
    public enum Assignment {
        Gamepad1 = 0,
        Gamepad2 = 1,
        Gamepad3 = 2,
        Gamepad4 = 3,
    }

    public Assignment _gamepadAssignment;

    public float GetAxis(VfigInput.Axis axis)
    {
        return VfigInput.GetAxis((int)_gamepadAssignment, axis);
    }

    public bool GetButton(VfigInput.Button button)
    {
        return VfigInput.GetButton((int)_gamepadAssignment, button);
    }

    public bool GetButtonDown(VfigInput.Button button)
    {
        return VfigInput.GetButtonDown((int)_gamepadAssignment, button);
    }

    public bool GetButtonUp(VfigInput.Button button)
    {
        return VfigInput.GetButtonUp((int)_gamepadAssignment, button);
    }
}
