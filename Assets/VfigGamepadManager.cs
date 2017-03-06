using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfigGamepadManager : MonoBehaviour
{
    private List<VfigGamepad> _registeredGamepads;
    private Dictionary<int, List<VfigGamepad>> _joystickIndexAssignments;
    private string[] _joystickNames;
    private float _lastPollTime;
    private const float _pollInterval = 1.0f;
    private float _lastUpdateTime;

    private ActuatorState[,] _previousState;
    private ActuatorState[,] _actuatorState;

    private static VfigGamepadManager _Instance;

    public static VfigGamepadManager Instance
    {
        get {
            if (_Instance == null) {
                _Instance = _CreateInstance();
            }
            return _Instance;
        }
    }

    private static VfigGamepadManager _CreateInstance()
    {
        GameObject go = new GameObject(".VfigGamepad.VfigGamepadManager");
        return (VfigGamepadManager)go.AddComponent<VfigGamepadManager>();
    }

    public VfigGamepadManager()
    {
        _joystickNames = new string[0];
        _lastPollTime = -Mathf.Infinity;
        _registeredGamepads = new List<VfigGamepad>();
        _joystickIndexAssignments = new Dictionary<int, List<VfigGamepad>>();
        _previousState = new ActuatorState[MaxJoysticks, ActuatorCount];
        _actuatorState = new ActuatorState[MaxJoysticks, ActuatorCount];
    }

    public void Register(VfigGamepad gamepad)
    {
        if (!_registeredGamepads.Contains(gamepad)) {
            _registeredGamepads.Add(gamepad);
        }
    }

    public void Unregister(VfigGamepad gamepad)
    {
        if (!_registeredGamepads.Contains(gamepad)) {
            _registeredGamepads.Remove(gamepad);
        }
    }

    public void Update()
    {
        // Check if it's about time to poll for joysticks
        float now = Time.unscaledTime;
        if (now - _lastPollTime > _pollInterval) {
            _lastPollTime = now;
            PollForJoystickChanges();
        }

        for (int j = 0; j < MaxJoysticks; ++j) {
            for (int a = 0; a < ActuatorCount; ++a) {
                _previousState[j,a] = _actuatorState[j,a];
                _actuatorState[j,a] = NextActuatorState(Actuators[a], j, _previousState[j,a]);
            }
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Buttons and Axes

    public float GetAxis(int joystickIndex, VfigGamepad.Axis axis)
    {
        int axisIndex = AxisActuatorIndexes[(int)axis];
        Debug.Log("Axis: " + axis + " (" + (int)axis + ") index: " + axisIndex);
        if (joystickIndex >= 0 && axisIndex >= 0) {
            return _actuatorState[joystickIndex, axisIndex].axis;
        } else {
            return 0f;
        }
    }

    public bool GetButton(int joystickIndex, VfigGamepad.Button button)
    {
        int buttonIndex = ButtonActuatorIndexes[(int)button];
        if (joystickIndex >= 0 && buttonIndex >= 0) {
            return _actuatorState[joystickIndex, buttonIndex].button;
        } else {
            return false;
        }
    }

    public bool GetButtonDown(int joystickIndex, VfigGamepad.Button button)
    {
        int buttonIndex = ButtonActuatorIndexes[(int)button];
        if (joystickIndex >= 0 && buttonIndex >= 0) {
            return _actuatorState[joystickIndex, buttonIndex].buttonDown;
        } else {
            return false;
        }
    }

    public bool GetButtonUp(int joystickIndex, VfigGamepad.Button button)
    {
        int buttonIndex = ButtonActuatorIndexes[(int)button];
        if (joystickIndex >= 0 && buttonIndex >= 0) {
            return _actuatorState[joystickIndex, buttonIndex].buttonUp;
        } else {
            return false;
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
            Debug.Log("New joysticks:");
            for (int i = 0 ; i < newNames.Length; ++i) {
                Debug.Log("  Joystick " + i + ": " + newNames[i]);
            }
            _joystickNames = new string[newNames.Length];
            newNames.CopyTo(_joystickNames, 0);
            UpdateJoystickAssignments(newNames.Length);
        }
    }

    private void UpdateJoystickAssignments(int joystickCount)
    {
        Dictionary<int, List<VfigGamepad>> assignments = new Dictionary<int, List<VfigGamepad>>();

        // Create a list of joystick indices available for automatic assignment
        List<int> unassignedJoystickIndexes = new List<int>();
        for (int i = 0; i < joystickCount; ++i) {
            unassignedJoystickIndexes.Add(i);
        }

        // Assign joystick indexes to controllers
        int gamepadCount = _registeredGamepads.Count;
        for (int i = 0; i < gamepadCount; ++i) {
            VfigGamepad gamepad = _registeredGamepads[i];
            int joystickIndex = -1;
            if (gamepad._gamepadAssignment == VfigGamepad.Assignment.Automatic
                && unassignedJoystickIndexes.Count > 0) {
                joystickIndex = unassignedJoystickIndexes[0];
                unassignedJoystickIndexes.RemoveAt(0);
            } else {
                joystickIndex = (int)gamepad._gamepadAssignment - 1;
                if (joystickIndex >= joystickCount) {
                    joystickIndex = -1;
                }
            }
            if (joystickIndex >= 0) {
                if (!assignments.ContainsKey(joystickIndex)) {
                    assignments[joystickIndex] = new List<VfigGamepad>();
                }
                assignments[joystickIndex].Add(gamepad);
                Debug.Log("Gamepad " + gamepad + " (" + gamepad._gamepadAssignment + ") assigned to index " + joystickIndex);

            }
            gamepad._joystickIndex = joystickIndex;
        }

        // FIXME
        // // Notify gamepads
        // for (int i = 0; i < gamepadsConnected.Length; ++i) {
        //     Gamepad gamepad = gamepads[i];
        //     if (!gamepadsDisconnected.Contains(gamepad)) {
        //         if (gamepad.OnConnected) {
        //             gamepad.OnConnected(gamepad);
        //         }
        //     }
        // }
        // for (int i = 0; i < gamepadsDisconnected.Length; ++i) {
        //     Gamepad gamepad = gamepads[i];
        //     if (!gamepadsConnected.Contains(gamepad)) {
        //         if (gamepad.OnDisconnected) {
        //             gamepad.OnDisconnected(gamepad);
        //         }
        //     }
        // }

        _joystickIndexAssignments = assignments;
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
        public readonly VfigGamepad.Axis outputAxis;
        public readonly VfigGamepad.Button outputButton;
        public readonly float outputMin;
        public readonly float outputMax;
        public readonly float outputDeadZone;

        public Actuator(bool inputIsAxis, int inputButton, int inputAxis, float inputMin, float inputMax,
            bool inputFalseZero, VfigGamepad.Axis outputAxis, VfigGamepad.Button outputButton,
            float outputMin, float outputMax, float outputDeadZone)
        {
            this.inputIsAxis = inputIsAxis;
            this.inputButton = inputButton;
            this.inputAxis = inputAxis;
            this.inputMin = inputMin;
            this.inputMax = inputMax;
            this.inputFalseZero = inputFalseZero;
            this.outputAxis = outputAxis;
            this.outputButton = outputButton;
            this.outputMin = outputMin;
            this.outputMax = outputMax;
            this.outputDeadZone = outputDeadZone;
        }
    }

    private const int MaxJoysticks = 8;
    private const int MaxInputAxes = 10;
    private const int MaxInputButtons = 20;
    private const int MaxOutputAxes = 6;
    private const int MaxOutputButtons = 15;
    private static readonly string[,] InputAxisNames;
    private static readonly string[,] InputButtonKeyNames;
    private const int ActuatorCount = MaxOutputButtons + MaxOutputAxes;
    private static readonly int[] ButtonActuatorIndexes;
    private static readonly int[] AxisActuatorIndexes;

    static VfigGamepadManager()
    {
        InputAxisNames = new string[MaxJoysticks, MaxInputAxes];
        InputButtonKeyNames = new string[MaxJoysticks, MaxInputButtons];
        for (int joystick = 0; joystick < MaxJoysticks; ++joystick) {
            for (int axis = 0; axis < MaxInputAxes; ++axis) {
                InputAxisNames[joystick, axis] =
                    string.Format("VfigGamepad.Joystick{0}Axis{1}", joystick + 1, axis + 1);
            }
            for (int button = 0; button < MaxInputButtons; ++button) {
                InputButtonKeyNames[joystick, button] =
                    string.Format("joystick {0} button {1}", joystick + 1, button);
            }
        }

        ButtonActuatorIndexes = new int[MaxOutputButtons];
        AxisActuatorIndexes = new int[MaxOutputAxes];
        for (int i = 0; i < MaxOutputButtons; ++i) {
            ButtonActuatorIndexes[i] = -1;
        }
        for (int i = 0; i < MaxOutputAxes; ++i) {
            AxisActuatorIndexes[i] = -1;
        }
        for (int a = 0; a < ActuatorCount; ++a) {
            Actuator actuator = Actuators[a];
            if (actuator.outputButton != VfigGamepad.Button.Unassigned) {
                ButtonActuatorIndexes[(int)actuator.outputButton] = a;
            }
            if (actuator.outputAxis != VfigGamepad.Axis.Unassigned) {
                AxisActuatorIndexes[(int)actuator.outputAxis] = a;
            }
        }
    }

// FIXME
// so, next steps: make a (different?) monobehaviour for storing all gamepad states, updating them once per frame,
// and every second or so checking for new joysticks. Then this monobehaviour just provides convenient access to it?



    private static readonly Actuator[] Actuators = new Actuator[ActuatorCount] {
        /* Xbox 360 controller mappings for each platform. This shit is crazy yo. */
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        new Actuator( false, 16,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.A,              0f, 1f,    0f ),
        new Actuator( false, 17,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.B,              0f, 1f,    0f ),
        new Actuator( false, 18,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.X,              0f, 1f,    0f ),
        new Actuator( false, 19,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Y,              0f, 1f,    0f ),
        new Actuator( false,  7,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadLeft,       0f, 1f,    0f ),
        new Actuator( false,  8,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadRight,      0f, 1f,    0f ),
        new Actuator( false,  5,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadUp,         0f, 1f,    0f ),
        new Actuator( false,  6,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadDown,       0f, 1f,    0f ),
        new Actuator( false, 13,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.LeftShoulder,   0f, 1f,    0f ),
        new Actuator( false, 14,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.RightShoulder,  0f, 1f,    0f ),
        new Actuator( false, 11,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.LeftStick,      0f, 1f,    0f ),
        new Actuator( false, 12,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.RightStick,     0f, 1f,    0f ),
        new Actuator( false,  9,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Start,          0f, 1f,    0f ),
        new Actuator( false, 10,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Back,           0f, 1f,    0f ),
        new Actuator( false, 15,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Center,         0f, 1f,    0f ),
        new Actuator( true,   0,  0, -1f,  1f, false, VfigGamepad.Axis.LeftStickX,   VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  1, -1f,  1f, false, VfigGamepad.Axis.LeftStickY,   VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  2, -1f,  1f, false, VfigGamepad.Axis.RightStickX,  VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  3, -1f,  1f, false, VfigGamepad.Axis.RightStickY,  VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  4, -1f,  1f, true,  VfigGamepad.Axis.LeftTrigger,  VfigGamepad.Button.Unassigned,     0f, 1f,    0f ),
        new Actuator( true,   0,  5, -1f,  1f, true,  VfigGamepad.Axis.RightTrigger, VfigGamepad.Button.Unassigned,     0f, 1f,    0f ),
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        new Actuator( false,  0,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.A,              0f, 1f,    0f ),
        new Actuator( false,  1,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.B,              0f, 1f,    0f ),
        new Actuator( false,  2,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.X,              0f, 1f,    0f ),
        new Actuator( false,  3,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Y,              0f, 1f,    0f ),
        new Actuator( true,   0,  5,  0f, -1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadLeft,       0f, 1f,    0f ),
        new Actuator( true,   0,  5,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadRight,      0f, 1f,    0f ),
        new Actuator( true,   0,  6,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadUp,         0f, 1f,    0f ),
        new Actuator( true,   0,  6,  0f, -1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadDown,       0f, 1f,    0f ),
        new Actuator( false,  4,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.LeftShoulder,   0f, 1f,    0f ),
        new Actuator( false,  5,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.RightShoulder,  0f, 1f,    0f ),
        new Actuator( false,  8,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.LeftStick,      0f, 1f,    0f ),
        new Actuator( false,  9,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.RightStick,     0f, 1f,    0f ),
        new Actuator( false,  7,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Start,          0f, 1f,    0f ),
        new Actuator( false,  6,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Back,           0f, 1f,    0f ),
        new Actuator( false, 14,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Center,         0f, 1f,    0f ),
        new Actuator( true,   0,  0, -1f,  1f, false, VfigGamepad.Axis.LeftStickX,   VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  1, -1f,  1f, false, VfigGamepad.Axis.LeftStickY,   VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  3, -1f,  1f, false, VfigGamepad.Axis.RightStickX,  VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  4, -1f,  1f, false, VfigGamepad.Axis.RightStickY,  VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  8,  0f,  1f, false, VfigGamepad.Axis.LeftTrigger,  VfigGamepad.Button.Unassigned,     0f, 1f,    0f ),
        new Actuator( true,   0,  9,  0f,  1f, false, VfigGamepad.Axis.RightTrigger, VfigGamepad.Button.Unassigned,     0f, 1f,    0f ),
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        new Actuator( false,  0,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.A,              0f, 1f,    0f ),
        new Actuator( false,  1,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.B,              0f, 1f,    0f ),
        new Actuator( false,  2,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.X,              0f, 1f,    0f ),
        new Actuator( false,  3,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Y,              0f, 1f,    0f ),
        new Actuator( true,   0,  6,  0f, -1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadLeft,       0f, 1f,    0f ),
        new Actuator( true,   0,  6,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadRight,      0f, 1f,    0f ),
        new Actuator( true,   0,  7,  0f, -1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadUp,         0f, 1f,    0f ),
        new Actuator( true,   0,  7,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.DPadDown,       0f, 1f,    0f ),
        new Actuator( false,  4,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.LeftShoulder,   0f, 1f,    0f ),
        new Actuator( false,  5,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.RightShoulder,  0f, 1f,    0f ),
        new Actuator( false,  8,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.LeftStick,      0f, 1f,    0f ),
        new Actuator( false,  9,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.RightStick,     0f, 1f,    0f ),
        new Actuator( false,  7,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Start,          0f, 1f,    0f ),
        new Actuator( false,  6,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Back,           0f, 1f,    0f ),
        new Actuator( false, 14,  0,  0f,  1f, false, VfigGamepad.Axis.Unassigned,   VfigGamepad.Button.Center,         0f, 1f,    0f ),
        new Actuator( true,   0,  0, -1f,  1f, false, VfigGamepad.Axis.LeftStickX,   VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  1,  1f, -1f, false, VfigGamepad.Axis.LeftStickY,   VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  3, -1f,  1f, false, VfigGamepad.Axis.RightStickX,  VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  4,  1f, -1f, false, VfigGamepad.Axis.RightStickY,  VfigGamepad.Button.Unassigned,    -1f, 1f, 0.15f ),
        new Actuator( true,   0,  2,  0f,  1f, false, VfigGamepad.Axis.LeftTrigger,  VfigGamepad.Button.Unassigned,     0f, 1f,    0f ),
        new Actuator( true,   0,  5,  0f,  1f, false, VfigGamepad.Axis.RightTrigger, VfigGamepad.Button.Unassigned,     0f, 1f,    0f ),
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

    private static ActuatorState NextActuatorState(Actuator actuator, int joystick, ActuatorState oldState)
    {
        ActuatorState newState = oldState;

        float inputValue;
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
            inputDown = false;
            inputUp = false;
        } else {
            string keyName = InputButtonKeyNames[joystick, actuator.inputButton];
            bool rawValue = Input.GetKey(keyName);
            inputValue = rawValue ? actuator.inputMax : actuator.inputMin;
            inputDown = Input.GetKeyDown(keyName);
            inputUp = Input.GetKeyUp(keyName);
        }

        if (actuator.outputAxis != VfigGamepad.Axis.Unassigned) {
            // FIXME - handle dead zone
            // return Mathf.InverseLerp(config.deadZone, 1,  Mathf.Abs(value)) * Mathf.Sign(value);
            newState.axis = Mathf.Lerp(actuator.outputMin, actuator.outputMax, inputValue);
        }

        if (actuator.outputButton != VfigGamepad.Button.Unassigned) {
            bool outputValue = Mathf.Approximately(inputValue, 1f);
            newState.button = outputValue;
            newState.buttonDown = inputDown;
            newState.buttonUp = inputUp;
        }

        return newState;
    }
}
