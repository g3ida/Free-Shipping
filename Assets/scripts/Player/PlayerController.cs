using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public PlayerManager manager;
    public float move_input { get; private set; }
    public float vertical_input { get; private set; }

    public int direction { get; private set; }
    public int direction_vert { get; private set; }


    struct ActionSettings
    {
        public ActionSettings(float permissiveness, float responsiveness)
        {
            this.permissiveness = permissiveness;
            this.responsiveness = responsiveness;
        }

        public float permissiveness; //how much time we permit action since its conditions were met.
        public float responsiveness; //how much do we dalay action hoping its conditions to be met.
    }


    ActionSettings jump_settings = new ActionSettings(0.01f, 0.01f);
    ActionSettings dash_settings = new ActionSettings(0.01f, 0.01f);

    //jump stuff
    public bool jump_pressed = false;
    public bool jump_delayed = false;
    public bool jump_released = false;
    private float time_until_full_jump_is_considered = 0.15f;
    private float jump_timer = 0f;
    private bool released_jump = false;


    public bool rotation_pressed = false;
    public bool reversed_rotation_pressed = false;
    public bool rolling_down = false;

    //dash settings
    public int max_allowed_dashes = 1;
    public float dash_time = 0.12f;
    public float dash_velocity = 70.0f;
    public float dash_end_velocity = 3.0f;

    public bool dash_pressed = false;

    private int dash_counter = 0;
    private float dash_timer = 0.0f;
    private Vector2 dash_direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if(manager.player_collision.is_grounded)
        {
            dash_counter = 0;
        }

        move_input = Input.GetAxisRaw("Horizontal");
        vertical_input = Input.GetAxisRaw("Vertical");
        
        direction = (move_input > 0.7f ? 1 : (move_input < -0.7f ? -1 : direction));
        //unlike the direction the vertical direction is not kept if no input is found.
        direction_vert = (vertical_input > 0.7f ? 1 : (vertical_input < -0.7f ? -1 : 0));

        update_jump();
        update_dash();
        update_rotation();
        update_rolling();
    }


    void update_rotation()
    {
        rotation_pressed = false;
        if (Input.GetKeyDown(KeyCode.Z) && !manager.player_collision.is_grounded)
        {
            rotation_pressed = true;
        }
        if (Input.GetKeyDown(KeyCode.V) && !manager.player_collision.is_grounded)
        {
            reversed_rotation_pressed = true;
        }
    }

    void update_rolling()
    {
        if (Input.GetKey(KeyCode.Z) && manager.player_collision.is_grounded)
        {
            rolling_down = true;
        }
        else
        {
            rolling_down = false;
        }
    }


    /////////////////////////
        #region Dash
    /////////////////////////


    IEnumerator set_dash_after_delay(float Count)
    {
        yield return new WaitForSeconds(Count);
        //just check once again
        if (dash_counter < max_allowed_dashes) {
            dash_pressed = true;
            dash_counter++;
        }
        yield return null;
    }


    void update_dash()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (dash_counter < max_allowed_dashes)
            {
                StartCoroutine("set_dash_after_delay", dash_settings.responsiveness);
            }
        }
    }
    #endregion


    /////////////////////////
    #region Jump
    /////////////////////////

    void update_jump()
    {
        //jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (can_jump())
            {
                jump_pressed = true;
            } else
            {
                jump_delayed = true;
                StartCoroutine("test_jump_after_delay", jump_settings.responsiveness);
            }
        }


        if (Input.GetButtonUp("Jump"))
        {
            if(jump_delayed == true)
            {
                jump_delayed = false;
            }
            jump_pressed = false;
            jump_released = true;
        }
    }


    bool can_jump()
    {
        if(manager.player_collision.is_grounded)
        {
            return true;
        }

        if(manager.player_collision.time_since_grounded <= jump_settings.permissiveness && !jump_pressed)
        {
            return true;
        }

        return false;

    }

    IEnumerator test_jump_after_delay(float Count)
    {
        yield return new WaitForSeconds(Count);

        if(jump_delayed == false)
        {
            //need to yield release
        }

        jump_delayed = false;
        if (manager.player_collision.is_grounded)
        {
            jump_pressed = true;
        }
        yield return null;
    }
    #endregion
}
