using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitArea : MonoBehaviour {

    /// <summary>
    /// Script to check that a 2D vector point is inside a polygon (Array of 2D vectors)
    /// </summary>
    public static bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
    { 
        int j = polyPoints.Length - 1;
        bool inside = false; 
        for (int i = 0; i < polyPoints.Length; j = i++) { 
          if (((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y<polyPoints[i].y)) && 
             (p.x<(polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
             inside = !inside; 
        }
       return inside; 
    }
}
