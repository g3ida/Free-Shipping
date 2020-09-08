using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float power = 0.5f;
    public float duration = 0.5f;
    private float duration_counter;
    public bool do_shake = false;

    Vector3 start_position;
    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        start_position = cam.localPosition;
        duration_counter = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (do_shake)
        {
            if (duration_counter > 0f)
            {
                Vector2 tmp = Random.insideUnitCircle * power;
                Vector3 random_addition = new Vector3(tmp.x, tmp.y, 0);
                cam.localPosition = start_position + random_addition;
                duration_counter -= Time.deltaTime;
            }
            else
            {
                do_shake = false;
                duration_counter = duration;
                cam.localPosition = start_position;
            }
        }
    }
}
