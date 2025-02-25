using UnityEngine;

public class DeltaTimeLogger : MonoBehaviour
{
    private float maxDeltaTime = 0f;

    void Update()
    {
        if (Time.deltaTime > maxDeltaTime)
        {
            maxDeltaTime = Time.deltaTime;
            Debug.Log($"[Frame={Time.frameCount}] New max delta={maxDeltaTime}");
        }
    }
}

