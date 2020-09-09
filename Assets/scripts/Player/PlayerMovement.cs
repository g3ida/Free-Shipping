using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerManager Manager;
    private bool WasGrounded;
    
    //ghost effect
    public GhostEffect GhostEffect;

    private Animator PlayerAnimator;
    public ParticleSystem Dust;
    
    [SerializeField]
    CameraShake CameraShake;

    public float Velocity = 12.0f;

    public float JumpForce = 1000.0f;
    public float DragForceConstant = 20f;
    private Rigidbody2D Rb;

    //jump stuff
    private float TimeUntilFullJumpIsConsidered = 0.2f;
    private float JumpTimer = 0f;
    private bool IsJumpReleased = false;
    
    //for the rotation of the box while jumping
    private float RotationDuration = 0.15f;
    private float ThetaZero;
    private float ThetaTarget;
    private float ThetaPoint;
    private float RotationTimer = 0f;


    //dash stuff
    public float DashTime = 0.12f;
    private float DashTimer = 0.0f;
    public float DashVelocity = 70.0f;
    public float DashEndVelocity = 3.0f;
    public Vector2 DashDirection;

    SpriteRenderer SprRenderer;

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        SprRenderer = GetComponent<SpriteRenderer>();
        WasGrounded = Manager.ColliderInstance.IsGrounded;
        //we can change properties like bounciness etc...
    }
    void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        //dash event
        if (Manager.ControllerInstance.DashPressed)
        {
            CameraShake.DoShake = true;

            int hor = Manager.ControllerInstance.Direction;
            int vert = Manager.ControllerInstance.VerticalDirection;
            if (vert != 0) { hor = 0; }

            DashDirection = new Vector2(hor, vert);
            DashTimer = DashTime;
        }

        if (Manager.ControllerInstance.RotationPressed)
        {
            Manager.ControllerInstance.RotationPressed = false;
            ThetaZero = Rb.rotation;
            ThetaTarget = Mathf.Round((Rb.rotation + 90) / 90f) * 90f;
            ThetaPoint = (ThetaTarget - ThetaZero) / RotationDuration;
            RotationTimer = RotationDuration;
            Rb.constraints = RigidbodyConstraints2D.None;
        }


        if (Manager.ControllerInstance.ReversedRotationPressed)
        {
            Manager.ControllerInstance.ReversedRotationPressed = false;
            ThetaZero = Rb.rotation;
            ThetaTarget = Mathf.Round((Rb.rotation - 90) / 90f) * 90f;
            ThetaPoint = (ThetaTarget - ThetaZero) / RotationDuration;
            RotationTimer = RotationDuration;
            Rb.constraints = RigidbodyConstraints2D.None;
        }

        //rolling is pressed
        /*if (manager.player_controller.rolling_down)
        {
            rolling_timer -= Time.deltaTime;
            if (Mathf.Abs(rb.velocity.x) < 0.01f)
            {
                rolling_timer = time_to_cancel_rolling;
            }
            //torque = true;
        }
        else
        {
            if (rolling_timer > 0.01f)
            {
                rolling_cancelled = true;
            }
        }*/


        //jumping
        if (JumpTimer > 0.001f)
        {
            JumpTimer -= Time.deltaTime;
        }

        //jump has been released before full jump reached
        if (Manager.ControllerInstance.JumpReleased == true)
        {
            Manager.ControllerInstance.JumpReleased = false;
            if (JumpTimer > 0.001f)
            {
                IsJumpReleased = true;
            }
        }


    }

    void FixedUpdate()
    {

        //if just landed play the dust particle effects
        if (Manager.ColliderInstance.IsGrounded && !WasGrounded)
        {
            create_dust();
            if (Manager.ColliderInstance.CurrentFace == Manager.ColliderInstance.BottomFace ||
                Manager.ColliderInstance.CurrentFace == Manager.ColliderInstance.TopFace)
            {
                PlayerAnimator.SetInteger("ground_side", 1);
            }
            PlayerAnimator.SetBool("jump_squash", true);
        }
        else
        {
            PlayerAnimator.SetInteger("ground_side", 2);
            PlayerAnimator.SetBool("jump_squash", false);
        }

        WasGrounded = Manager.ColliderInstance.IsGrounded;



        //no rotation action is performed then do not allow rotation
        if (RotationTimer == 0)
        {
            Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        //the rotation
        if (RotationTimer > 0f)
        {
            RotationTimer -= Time.deltaTime;
            if (RotationTimer < 0f)
            {
                ThetaPoint = 0f;
                Rb.angularVelocity = 0f;
                Rb.rotation = ThetaTarget;
            }
            Rb.angularVelocity = ThetaPoint;
        }
        else
        {
            RotationTimer = 0f;
        }


        /*if (manager.player_controller.rolling_down)
        {
            //the move on ground state
            Vector3 position = transform.position;
            position.y = position.y + manager.player_collision.sprite_height * 0.3f;
            float move_input = manager.player_controller.move_input * velocity;
            rb.AddForceAtPosition(new Vector2(move_input * 5f, -Mathf.Abs(move_input)), position, ForceMode2D.Force);
            rb.velocity = new Vector2(Mathf.Max(Mathf.Min(rb.velocity.x, 10f), -10f), rb.velocity.y);
            //adjust the angular velocity given the fact that a 360 degrees rotation of the box corresponds
            //to a move of distance 4 * sprite_height
            rb.angularVelocity = -rb.velocity.x * 360 / (4 * manager.player_collision.sprite_height);
            timer_till_next_move = time_till_push * 2f;
        }
        else
        {
            //move on ground state
            timer_till_next_move += Time.deltaTime;
            if (direction != 0)
            {
                if (Mathf.Abs(rb.angularVelocity) > (rb.velocity.x * 360 / (4 * manager.player_collision.sprite_height)) * 0.3f)
                {
                    rb.angularVelocity = rb.angularVelocity * 0.5f;
                }

                if (timer_till_next_move > time_till_push)
                {
                    timer_till_next_move = 0f;
                    rb.AddForce(new Vector2(direction * velocity * 0.85f, 0), ForceMode2D.Impulse);
                }
                rb.velocity = new Vector2(Mathf.Max(Mathf.Min(rb.velocity.x, 10f), -10f), rb.velocity.y);

            }
            direction = 0f;
        }*/


        //perform jump
        if (Manager.ControllerInstance.JumpPressed)
        {
            JumpTimer = TimeUntilFullJumpIsConsidered;
            //rb.velocity = Vector2.up * jump_force + rb.velocity;
            Rb.AddForce(new Vector2(0f, JumpForce));
            Manager.ControllerInstance.JumpPressed = false;
        }

        //if no dash and on the air the follow input
        if (!Manager.ColliderInstance.IsGrounded && DashTimer < 0.01f)
        {
            Rb.velocity = new Vector2(Manager.ControllerInstance.MoveInput * Velocity, Rb.velocity.y);
        }


        //walk on the ground
        if (Manager.ColliderInstance.IsGrounded)
        {
            float rot = Mathf.Abs(Rb.rotation) % 360;
            if (Mathf.Abs(rot - 90f) < 0.1f || Mathf.Abs(rot - 270f) < 0.1f)
            {
                SprRenderer.sharedMaterial.SetFloat("_VerticalSkew", -Manager.ControllerInstance.MoveInput * 0.05f);
                Rb.velocity = new Vector2(Manager.ControllerInstance.MoveInput * Velocity, Rb.velocity.y);
            }
            else
            {
                SprRenderer.sharedMaterial.SetFloat("_HorizontalSkew", Manager.ControllerInstance.MoveInput * 0.05f);
                Rb.velocity = new Vector2(Manager.ControllerInstance.MoveInput * Velocity, Rb.velocity.y);
            }
        }
        else
        {
            SprRenderer.sharedMaterial.SetFloat("_VerticalSkew", 0);
            SprRenderer.sharedMaterial.SetFloat("_HorizontalSkew", 0);
        }


        //drag force uncomment this if you want to add some air resistance
        //Vector2 drag_force = new Vector2(rb.velocity.x - drag_force_constant * Math.Abs(rb.velocity.x) * rb.velocity.x, 0.0f);
        //rb.AddForce(drag_force, ForceMode2D.Force);




        //allow the cancelling of the jump
        if (IsJumpReleased && Rb.velocity.y > 0)
        {
            //Debug.Log("here");
            Rb.velocity = new Vector2(Rb.velocity.x, Rb.velocity.y * 0.5f);
            IsJumpReleased = false;
        }

        //cancel rolling if you stop hitting z early
        /*if (rolling_cancelled)
        {
            rb.angularVelocity = 0.5f * rb.angularVelocity;
            rb.velocity = new Vector2(rb.velocity.x * 0.5f, rb.velocity.y);
            rolling_cancelled = false;
            rolling_timer = 0.0f;
        }*/



        //dash physics
        if (DashTimer > 0.01f)
        {
            if (Manager.ControllerInstance.DashPressed == true)
            {
                Manager.ControllerInstance.DashPressed = false;
            }
            GhostEffect.MakeGhost = true;
            //create_dash_particle();
            Rb.velocity = DashVelocity * DashDirection;
            DashTimer -= Time.deltaTime;
            if (DashTimer <= 0.01f)
            {
                Rb.velocity = DashEndVelocity * DashDirection;
            }
        }
        else
        {
            DashTimer = 0f;
        }
    }


    void create_dust()
    {
        Dust.transform.position = Rb.transform.position - Vector3.up * Manager.ColliderInstance.SpriteHeight * 0.5f;
        Dust.Play();
    }
}
