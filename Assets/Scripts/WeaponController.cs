using Unity.Mathematics;
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
        const double GyroToAngle = 17.5f * 360 / System.Math.PI;

        // Delta time from the last event
        var dt = ctx.time - ctx.control.device.lastUpdateTime;
        dt = System.Math.Min(dt, 1.0 / 60); // Discarding large deltas

        return Quaternion.Euler(gyro * (float)(GyroToAngle * dt));
    }

    #endregion

    #region Private members
    [SerializeField] private PlayerInput playerInput;
   // [SerializeField] private GameObject GunModel;
    [SerializeField] private GameObject GunParent;
    [SerializeField] private float maxAngle = 334;
    [SerializeField] private float minAngle = 4;
    private int playerNum;
    private Quaternion controllerRotation;
    [SerializeField] private Weapon[] weapons;
    private Weapon currentWeapon;
    private int currentWeaponIndex;
    private Vector2 stickMovement = Vector2.zero;
    private int schemenum = 0;
    private Quaternion totalStickRotation = Quaternion.identity;

    // Accumulation of gyro input
    Quaternion _accGyro = Quaternion.identity;

    #endregion

    #region MonoBehaviour implementation
    private void Awake()
    {

        currentWeaponIndex = 0;
        currentWeapon = weapons[currentWeaponIndex];
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == currentWeaponIndex)
            {
                weapons[i].MakeActive();
            }
            else
            {
                weapons[i].PutAway();
            }
        }
    }

    void Start()
    {
        playerNum = playerInput.playerIndex;
        if (playerInput.currentControlScheme == "PS4")
        { 
            // DS4 input layout extension
            InputSystem.RegisterLayoutOverride(LayoutJson);

            // Gyroscope input callback
            /**
            var action = new InputAction(binding: "<Gamepad>/gyro");
            action.performed += ctx => _accGyro *= this.GyroInputToRotation(ctx, this.playerNum);
            action.Enable();
            */
            //var action = new InputAction(binding: "<Gamepad>/gyro");
            playerInput.currentActionMap.Disable();
            if (playerInput.currentActionMap.FindAction("gyro" + playerNum) == null)
            {
                playerInput.currentActionMap.AddAction("gyro" + playerNum, InputActionType.Value, "<Gamepad>/gyro");


            }
            playerInput.currentActionMap.FindAction("gyro" + playerNum).performed += ctx => _accGyro *= this.GyroInputToRotation(ctx);
            playerInput.currentActionMap.Enable();

        }
       
        controllerRotation = transform.localRotation;

    }

    void Update()
    {
        //Vector3 rotate = _accGyro.eulerAngles;
        //Debug.LogError(_accGyro);
        // Current status
        var rot = controllerRotation; // use transform.localRotation to not preserve controller rotation past bounds

        // Rotation from gyroscope
        if (schemenum == 0) //ps4 with bluetooth
        {
            _accGyro.x = _accGyro.y; // this is good
            _accGyro.y = -_accGyro.z; //This is good
            _accGyro.z = 0; // 
        } else if (schemenum == 1)
        {

        }

        Vector3 newRotation = rot.eulerAngles;
        newRotation.x += _accGyro.eulerAngles.x;
        newRotation.y += _accGyro.eulerAngles.y;
        rot = Quaternion.Euler(newRotation);
        _accGyro = Quaternion.identity;

        // Accelerometer input
        /** not using this
        var accel = playerInput.devices[0]?.GetChildControl<Vector3Control>("accel");
        var gravity = accel?.ReadValue() ?? -Vector3.up;

        // Drift compensation using gravitational acceleration
        var comp = Quaternion.FromToRotation(rot * gravity, -Vector3.up);

        // Compensation reduction
        comp.w *= 0.1f / Time.deltaTime;
        comp = comp.normalized;
        */
        // Update
        Vector3 stickRotVec3 = totalStickRotation.eulerAngles;
        stickRotVec3.x += -50 * stickMovement.y * Time.deltaTime;
        stickRotVec3.y += 50 * stickMovement.x * Time.deltaTime;
        totalStickRotation = Quaternion.Euler(stickRotVec3);
        controllerRotation = rot;
        transform.localRotation = Quaternion.Euler(stickRotVec3 + newRotation);
        if (transform.localEulerAngles.x < maxAngle && transform.localEulerAngles.x > minAngle)
        {
            if (Mathf.Abs(transform.localEulerAngles.x - maxAngle) < Mathf.Abs(transform.localEulerAngles.x - minAngle)) 
            {
                transform.localRotation = Quaternion.Euler(maxAngle, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
               // totalControllerRotation.x = 0;
            }
            else
            {
                transform.localRotation = Quaternion.Euler(minAngle, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
                //  totalControllerRotation.x = 0;
            }
        }
        Vector3 gunParentTransformnz = (new Vector3(GunParent.transform.forward.x, 0, GunParent.transform.forward.z)).normalized;
        Vector3 thisTransformnz = (new Vector3(transform.forward.x, 0, transform.forward.z)).normalized;
        //Debug.LogError(Vector3.Angle(gunParentTransformnz, thisTransformnz));
        if (Vector3.Angle(gunParentTransformnz, thisTransformnz) > 25)
        {
            GunParent.transform.forward = Vector3.RotateTowards(gunParentTransformnz, thisTransformnz, Mathf.PI *Time.deltaTime * Mathf.Clamp(Vector3.Angle(gunParentTransformnz, thisTransformnz) / 50,1.3f, 3.5f)/ 6, 0);
        }
        Vector3 aimingTransform = this.transform.position + this.transform.forward * 15;
        Vector3 newModelForward = (aimingTransform - currentWeapon.GetModelTransform().position).normalized;
        currentWeapon.GetModelTransform().forward = newModelForward;
        
    }

    #endregion

    public void ResetView(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //transform.localRotation = Quaternion.identity; use this to not preserve controller rotation past boundaries
            controllerRotation = Quaternion.identity;
            totalStickRotation = Quaternion.identity;
        }
    }

    public void SwapWeapon(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log(ctx.ReadValue<float>());
            if (ctx.ReadValue<float>() < 0)
            {
                currentWeaponIndex = math.abs((currentWeaponIndex - 1) % 2);
            }
            else if (ctx.ReadValue<float>() > 0)
            {
                currentWeaponIndex = (currentWeaponIndex + 1) % 2;
            }

            if (ctx.ReadValue<float>() != 0)
            {
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (i == currentWeaponIndex)
                    {
                        weapons[i].MakeActive();
                        currentWeapon = weapons[i];
                    }
                    else
                    {
                        weapons[i].PutAway();
                    }
                }
            }

        }
 
    }

    public void FireWeapon(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            currentWeapon.AttackDown();
        }
        else
        {
            currentWeapon.AttackRelease();
        }
    }

    public void MoveStick(InputAction.CallbackContext ctx)
    {
        stickMovement = ctx.ReadValue<Vector2>();
    }

    public void ChangeScheme(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            schemenum = (schemenum + 1) % 2;

        }
    }

    public void Reload(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            currentWeapon.Reload();

        }
    }
}
