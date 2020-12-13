using UnityEngine;
using UnityEditor;
using System;

public class StickyGlueBoxFace : BoxFace
{
    private SpringJoint2D joint_;
    bool wasJoint_ = false; //indicates whether the joint was estableshed or not. 
    public StickyGlueBoxFace(PlayerManager player) : base(player)
    {
        //set up mask
        interactionMask = Physics.DefaultRaycastLayers;
        interactionMask &= ~0 ^ Layers.PLAYER; // exclude the player
        //interactionMask = ~interactionMask;
        rayDistance = player.ColliderInstance.SpriteHeight * 0.54f;
    }

    public override void Interact(RaycastHit2D hit)
    {
        //debug the joint
        if (joint_ != null)
        {
            Debug.DrawLine(player.transform.localToWorldMatrix.MultiplyPoint3x4(joint_.anchor),
                hit.collider.gameObject.transform.localToWorldMatrix.MultiplyPoint3x4(joint_.connectedAnchor), Color.blue);
        }

        if (joint_ == null)
        {
            joint_ = CreateJointFromRaycastHit(hit, breakForce: 2000f, frequency: 20f);
            wasJoint_ = true;
            //reset the dash counter so that the player can dash after being stuck to a wall
            player.ControllerInstance.DashCounter = 0;
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
}