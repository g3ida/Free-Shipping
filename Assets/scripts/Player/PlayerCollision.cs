using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public bool IsGrounded { get; private set; } //tells if the player is laying on the ground.
    public float TimeSinceGrounded { get; private set; } //how much time elapsed since the player left the ground.

    //the last face currently facing the ground or wall or anything
    public BoxCollider2D CurrentFace { get; private set; }

    //the trigger faces to detect collision
    public BoxCollider2D TopFace { get; private set; }
    public BoxCollider2D LeftFace { get; private set; }
    public BoxCollider2D RightFace { get; private set; }
    public BoxCollider2D BottomFace { get; private set; }

    private Rigidbody2D Rb;

    public float SpriteHeight { get; private set; }

    //for raycasting
    private int CollisionLayerMask;
    //indicates the length of the ray relatively to the player height.
    private readonly float RAY_LENGTH_FACTOR = 0.51f;

    void Awake()
    {
        IsGrounded = false;
        TimeSinceGrounded = 0f;

        Rb = GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteHeight = spriteRenderer.bounds.size.y;

        //setup the collision layer mask by excluding the player layer
        CollisionLayerMask = 1 << 8;
        CollisionLayerMask = ~CollisionLayerMask;

        TopFace = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        LeftFace = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        RightFace = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        BottomFace = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;

        TopFace.size = new Vector2(SpriteHeight * 0.96f, SpriteHeight * 0.2f);
        BottomFace.size = new Vector2(SpriteHeight * 0.96f, SpriteHeight * 0.2f);
        LeftFace.size = new Vector2(SpriteHeight * 0.2f, SpriteHeight * 0.96f);
        RightFace.size = new Vector2(SpriteHeight * 0.2f, SpriteHeight * 0.96f);

        TopFace.offset = new Vector2(0f, SpriteHeight * 0.41f);
        BottomFace.offset = new Vector2(0f, -SpriteHeight * 0.41f);
        LeftFace.offset = new Vector2(-SpriteHeight * 0.41f, 0f);
        RightFace.offset = new Vector2(SpriteHeight * 0.41f, 0f);

        TopFace.isTrigger = true;
        BottomFace.isTrigger = true;
        LeftFace.isTrigger = true;
        RightFace.isTrigger = true;
        //we can change properties like bounciness etc...
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.IsTouching(TopFace))
        {
            //Debug.Log("top_face");
            CurrentFace = TopFace;
        }
        else if (col.IsTouching(BottomFace))
        {
            //Debug.Log("bottom_face");
            CurrentFace = BottomFace;
        }
        else if (col.IsTouching(LeftFace))
        {
            //Debug.Log("left_face");
            CurrentFace = LeftFace;
        }
        else if (col.IsTouching(RightFace))
        {
            //Debug.Log("right_face");
            CurrentFace = RightFace;
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
        if (IsGrounded == false)
        {
            TimeSinceGrounded += Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        float angle_addition = Mathf.Abs(Mathf.Cos((2f * 3.141592f / 180f) * ((Rb.rotation - 45) % 90f)));
        float height_correction = (1 + 0.4142f * angle_addition) * SpriteHeight * RAY_LENGTH_FACTOR;

        //raycasting to see if the player hits the ground.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, height_correction, CollisionLayerMask);
        if (hit.collider != null)
        {
            IsGrounded = true;
            TimeSinceGrounded = 0f;
        }
        else
        {
            IsGrounded = false;
            Debug.DrawRay(transform.position, -Vector2.up * SpriteHeight * 1.414214f * RAY_LENGTH_FACTOR, Color.white);
        }
    }
}
