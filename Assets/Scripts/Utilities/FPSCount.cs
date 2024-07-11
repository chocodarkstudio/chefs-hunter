using UnityEngine;

public class FPSCount : MonoBehaviour
{
    public static float FPS { get; private set; } = 0.0f;

    float frameCount = 0f;
    float dt = 0.0f;
    const float updateRate = 4.0f;  // 4 updates per sec.


    void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0f / updateRate)
        {
            FPS = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }

        //Debug.Log(FPS);
    }


}
