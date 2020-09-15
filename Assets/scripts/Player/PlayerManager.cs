using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{

    public PlayerController ControllerInstance;
    public PlayerCollision ColliderInstance;
    public PlayerMovement MovementInstance;

    public const int NUM_FACES = 4;
    //faces are sorted in the order : top, left, bottom, right
    public enum FaceIndex{ top = 0, left = 1, bottom = 2, right = 3 }
    public BoxFace[] faces = { null, null, null, null };

    // Start is called before the first frame update
    void Start()
    {
        ColliderInstance.Manager = this;
        MovementInstance.Manager = this;
        ControllerInstance.Manager = this;
        //setup a test face
        faces[(int)FaceIndex.top] = new StickyGlueBoxFace(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
