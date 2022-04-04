using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerEditor
{
    public static class MathUtils
    {
        
        public static Vector2 Abs(this Vector2 vec)
        {
            return new Vector2(
                Mathf.Abs(vec.x),
                Mathf.Abs(vec.y));
        }
        public static Vector3 Abs(this Vector3 vec)
        {
            return new Vector3(
                Mathf.Abs(vec.x),
                Mathf.Abs(vec.y),
                Mathf.Abs(vec.z));
        }
        public static Vector4 Abs(this Vector4 vec)
        {
            return new Vector4(
                Mathf.Abs(vec.x),
                Mathf.Abs(vec.y),
                Mathf.Abs(vec.z),
                Mathf.Abs(vec.w));
        }
    }   
}

