using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

sealed class WeaponController : MonoBehaviour
{
    #region Static class members
    

    // Layout extension written in JSON
    const string LayoutJson = @"{
      ""name"": ""DualShock4GamepadHIDCustom"",
      ""extend"": ""DualShock4GamepadHID"",
      ""controls"": [
        {""name"":""gyro"", ""format"":""VC3S"", ""offset"":13,
         ""layout"":""Vector3"", ""processors"":""ScaleVector3(x=-1,y=-1,z=1)""},
        {""name"":""gyro/x"", ""format"":""SHRT"", ""offset"":0 },
        {""name"":""gyro/y"", ""format"":""SHRT"", ""offset"":2 },
        {""name"":""gyro/z"", ""format"":""SHRT"", ""offset"":4 },
        {""name"":""accel"", ""format"":""VC3S"", ""offset"":19,
         ""layout"":""Vector3"", ""processors"":""ScaleVector3(x=-1,y=-1,z=1)""},
        {""name"":""accel/x"", ""format"":""SHRT"", ""offset"":0 },
        {""name"":""accel/y"", ""format"":""SHRT"", ""offset"":2 },
        {""name"":""accel/z"", ""format"":""SHRT"", ""offset"":4 }
      ]}";

    // Gyro vector data to rotation conversion
    private Quaternion GyroInputToRotation(in InputAction.CallbackContext ctx)
    {

        // Gyro input data
        var gyro = ctx.ReadValue<Vector3>();

        // Coefficient converting a gyro data value into a degree
        // Note: The actual constant is undocumented and unknown.
        //       I just put a plasible value by guessing.
        const double GyroToAngle = 16 * 360 / System.Math.PI;

        // Delta time from the last event
        var dt = ctx.time - ctx.control.device.lastUpdateTime;
        dt = System.Math.Min(dt, 1.0 / 60); // Discarding large deltas

        return Quaternion.Euler(gyro * (float)(GyroToAngle * dt));
    }

    #endregion

    #region Private members
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject GunModel;
    [SerializeField] private GameObject GunParent;
    private int playerNum;

    // Accumulation of gyro input
    Quaternion _accGyro = Quaternion.identity;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // DS4 input layout extension
        InputSystem.RegisterLayoutOverride(LayoutJson);
        playerNum = playerInput.playerIndex;

        // Gyroscope input callback
        /**
        var action = new InputAction(binding: "<Gamepad>/gyro");
        action.performed += ctx => _accGyro *= this.GyroInputToRotation(ctx, this.playerNum);
        action.Enable();
        */
        //var action = new InputAction(binding: "<Gamepad>/gyro");
        playerInput.currentActionMap.Disable();
        playerInput.currentActionMap.AddAction("gyro" + playerNum, InputActionType.Value, "<Gamepad>/gyro");
        playerInput.currentActionMap.FindAction("gyro" + playerNum).performed += ctx => _accGyro *= this.GyroInputToRotation(ctx);
        playerInput.currentActionMap.Enable();

    }

    void Update()
    {
        Vector3 rotate = _accGyro.eulerAngles;
        //Debug.LogError(_accGyro);
        // Current status
        var rot = transform.localRotation;

        // Rotation from gyroscope
        _accGyro.x = _accGyro.y; // this is good
        _accGyro.y = -_accGyro.z; //This is good
        _accGyro.z = 0; // 
        rot *= _accGyro;
        _accGyro = Quaternion.identity;

        // Accelerometer input
        var accel = playerInput.devices[0]?.GetChildControl<Vector3Control>("accel");
        var gravity = accel?.ReadValue() ?? -Vector3.up;

        // Drift compensation using gravitational acceleration
        var comp = Quaternion.FromToRotation(rot * gravity, -Vector3.up);

        // Compensation reduction
        comp.w *= 0.1f / Time.deltaTime;
        comp = comp.normalized;

        // Update

        transform.localRotation = rot;
        if (transform.localEulerAngles.x < 334 && transform.localEulerAngles.x > 4)
        {
            if (Mathf.Abs(transform.localEulerAngles.x - 334) < Mathf.Abs(transform.localEulerAngles.x - 4)) 
            {
                transform.localRotation = Quaternion.Euler(334, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
            } else
            {
                transform.localRotation = Quaternion.Euler(4, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
            }
        }
        Vector3 gunParentTransformnz = (new Vector3(GunParent.transform.forward.x, 0, GunParent.transform.forward.z)).normalized;
        Vector3 thisTransformnz = (new Vector3(transform.forward.x, 0, transform.forward.z)).normalized;
        //Debug.LogError(Vector3.Angle(gunParentTransformnz, thisTransformnz));
        if (Vector3.Angle(gunParentTransformnz, thisTransformnz) > 25)
        {
            GunParent.transform.forward = Vector3.RotateTowards(gunParentTransformnz, thisTransformnz, Mathf.PI *Time.deltaTime / 6, 0);
        }
        Vector3 aimingTransform = this.transform.position + this.transform.forward * 20;
        Vector3 newModelForward = (aimingTransform - GunModel.transform.position).normalized;
        GunModel.transform.forward = newModelForward;
        
    }

    #endregion

    public void ResetView(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Center Pressed");
            transform.localRotation = Quaternion.identity;

        }
    }
}
