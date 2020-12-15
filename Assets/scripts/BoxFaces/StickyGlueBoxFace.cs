using UnityEngine;

public class StickyGlueBoxFace : BoxFace
{
    private SpringJoint2D joint_;
    bool wasJoint_ = false; //indicates whether the joint was estableshed or not. 
    public StickyGlueBoxFace(PlayerManager player, PlayerManager.FaceIndex faceIndex) : base(player, faceIndex)
    {
        //set up mask
        interactionMask = Physics.DefaultRaycastLayers;
        interactionMask &= ~0 ^ Layers.PLAYER; // exclude the player
        //interactionMask = ~interactionMask;
        rayDistance = player.ColliderInstance.SpriteHeight * 0.54f;

        AddSprite(Resources.Load<Texture2D>("box_glue"), faceIndex);
    }
    public override void Interact(RaycastHit2D hit)
    {
        if (joint_ != null)
        {
            //debug the joint line
            Debug.DrawLine(player.transform.localToWorldMatrix.MultiplyPoint3x4(joint_.anchor),
                hit.collider.gameObject.transform.localToWorldMatrix.MultiplyPoint3x4(joint_.connectedAnchor), Color.blue);
            //reset the dash counter so that the player can dash after being stuck to a wall
            player.ControllerInstance.DashCounter = 0;
        }

        if (joint_ == null)
        {
            joint_ = CreateJointFromRaycastHit(hit, breakForce: 2000f, frequency: 20f);
            wasJoint_ = true;
        }
    }
    SpringJoint2D CreateJointFromRaycastHit(RaycastHit2D hit, float breakForce, float frequency)
    {
        SpringJoint2D joint = player.gameObject.AddComponent<SpringJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
        joint.connectedAnchor = hit.collider.gameObject.transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point);
        joint.breakForce = breakForce;
        joint.frequency = frequency;
        joint.enableCollision = true;
        joint.autoConfigureDistance = false;
        joint.distance = player.ColliderInstance.SpriteHeight * 0.5f;
        return joint;
    }
    public GameObject AddSprite(Texture2D tex, PlayerManager.FaceIndex faceIndex)
    {
        Texture2D _texture = tex;
        Sprite newSprite = Sprite.Create(_texture, new Rect(0f, 0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
        GameObject sprGameObj = new GameObject();
        sprGameObj.name = "glueSprite";
        sprGameObj.AddComponent<SpriteRenderer>();
        SpriteRenderer sprRenderer = sprGameObj.GetComponent<SpriteRenderer>();
        sprRenderer.sortingOrder = 100;
        sprRenderer.transform.position = player.transform.position;
        sprRenderer.transform.Rotate(Vector3.forward * 90 * (int)faceIndex);
        sprRenderer.sprite = newSprite;
        sprRenderer.sortingLayerName = "player";
        sprGameObj.transform.parent = player.transform;
        return sprGameObj;
    }
    public override void Update()
    {   
        //if the joint was broken this frame
        //(note that we can't use Joint.OnJointBreak(float) because it should be called
        // in the object that have the joint componenet or we want it here to separate logic)
        if (joint_ == null && wasJoint_)
        {
            //set was joint to false so this want get executed twice.
            wasJoint_ = false;
            //nothing to do for this moment
        }
    }
    public override void Destroy()
    {
    }
}