using UnityEngine;

public class Health_Utill
{
    public static float Percent(float current, float max)
    {
        return current != 0 && max != 0 ? current / max : 0;
    }
}
