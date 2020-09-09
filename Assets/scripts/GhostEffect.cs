using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    public float GhostDelay = 0.001f;
    private float GhostDelayCounter;
    public GameObject Ghost;
    [HideInInspector]
    public bool MakeGhost = false;

    void Start()
    {
        GhostDelayCounter = GhostDelay;
    }
    void Update()
    {
        if (MakeGhost)
        {
            if (GhostDelayCounter > 0.001f)
            {
                GhostDelayCounter -= Time.deltaTime;
            }
            else
            {
                MakeGhost = false;
                GameObject current_ghost = Instantiate(Ghost, transform.position, transform.rotation);
                Sprite current_sprite = GetComponent<SpriteRenderer>().sprite;
                current_ghost.GetComponent<SpriteRenderer>().sprite = current_sprite;
                current_ghost.transform.localRotation = this.transform.localRotation;
                current_ghost.transform.localScale = this.transform.localScale;
                Destroy(current_ghost, 0.1f);
                GhostDelayCounter = GhostDelay;
            }
        }
    }
}
