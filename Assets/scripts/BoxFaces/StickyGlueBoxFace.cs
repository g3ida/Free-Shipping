using UnityEngine;
using UnityEditor;

public class StickyGlueBoxFace : BoxFace
{
    public StickyGlueBoxFace(PlayerManager player) : base(player)
    {
        //set up mask
        interactionMask = Physics.DefaultRaycastLayers;
        interactionMask &= ~0 ^ Layers.PLAYER;
        //interactionMask = ~interactionMask;
        rayDistance = player.ColliderInstance.SpriteHeight *0.6f;
    }

    public override void Interact(GameObject gameObject)
    {
        Debug.Log(gameObject.transform.position);
    }
}