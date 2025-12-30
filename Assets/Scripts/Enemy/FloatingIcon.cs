using UnityEngine;

public class FloatingIcon : MonoBehaviour
{
    public float amplitude = 0.2f; // რამდენად მაღლა/დაბლა ავიდეს
    public float frequency = 2f;   // მოძრაობის სისწრაფე

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // მათემატიკური ფორმულა რბილი ლივლივისთვის
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}