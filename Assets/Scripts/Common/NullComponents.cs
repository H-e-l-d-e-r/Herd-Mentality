using System;
using UnityEngine;

public static class NullComponents
{
    public static Exception ThrowIfNull(Component obj)
    {
        if(obj == null)
        {
            Exception e = new ArgumentNullException(obj.name);
            Debug.LogException(e);
            return e;
        }

        return null;
    }

    public static Exception ThrowIfNull(GameObject obj)
    {
        if (obj == null)
        {
            Exception e = new ArgumentNullException(obj.name);
            Debug.LogException(e);
            return e;
        }

        return null;
    }
}