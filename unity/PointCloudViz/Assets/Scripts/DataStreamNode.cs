using UnityEngine;
using System.Collections;

/// <summary>
/// Node on spline of Current stream. Should be child of CurrentStream instance
/// </summary>
public class DataStreamNode : MonoBehaviour {
  /// <summary>
  /// Next node in spline
  /// </summary>
	public DataStreamNode next; 
	private DataStreamNode prev = null; 
	/// <summary>
	/// radius of spline at this position
	/// </summary>
  public float radius = 1.0f; 
  
	/// <summary>
	/// Is this the first node in the spline?
	/// </summary>
   public bool IsFirst {
    get { return (prev == null); }
  }      
  
  void Awake() {
    if (next != null) {
      next.prev = this;
    }
  }
  
	void Start() {
	
	}
	
	void Update() {
	
	}    
	
	void OnDrawGizmos() {
     	Gizmos.DrawWireCube(transform.position, new Vector3(1,1,1));     
	if (next != null) {
		Gizmos.color = Color.red;
	       Gizmos.DrawLine(transform.position, next.transform.position);

	   }
	}
	
}
