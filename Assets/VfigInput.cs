using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfigInput : MonoBehaviour
{

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Buttons and Axes

    public enum Axis {
        LeftStickX = 0,
        LeftStickY = 1,
        RightStickX = 2,
        RightStickY = 3,
        LeftTrigger = 4,
        RightTrigger = 5,
    }
    public const int AxisCount = 6;

    public enum Button {
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
    public const int ButtonCount = 15;

    public static bool GetButtonDown(int gamepad, Button button)
    {
        return VfigInput.Instance._buttonActuatorStates[VfigInput.Instance.JoystickIndex(gamepad), (int)button].buttonDown;
    }

    public static bool GetButtonUp(int gamepad, Button button)
    {
        return VfigInput.Instance._buttonActuatorStates[VfigInput.Instance.JoystickIndex(gamepad), (int)button].buttonUp;
    }

    public static bool GetButton(int gamepad, Button button)
    {
        return VfigInput.Instance._buttonActuatorStates[VfigInput.Instance.JoystickIndex(gamepad), (int)button].button;
    }

    public static float GetAxis(int gamepad, Axis axis)
    {
        return VfigInput.Instance._axisActuatorStates[VfigInput.Instance.JoystickIndex(gamepad), (int)axis].axis;
    }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Input state

    private string[] _joystickNames;
    private float _lastPollTime;
    private const float _pollInterval = 1.0f;

    private ActuatorState[/* joystick */, /* axis */] _axisActuatorStates;
    private ActuatorState[/* joystick */, /* button */] _buttonActuatorStates;

    private static VfigInput _Instance;

    public static VfigInput Instance
    {
        get {
            if (_Instance == null) {
                _Instance = _CreateInstance();
            }
            return _Instance;
        }
    }

    private static VfigInput _CreateInstance()
    {
        GameObject go = new GameObject("VfigInput");
        return (VfigInput)go.AddComponent<VfigInput>();
    }

    public VfigInput()
    {
        _joystickNames = new string[0];
        _lastPollTime = -Mathf.Infinity;

        _axisActuatorStates = new ActuatorState[MaxJoysticks, AxisCount];
        _buttonActuatorStates = new ActuatorState[MaxJoysticks, ButtonCount];
    }

    public void Update()
    {
        // Check if it's about time to poll for joysticks
        float now = Time.unscaledTime;
        if (now - _lastPollTime > _pollInterval) {
            _lastPollTime = now;
            PollForJoystickChanges();
        }

        UpdateActuatorStates();
    }

    private void UpdateActuatorStates()
    {
        for (int joystick = 0; joystick < MaxJoysticks; ++joystick) {
            for (int axis = 0; axis < AxisCount; ++axis) {
                _axisActuatorStates[joystick, axis] = GetActuatorState(joystick,
                    AxisActuators[axis], _axisActuatorStates[joystick, axis]);
            }
            for (int button = 0; button < ButtonCount; ++button) {
                _buttonActuatorStates[joystick, button] = GetActuatorState(joystick,
                    ButtonActuators[button], _buttonActuatorStates[joystick, button]);
            }
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Joystick connections

    private void PollForJoystickChanges()
    {
        string[] oldNames = _joystickNames;
        string[] newNames = Input.GetJoystickNames();
        bool isEqual = true;
        if (oldNames.Length != newNames.Length) {
            isEqual = false;
        } else {
            for (int i = 0 ; i < newNames.Length; ++i) {
                if (oldNames[i] != newNames[i]) {
                    isEqual = false;
                    break;
                }
            }
        }
        if (!isEqual) {
            _joystickNames = new string[newNames.Length];
            newNames.CopyTo(_joystickNames, 0);
            // An improvement would be to monitor which joysticks are connected
            // and reevaluate the joystick-to-gamepad lookup here.
        }
    }

    private int JoystickIndex(int gamepadIndex)
    {
        // An improvement would be to try to identify which joysticks look like
        // gamepads, and which mapping to use.
        return gamepadIndex;
    }


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Joysticks

    private const int MaxJoysticks = 8;
    private const int MaxInputAxes = 10;
    private const int MaxInputButtons = 20;
    private static readonly string[,] InputAxisNames;
    private static readonly string[,] InputButtonKeyNames;

    static VfigInput()
    {
        InputAxisNames = new string[MaxJoysticks, MaxInputAxes];
        InputButtonKeyNames = new string[MaxJoysticks, MaxInputButtons];
        for (int joystick = 0; joystick < MaxJoysticks; ++joystick) {
            for (int axis = 0; axis < MaxInputAxes; ++axis) {
                InputAxisNames[joystick, axis] =
                    string.Format("VfigInput.Joystick{0}Axis{1}", joystick + 1, axis + 1);
            }
            for (int button = 0; button < MaxInputButtons; ++button) {
                InputButtonKeyNames[joystick, button] =
                    string.Format("joystick {0} button {1}", joystick + 1, button);
            }
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Actuators

    private class Actuator {
        public readonly bool inputIsAxis;
        public readonly int inputButton;
        public readonly int inputAxis;
        public readonly float inputMin;
        public readonly float inputMax;
        public readonly bool inputFalseZero;
        public readonly float outputMin;
        public readonly float outputMax;
        public readonly float outputDeadZone;

        public Actuator(bool inputIsAxis, int inputButton, int inputAxis, float inputMin, float inputMax,
            bool inputFalseZero, float outputMin, float outputMax, float outputDeadZone)
        {
            this.inputIsAxis = inputIsAxis;
            this.inputButton = inputButton;
            this.inputAxis = inputAxis;
            this.inputMin = inputMin;
            this.inputMax = inputMax;
            this.inputFalseZero = inputFalseZero;
            this.outputMin = outputMin;
            this.outputMax = outputMax;
            this.outputDeadZone = outputDeadZone;
        }
    }

    /* Xbox 360 controller mappings for each platform. */

    private static readonly Actuator[] AxisActuators = new Actuator[AxisCount] {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        new Actuator( true,   0,  0, -1f,  1f, false, -1f, 1f, 0.15f ), // LeftStickX
        new Actuator( true,   0,  1, -1f,  1f, false, -1f, 1f, 0.15f ), // LeftStickY
        new Actuator( true,   0,  2, -1f,  1f, false, -1f, 1f, 0.15f ), // RightStickX
        new Actuator( true,   0,  3, -1f,  1f, false, -1f, 1f, 0.15f ), // RightStickY
        new Actuator( true,   0,  4, -1f,  1f, true,   0f, 1f,    0f ), // LeftTrigger
        new Actuator( true,   0,  5, -1f,  1f, true,   0f, 1f,    0f ), // RightTrigger
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        new Actuator( true,   0,  0, -1f,  1f, false, -1f, 1f, 0.15f ), // LeftStickX
        new Actuator( true,   0,  1, -1f,  1f, false, -1f, 1f, 0.15f ), // LeftStickY
        new Actuator( true,   0,  3, -1f,  1f, false, -1f, 1f, 0.15f ), // RightStickX
        new Actuator( true,   0,  4, -1f,  1f, false, -1f, 1f, 0.15f ), // RightStickY
        new Actuator( true,   0,  2,  0f,  1f, false,  0f, 1f,    0f ), // LeftTrigger
        new Actuator( true,   0,  2,  0f, -1f, false,  0f, 1f,    0f ), // RightTrigger
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        new Actuator( true,   0,  0, -1f,  1f, false, -1f, 1f, 0.15f ), // LeftStickX
        new Actuator( true,   0,  1,  1f, -1f, false, -1f, 1f, 0.15f ), // LeftStickY
        new Actuator( true,   0,  3, -1f,  1f, false, -1f, 1f, 0.15f ), // RightStickX
        new Actuator( true,   0,  4,  1f, -1f, false, -1f, 1f, 0.15f ), // RightStickY
        new Actuator( true,   0,  2,  0f,  1f, false,  0f, 1f,    0f ), // LeftTrigger
        new Actuator( true,   0,  5,  0f,  1f, false,  0f, 1f,    0f ), // RightTrigger
#endif
    };

    private static readonly Actuator[] ButtonActuators = new Actuator[ButtonCount] {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        new Actuator( false, 16,  0,  0f,  1f, false,  0f, 1f,    0f ), // A
        new Actuator( false, 17,  0,  0f,  1f, false,  0f, 1f,    0f ), // B
        new Actuator( false, 18,  0,  0f,  1f, false,  0f, 1f,    0f ), // X
        new Actuator( false, 19,  0,  0f,  1f, false,  0f, 1f,    0f ), // Y
        new Actuator( false,  7,  0,  0f,  1f, false,  0f, 1f,    0f ), // DPadLeft
        new Actuator( false,  8,  0,  0f,  1f, false,  0f, 1f,    0f ), // DPadRight
        new Actuator( false,  5,  0,  0f,  1f, false,  0f, 1f,    0f ), // DPadUp
        new Actuator( false,  6,  0,  0f,  1f, false,  0f, 1f,    0f ), // DPadDown
        new Actuator( false, 13,  0,  0f,  1f, false,  0f, 1f,    0f ), // LeftShoulder
        new Actuator( false, 14,  0,  0f,  1f, false,  0f, 1f,    0f ), // RightShoulder
        new Actuator( false, 11,  0,  0f,  1f, false,  0f, 1f,    0f ), // LeftStick
        new Actuator( false, 12,  0,  0f,  1f, false,  0f, 1f,    0f ), // RightStick
        new Actuator( false,  9,  0,  0f,  1f, false,  0f, 1f,    0f ), // Start
        new Actuator( false, 10,  0,  0f,  1f, false,  0f, 1f,    0f ), // Back
        new Actuator( false, 15,  0,  0f,  1f, false,  0f, 1f,    0f ), // Center
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        new Actuator( false,  0,  0,  0f,  1f, false,  0f, 1f,    0f ), // A
        new Actuator( false,  1,  0,  0f,  1f, false,  0f, 1f,    0f ), // B
        new Actuator( false,  2,  0,  0f,  1f, false,  0f, 1f,    0f ), // X
        new Actuator( false,  3,  0,  0f,  1f, false,  0f, 1f,    0f ), // Y
        new Actuator( true,   0,  5,  0f, -1f, false,  0f, 1f,    0f ), // DPadLeft
        new Actuator( true,   0,  5,  0f,  1f, false,  0f, 1f,    0f ), // DPadRight
        new Actuator( true,   0,  6,  0f,  1f, false,  0f, 1f,    0f ), // DPadUp
        new Actuator( true,   0,  6,  0f, -1f, false,  0f, 1f,    0f ), // DPadDown
        new Actuator( false,  4,  0,  0f,  1f, false,  0f, 1f,    0f ), // LeftShoulder
        new Actuator( false,  5,  0,  0f,  1f, false,  0f, 1f,    0f ), // RightShoulder
        new Actuator( false,  8,  0,  0f,  1f, false,  0f, 1f,    0f ), // LeftStick
        new Actuator( false,  9,  0,  0f,  1f, false,  0f, 1f,    0f ), // RightStick
        new Actuator( false,  7,  0,  0f,  1f, false,  0f, 1f,    0f ), // Start
        new Actuator( false,  6,  0,  0f,  1f, false,  0f, 1f,    0f ), // Back
        new Actuator( false, 14,  0,  0f,  1f, false,  0f, 1f,    0f ), // Center
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        new Actuator( false,  0,  0,  0f,  1f, false,  0f, 1f,    0f ), // A
        new Actuator( false,  1,  0,  0f,  1f, false,  0f, 1f,    0f ), // B
        new Actuator( false,  2,  0,  0f,  1f, false,  0f, 1f,    0f ), // X
        new Actuator( false,  3,  0,  0f,  1f, false,  0f, 1f,    0f ), // Y
        new Actuator( true,   0,  6,  0f, -1f, false,  0f, 1f,    0f ), // DPadLeft
        new Actuator( true,   0,  6,  0f,  1f, false,  0f, 1f,    0f ), // DPadRight
        new Actuator( true,   0,  7,  0f, -1f, false,  0f, 1f,    0f ), // DPadUp
        new Actuator( true,   0,  7,  0f,  1f, false,  0f, 1f,    0f ), // DPadDown
        new Actuator( false,  4,  0,  0f,  1f, false,  0f, 1f,    0f ), // LeftShoulder
        new Actuator( false,  5,  0,  0f,  1f, false,  0f, 1f,    0f ), // RightShoulder
        new Actuator( false,  8,  0,  0f,  1f, false,  0f, 1f,    0f ), // LeftStick
        new Actuator( false,  9,  0,  0f,  1f, false,  0f, 1f,    0f ), // RightStick
        new Actuator( false,  7,  0,  0f,  1f, false,  0f, 1f,    0f ), // Start
        new Actuator( false,  6,  0,  0f,  1f, false,  0f, 1f,    0f ), // Back
        new Actuator( false, 14,  0,  0f,  1f, false,  0f, 1f,    0f ), // Center
#endif
    };

    private struct ActuatorState
    {
        public float axis;
        public bool handledFalseZero;
        public bool button;
        public bool buttonDown;
        public bool buttonUp;
    }

    private static ActuatorState GetActuatorState(int joystick, Actuator actuator, ActuatorState oldState)
    {
        ActuatorState newState = oldState;

        float inputValue;
        bool inputButton;
        bool inputDown;
        bool inputUp;
        if (actuator.inputIsAxis) {
            string axisName = InputAxisNames[joystick, actuator.inputAxis];
            float rawValue = Input.GetAxisRaw(axisName);
            if (actuator.inputFalseZero && !newState.handledFalseZero) {
                if (Mathf.Approximately(rawValue, 0f)) {
                    rawValue = actuator.inputMin;
                } else {
                    newState.handledFalseZero = true;
                }
            }
            inputValue = Mathf.InverseLerp(actuator.inputMin, actuator.inputMax, rawValue);
            inputButton = (inputValue >= 0.75f);
            inputDown = false;
            inputUp = false;
        } else {
            string keyName = InputButtonKeyNames[joystick, actuator.inputButton];
            bool rawValue = Input.GetKey(keyName);
            inputValue = rawValue ? actuator.inputMax : actuator.inputMin;
            inputButton = rawValue;
            inputDown = Input.GetKeyDown(keyName);
            inputUp = Input.GetKeyUp(keyName);
        }

        // FIXME - handle dead zone
        // return Mathf.InverseLerp(config.deadZone, 1,  Mathf.Abs(value)) * Mathf.Sign(value);
        newState.axis = Mathf.Lerp(actuator.outputMin, actuator.outputMax, inputValue);
        newState.button = inputButton;
        newState.buttonDown = inputDown;
        newState.buttonUp = inputUp;

        return newState;
    }
}
