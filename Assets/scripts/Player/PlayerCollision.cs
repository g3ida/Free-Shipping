using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PlayerCollision : MonoBehaviour
{

    public bool is_grounded { get; private set; } //tells if the player is laying on the ground.
    public float time_since_grounded { get; private set; } //how much time elapsed since the player left the ground.
    
    //the last face currently facing the ground or wall or anything
    public BoxCollider2D current_face { get; private set; }

    //the trigger faces to detect collision
    public BoxCollider2D top_face { get; private set; }
    public BoxCollider2D left_face { get; private set; }
    public BoxCollider2D right_face { get; private set; }
    public BoxCollider2D bottom_face { get; private set; }

    private Rigidbody2D rb;

    public float sprite_height { get; private set; }

    //for raycasting
    private int collision_layer_mask;
    //indicates the length of the ray relatively to the player height.
    private float ray_length_factor = 0.51f;

    void Awake()
    {
        is_grounded = false;
        time_since_grounded = 0f;

        rb = GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        sprite_height = spriteRenderer.bounds.size.y;


        //setup the collision layer mask by excluding the player layer
        collision_layer_mask = 1 << 8;
        collision_layer_mask = ~collision_layer_mask;

        top_face = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        left_face = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        right_face = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bottom_face = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;

        top_face.size = new Vector2(sprite_height * 0.96f, sprite_height * 0.2f);
        bottom_face.size = new Vector2(sprite_height * 0.96f, sprite_height * 0.2f);
        left_face.size = new Vector2(sprite_height * 0.2f, sprite_height * 0.96f);
        right_face.size = new Vector2(sprite_height * 0.2f, sprite_height * 0.96f);

        top_face.offset = new Vector2(0f, sprite_height * 0.41f);
        bottom_face.offset = new Vector2(0f, -sprite_height * 0.41f);
        left_face.offset = new Vector2(-sprite_height * 0.41f, 0f);
        right_face.offset = new Vector2(sprite_height * 0.41f, 0f);

        top_face.isTrigger = true;
        bottom_face.isTrigger = true;
        left_face.isTrigger = true;
        right_face.isTrigger = true;

        //we can change properties like bounciness etc...
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.IsTouching(top_face))
        {
            //Debug.Log("top_face");
            current_face = top_face;
        }
        else if (col.IsTouching(bottom_face))
        {
            //Debug.Log("bottom_face");
            current_face = bottom_face;
        }
        else if (col.IsTouching(left_face))
        {
            //Debug.Log("left_face");
            current_face = left_face;
        }
        else if (col.IsTouching(right_face))
        {
            //Debug.Log("right_face");
            current_face = right_face;
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        //TODO
        //i need to take into account if multiple faces
        //are  colliding at the same time


        /*if (col.IsTouching(top_face))
        {
        }
        else if (col.IsTouching(bottom_face))
        {
        }
        else if (col.IsTouching(left_face))
        {
        }
        else if (col.IsTouching(right_face))
        {
        }*/

    }


    private void Update()
    {
        if (is_grounded == false)
        {
            time_since_grounded += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        float angle_addition = Mathf.Abs(Mathf.Cos((2f * 3.141592f / 180f) * ((rb.rotation - 45) % 90f)));
        float height_correction = (1 + 0.4142f * angle_addition) * sprite_height * ray_length_factor;

        //raycasting to see if the player hits the ground.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, height_correction, collision_layer_mask);
        if (hit.collider != null)
        {
            is_grounded = true;
            time_since_grounded = 0f;
        }
        else
        {
            is_grounded = false;
            Debug.DrawRay(transform.position, -Vector2.up * sprite_height * 1.414214f * ray_length_factor, Color.white);
        }
    }
}
