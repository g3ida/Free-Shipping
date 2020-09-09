using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerManager Manager;
    public float MoveInput { get; private set; }
    public float VerticalInput { get; private set; }

    public int Direction { get; private set; }
    public int VerticalDirection { get; private set; }

    struct ActionSettings
    {
        public ActionSettings(float permissiveness, float responsiveness)
        {
            this.Permissiveness = permissiveness;
            this.Responsiveness = responsiveness;
        }

        public float Permissiveness; //how much time we permit action since its conditions were met.
        public float Responsiveness; //how much do we dalay action hoping its conditions to be met.
    }


    ActionSettings JumpSettings = new ActionSettings(0.01f, 0.01f);
    ActionSettings DashSettings = new ActionSettings(0.01f, 0.01f);

    //jump stuff
    public bool JumpPressed = false;
    public bool JumpDelayed = false;
    public bool JumpReleased = false;

    public bool RotationPressed = false;
    public bool ReversedRotationPressed = false;

    //dash settings
    public int MaxAllowedDashes = 1;
    public float DashTime = 0.12f;
    public float DashVelocity = 70.0f;
    public float DashEndVelocity = 3.0f;

    public bool DashPressed = false;

    private int DashCounter = 0;

    void Start()
    {
        Direction = 1;
    }
    void Update()
    {
        if (Manager.ColliderInstance.IsGrounded)
        {
            DashCounter = 0;
        }
        MoveInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        Direction = (MoveInput > 0.7f ? 1 : (MoveInput < -0.7f ? -1 : Direction));
        //unlike the direction the vertical direction is not kept if no input is found.
        VerticalDirection = (VerticalInput > 0.7f ? 1 : (VerticalInput < -0.7f ? -1 : 0));

        update_jump();
        update_dash();
        update_rotation();
    }
    void update_rotation()
    {
        RotationPressed = false;
        if (Input.GetKeyDown(KeyCode.Z) && !Manager.ColliderInstance.IsGrounded)
        {
            RotationPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.V) && !Manager.ColliderInstance.IsGrounded)
        {
            ReversedRotationPressed = true;
        }
    }
    #region Dash
    IEnumerator set_dash_after_delay(float Count)
    {
        yield return new WaitForSeconds(Count);
        //just check once again
        if (DashCounter < MaxAllowedDashes)
        {
            DashPressed = true;
            DashCounter++;
        }
        yield return null;
    }
    void update_dash()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (DashCounter < MaxAllowedDashes)
            {
                StartCoroutine("set_dash_after_delay", DashSettings.Responsiveness);
            }
        }
    }
    #endregion
    #region Jump
    void update_jump()
    {
        //jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (can_jump())
            {
                JumpPressed = true;
            }
            else
            {
                JumpDelayed = true;
                StartCoroutine("TestJumpAfterDelay", JumpSettings.Responsiveness);
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (JumpDelayed == true)
            {
                JumpDelayed = false;
            }
            JumpPressed = false;
            JumpReleased = true;
        }
    }
    bool can_jump()
    {
        if (Manager.ColliderInstance.IsGrounded)
        {
            return true;
        }
        if (Manager.ColliderInstance.TimeSinceGrounded <= JumpSettings.Permissiveness && !JumpPressed)
        {
            return true;
        }
        return false;
    }
    IEnumerator TestJumpAfterDelay(float Count)
    {
        yield return new WaitForSeconds(Count);
        if (JumpDelayed == false)
        {
            //need to yield release
        }
        JumpDelayed = false;
        if (Manager.ColliderInstance.IsGrounded)
        {
            JumpPressed = true;
        }
        yield return null;
    }
    #endregion
}
