using UnityEngine;
using UnityEditor;

public class StickyGlueBoxFace : BoxFace
{
    public new float rayDistance = 100f;
    public new Sprite sprite;
    public new int interactionMask;
    public new PlayerManager player;
    public StickyGlueBoxFace(PlayerManager player) : base(player)
    {
        interactionMask = 1 << 8;
        interactionMask = ~interactionMask;
    }

    public override void Interact(GameObject gameObject)
    {
        Debug.Log(gameObject.transform.position);
    }
}