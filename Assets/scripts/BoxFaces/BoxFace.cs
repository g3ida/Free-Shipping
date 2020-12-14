﻿using UnityEngine;
using System.Collections;

public abstract class BoxFace
{
    public float rayDistance = 0f;
    public Sprite sprite;
    public int interactionMask;
    public PlayerManager player;
    public PlayerManager.FaceIndex faceIndex;
    public BoxFace(PlayerManager player, PlayerManager.FaceIndex faceIndex)
    {
        this.player = player;
        this.faceIndex = faceIndex;
    }
    public abstract void Interact(RaycastHit2D hit);
    public virtual void Update()
    {}
    public virtual void Destroy()
    {}
}
