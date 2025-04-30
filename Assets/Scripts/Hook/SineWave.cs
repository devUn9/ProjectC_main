using UnityEngine;

public class SineWave : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int points;
    public float amp = 1;
    public float fre = 1;
    public Vector2 xLimits = new Vector2(0,1);
    public float movement = 1;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Draw()
    {
        float xStart = 0;
        float Tau = 2 * Mathf.PI;
        float xFinish = xLimits.y;

        lineRenderer.positionCount = points;
        for(int currentPoint =0; currentPoint < points; ++currentPoint)
        {
            float progress = (float)currentPoint / (points-1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = Mathf.Sin((Tau * fre * x) + (Time.timeSinceLevelLoad*movement));
            lineRenderer.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }

    private void Update()
    {
        Draw();
    }
}
