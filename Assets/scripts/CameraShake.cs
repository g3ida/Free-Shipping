using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float Power = 0.5f;
    public float Duration = 0.5f;
    private float DurationCounter;
    public bool DoShake = false;

    Vector3 start_position;
    public Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        start_position = cam.localPosition;
        DurationCounter = Duration;
    }
    void Update()
    {
        if (DoShake)
        {
            if (DurationCounter > 0f)
            {
                Vector2 tmp = Random.insideUnitCircle * Power;
                Vector3 random_addition = new Vector3(tmp.x, tmp.y, 0);
                cam.localPosition = start_position + random_addition;
                DurationCounter -= Time.deltaTime;
            }
            else
            {
                DoShake = false;
                DurationCounter = Duration;
                cam.localPosition = start_position;
            }
        }
    }
}
