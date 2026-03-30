using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    public static void SetTransparency(this Image image, float transparency)
    {
        if (image != null)
        {
            Color alpha = image.color;
            alpha.a = transparency;
            image.color = alpha;
        }
    }
}