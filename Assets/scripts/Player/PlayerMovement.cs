using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{

    public PlayerManager manager;

    private bool was_grounded;

    //ghost effect
    public GhostEffect ghost_effect;

    private Animator player_animator;
    public ParticleSystem dust;
    //public ParticleSystem dash_particle;

    [SerializeField]
    CameraShake camera_shake; 
    
    public float velocity = 40.0f;

    public float jump_force = 1000.0f;
    public float drag_force_constant = 20f;
    private Rigidbody2D rb;

    //jump stuff
    private float time_until_full_jump_is_considered = 0.2f;
    private float jump_timer = 0f;
    private bool released_jump = false;

    //rolling stuff
    /*private float time_to_cancel_rolling = 0.8f;
    private float rolling_timer = 0f;
    private bool rolling_cancelled = false;*/

    //sliding stuff
    private float time_till_push = 0.45f;
    private float direction = 0;
    private float timer_till_next_move = 0f;
    //private bool torque = false;

    //for the rotation of the box while jumping
    private float rotation_duration = 0.15f;
    private float theta_zero;
    private float theta_target;
    private float theta_point;
    private float rotation_timer = 0f;


    //dash stuff
    public float dash_time = 0.12f;
    private float dash_timer = 0.0f;
    public float dash_velocity = 70.0f;
    public float dash_end_velocity = 3.0f;
    public Vector2 dash_direction;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        was_grounded = manager.player_collision.is_grounded;
        //we can change properties like bounciness etc...
    }


    void Start()
    {
        player_animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        
        //dash event
        if (manager.player_controller.dash_pressed)
        {
            camera_shake.do_shake = true;

            int hor = manager.player_controller.direction;
            int vert = manager.player_controller.direction_vert;
            if (vert != 0) { hor = 0; }
            
            dash_direction = new Vector2(hor, vert);
            dash_timer = dash_time;            
        }

        if (manager.player_controller.rotation_pressed)
        {
            manager.player_controller.rotation_pressed = false;
            theta_zero = rb.rotation;
            theta_target = Mathf.Round((rb.rotation + 90) / 90f) * 90f;
            theta_point = (theta_target - theta_zero) / rotation_duration;
            rotation_timer = rotation_duration;
            rb.constraints = RigidbodyConstraints2D.None;
        }


        if (manager.player_controller.reversed_rotation_pressed)
        {
            manager.player_controller.reversed_rotation_pressed = false;
            theta_zero = rb.rotation;
            theta_target = Mathf.Round((rb.rotation - 90) / 90f) * 90f;
            theta_point = (theta_target - theta_zero) / rotation_duration;
            rotation_timer = rotation_duration;
            rb.constraints = RigidbodyConstraints2D.None;
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
        if (jump_timer > 0.001f)
        {
            jump_timer -= Time.deltaTime;
        }

        //jump has been released before full jump reached
        if (manager.player_controller.jump_released == true)
        {
            manager.player_controller.jump_released = false;
            if (jump_timer > 0.001f)
            {
                released_jump = true;
            }
        }


    }

    void FixedUpdate()
    {

        //if just landed play the dust particle effects
        if (manager.player_collision.is_grounded && !was_grounded)
        {
            create_dust();
            if (manager.player_collision.current_face == manager.player_collision.bottom_face ||
                manager.player_collision.current_face == manager.player_collision.top_face)
            {
                player_animator.SetInteger("ground_side", 1);
            }
            player_animator.SetBool("jump_squash", true);
        }
        else
        {
            player_animator.SetInteger("ground_side", 2);
            player_animator.SetBool("jump_squash", false);
        }

        was_grounded = manager.player_collision.is_grounded;



        //no rotation action is performed then do not allow rotation
        if(rotation_timer == 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        //the rotation
        if (rotation_timer > 0f)
        {
            rotation_timer -= Time.deltaTime;
            if (rotation_timer < 0f)
            {
                theta_point = 0f;
                rb.angularVelocity = 0f;
                rb.rotation = theta_target;
            }
            rb.angularVelocity = theta_point;
        }
        else
        {
            rotation_timer = 0f;
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
        if (manager.player_controller.jump_pressed)
        {
            jump_timer = time_until_full_jump_is_considered;
            //rb.velocity = Vector2.up * jump_force + rb.velocity;
            rb.AddForce(new Vector2(0f, jump_force));
            manager.player_controller.jump_pressed = false;
        }

        //if no dash and on the air the follow input
        if (!manager.player_collision.is_grounded && dash_timer < 0.01f)
        {
            rb.velocity = new Vector2(manager.player_controller.move_input * velocity, rb.velocity.y);
        }


        //walk on the ground
        if(manager.player_collision.is_grounded)
        {
            float rot = Mathf.Abs(rb.rotation) % 360;
            if (Mathf.Abs(rot - 90f) < 0.1f || Mathf.Abs(rot - 270f) < 0.1f)
            {
                spriteRenderer.sharedMaterial.SetFloat("_VerticalSkew", -manager.player_controller.move_input * 0.05f);
                rb.velocity = new Vector2(manager.player_controller.move_input * velocity, rb.velocity.y);
            } else
            {
                spriteRenderer.sharedMaterial.SetFloat("_HorizontalSkew", manager.player_controller.move_input * 0.05f);
                rb.velocity = new Vector2(manager.player_controller.move_input * velocity, rb.velocity.y);
            }
        } else
        {
            spriteRenderer.sharedMaterial.SetFloat("_VerticalSkew", 0);
            spriteRenderer.sharedMaterial.SetFloat("_HorizontalSkew", 0);
        }


        //drag force uncomment this if you want to add some air resistance
        //Vector2 drag_force = new Vector2(rb.velocity.x - drag_force_constant * Math.Abs(rb.velocity.x) * rb.velocity.x, 0.0f);
        //rb.AddForce(drag_force, ForceMode2D.Force);




        //allow the cancelling of the jump
        if (released_jump && rb.velocity.y > 0)
        {
            //Debug.Log("here");
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            released_jump = false;
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
        if (dash_timer > 0.01f)
        {
            if(manager.player_controller.dash_pressed == true)
            {
                manager.player_controller.dash_pressed = false;
            }
            ghost_effect.make_ghost = true;
            //create_dash_particle();
            rb.velocity = dash_velocity * dash_direction;
            dash_timer -= Time.deltaTime;
            if(dash_timer <= 0.01f) {
                rb.velocity = dash_end_velocity * dash_direction;
            }
        }
        else
        {
            dash_timer = 0f;
        }
    }


    void create_dust()
    {
        dust.transform.position = rb.transform.position -Vector3.up * manager.player_collision.sprite_height * 0.5f;
        dust.Play();
    }
}
