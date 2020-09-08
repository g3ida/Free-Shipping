using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : MonoBehaviour
{

    public float ghost_delay = 0.001f;
    private float ghost_delay_counter;
    public GameObject ghost;
    public bool make_ghost = false;

    // Start is called before the first frame update
    void Start()
    {
        ghost_delay_counter = ghost_delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (make_ghost)
        {
            if (ghost_delay_counter > 0.001f)
            {
                ghost_delay_counter -= Time.deltaTime;
            }
            else
            {
                make_ghost = false;
                GameObject current_ghost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite current_sprite = GetComponent<SpriteRenderer>().sprite;
                current_ghost.GetComponent<SpriteRenderer>().sprite = current_sprite;
                current_ghost.transform.localRotation = this.transform.localRotation;
                current_ghost.transform.localScale = this.transform.localScale;
                Destroy(current_ghost, 0.1f);
                ghost_delay_counter = ghost_delay;
            }
        }
    }
}
