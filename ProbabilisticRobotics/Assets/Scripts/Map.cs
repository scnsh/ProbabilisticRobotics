using UnityEngine;
using System.Collections.Generic;
using System;

public class Map : MonoBehaviour {

  public List<Collider> m_obstacles = new List<Collider>();

	// Use this for initialization
	void Start () {

    Collider[] colliders = GetComponentsInChildren<Collider>();
    m_obstacles.AddRange(colliders);    	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  //マップの確率密度を返す
  public float isInObstacles(Vector3 lastPos, Vector3 newPos)
  {
    Ray ray = new Ray(lastPos, (newPos - lastPos).normalized);
    float maxDist = (newPos - lastPos).magnitude;
    for(int idx = 0; idx < m_obstacles.Count; ++idx)
    {
      if(ray.direction.magnitude > 0f)
      {
        RaycastHit hitInfo;
        bool isHit = m_obstacles[idx].Raycast(ray, out hitInfo, maxDist);
        if (isHit)
          return 0f; //どれか一つの障害物に対して衝突していれば0を返す
      }
    }
    return 1f; //どの障害物にも衝突していなければ1を返す(非負の値)
  }  
}
