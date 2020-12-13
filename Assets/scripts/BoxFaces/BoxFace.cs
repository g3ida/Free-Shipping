using UnityEngine;
using System.Collections;

public abstract class BoxFace
{
    public float rayDistance = 0f;
    public Sprite sprite;
    public int interactionMask;
    public PlayerManager player;
    public BoxFace(PlayerManager player)
    {
        this.player = player;
    }
    public abstract void Interact(RaycastHit2D hit);
    public virtual void Update()
    {

    }
}
