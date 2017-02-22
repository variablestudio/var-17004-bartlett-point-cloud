/// <summary>
/// Catmul rom splines
/// implemented from http://www.mvps.org/directx/articles/catmull/
/// </summary>

using UnityEngine;    
using System.Collections.Generic;     

public class CatmullRom {
  public static Vector3 GetPoint(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t) {
   return 0.5f *((2 * P1) + (-P0 + P2) * t + (2*P0 - 5*P1 + 4*P2 - P3) * t * t + (-P0 + 3*P1- 3*P2 + P3) * t * t * t);
  }
  
  public static float GetValue(float P0, float P1, float P2, float P3, float t) {
   return 0.5f *((2 * P1) + (-P0 + P2) * t + (2*P0 - 5*P1 + 4*P2 - P3) * t * t + (-P0 + 3*P1- 3*P2 + P3) * t * t * t);
  }
}       

public struct DistanceResult {
  public float t;
  public float distance;
  public Vector3 point;  
  public Vector3 forward;
  public Vector3 right;
  public Vector3 up;
}


public class Vector3CatmullRomSpline {
  private List<Vector3> values;                      
  //have length for each segment, last ratio is 0
  private List<float> lengths;   
  //starts with 0 and ends with 1
  private List<float> accumulatedRatios; 
  
  public List<Vector3> interpolatedValues; 
  public List<Vector3> normalizedValues;
  private float subdivisionPrecision = 0.20f;   
  private float desiredSegmentLength = 2.0f;
  
  public Vector3CatmullRomSpline() {
    values = new List<Vector3>();
    lengths = new List<float>();
    accumulatedRatios = new List<float>(); 
    interpolatedValues = new List<Vector3>();   
    normalizedValues = new List<Vector3>();
  }                        
  
	/// <summary>
	/// Adds a point in the spline
	/// </summary>
	/// <param name="value">
	/// A <see cref="Vector3"/>
	/// </param>
  public void Add(Vector3 value) {
    values.Add(value);
  }    
    
  /// <summary>
  /// precalculate what's the ratio |segment| / |whole spline|
  /// </summary>
  public void Precalc() { 
    //lengths.Clear();
    //accumulatedRatios.Clear();
    //interpolatedValues.Clear();
    //normalizedValues.Clear();
    if (lengths.Count == values.Count) {
      return;
    }        
        
    float totalLength = 0;                                 
    int numSteps = 10;
    float step = 1.0f/numSteps;
    for(int i=0; i<values.Count-1; i++) {
      float length = 0;             
      Vector3 prevPos = GetValue(i, 0);
      float t = step;      
      for(int j=0; j<numSteps; j++, t+=step) {
        Vector3 pos = GetValue(i, t);
        length += (pos - prevPos).magnitude;
        prevPos = pos;
      }           
      lengths.Add(length);      
      totalLength += length;
      //Debug.Log("Segment i:" + i + " length:" + length + " t:" + t + " len:" + (values[i+1]-values[i]).magnitude);  
    }                     
    //last segment is 0, adding for easier ratio calculations                                           
    lengths.Add(0);   
    
    for(int i=0; i<values.Count; i++) {  
      numSteps = (int)(lengths[i] / subdivisionPrecision);   
      step = 1.0f/numSteps;  
      float t = 0;
      for(int j=0; j<numSteps; j++, t+=step) {              
        interpolatedValues.Add(GetValue(i, t));
      } 
    }  
    interpolatedValues.Add(values[values.Count-1]);
    
    float totalRatio = 0;
    for(int i=0; i<values.Count; i++) {                 
      //Debug.Log("Segment i:" + i + " accRatio:" + totalRatio);
      accumulatedRatios.Add(totalRatio);
      totalRatio += lengths[i]/totalLength;
    }     
    
    //normalize values   
    Vector3 P = interpolatedValues[0]; 
    Vector3 prevP = P;
    normalizedValues.Add(P);       
    for(int i=1; i<interpolatedValues.Count; i++) {
      P = interpolatedValues[i];
      if ((P - prevP).magnitude >= desiredSegmentLength) {
        normalizedValues.Add(interpolatedValues[i]);
        prevP = P; 
      }
    }
        
    //Debug.Log("Total length:" + totalLength); 
    //Debug.Log("Total ratio:" + totalRatio); 
  }                                 
  
	/// <summary>
	/// Get position on spline
	/// </summary>
	/// <param name="i">
	/// A <see cref="System.Int32"/>. index point on spline
	/// </param>
	/// <param name="t">
	/// A <see cref="System.Single"/>. position on spline segment
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
  public Vector3 GetValue(int i, float t) {      
    if (i == 0 && t == 0) {
      return values[0];
    }                  
    
    if (i >= values.Count - 1) {
      return values[values.Count - 1];
    }
    
    Vector3 prevPos;    
    Vector3 currPos = values[i];
    Vector3 nextPos = values[i+1];
    Vector3 nextNextPos;  
    
    if (i > 0) {
      prevPos = values[i-1];
    }   
    else {
      //extrapolated
      prevPos = currPos - (nextPos - currPos);  
    } 
                      
    if (i < values.Count - 2) {
      nextNextPos = values[i + 2];
    }
    else { 
      //extrapolated      
      nextNextPos = nextPos + (nextPos - currPos);
    }  
    
    return CatmullRom.GetPoint(prevPos, currPos, nextPos, nextNextPos, t);
  }    
  
  //actually this is not so precised, drop it
  private Vector3 GetPrecise(float t) {
    int idx = -1;                 
    for(int i=0; i<accumulatedRatios.Count; i++) {
      if (accumulatedRatios[i] >= t) {
        idx = i-1;
        break;
      }
    }  
    float tt = (t - accumulatedRatios[idx])/(accumulatedRatios[idx+1] - accumulatedRatios[idx]);
    
    return GetValue(idx, tt);
  }   
  
  private Vector3 GetNormalized(float t) {
    float fIndex = (t*(normalizedValues.Count-1));
    int iIndex = (int)Mathf.Floor(fIndex);
    float reminder = fIndex - iIndex; 
    return normalizedValues[iIndex] + reminder * (normalizedValues[iIndex + 1] - normalizedValues[iIndex]);
  }
  
	/// <summary>
	/// Get a position on the spline from a normalized position along path
	/// </summary>
	/// <param name="t">
	/// A <see cref="System.Single"/>. Normalized position along path
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
  public Vector3 GetPosition(float t) { 
    Precalc();
    
    if (values.Count == 0) {
      return Vector3.zero;
    }   
    
    if (t <= 0) return values[0];
    if (t >= 1) return values[values.Count-1];  
    return GetNormalized(t);
  }
   
  /// <summary>
  /// based on http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
  /// </summary>
  /// <param name="point">
  /// A <see cref="Vector3"/>
  /// </param>
  /// <returns>
  /// A <see cref="DistanceResult"/>
  /// </returns>
  public DistanceResult GetDistance(Vector3 point) {
    DistanceResult result = new DistanceResult();
    result.distance = -1;    
      
    for(int i=0; i<normalizedValues.Count; i++) {    
      float distance = (normalizedValues[i] - point).magnitude;
      if (result.distance == -1 || result.distance > distance) {
        result.distance = distance;
        result.point = normalizedValues[i];
        result.t = i / (float)(normalizedValues.Count - 1);
        result.up = Vector3.up;  
        if (i == 0) {
          result.forward = (normalizedValues[i+1] - normalizedValues[i]).normalized;          
        } 
        else {
          result.forward = (normalizedValues[i] - normalizedValues[i-1]).normalized;
        } 
        result.right = Vector3.Cross(result.forward, result.up).normalized;       
        result.up = Vector3.Cross(result.right, result.forward).normalized;             
      }
    }
    return result;
  }
}