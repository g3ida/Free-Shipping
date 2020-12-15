using UnityEngine;

public class FrozenBoxFace : BoxFace
{
    public FrozenBoxFace(PlayerManager player, PlayerManager.FaceIndex faceIndex) : base(player, faceIndex)
    {
    }
    public override void Interact(RaycastHit2D hit)
    {
    }
    public override void Update()
    {
    }
    public override void Destroy()
    {
    }
}