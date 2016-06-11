using UnityEngine;
using System.Collections;

public class SampleVelocityWithMap : MonoBehaviour {

  public Map m_map = null;

  public bool m_simStart = false;
  public float m_simTime = 0f;

  public float m_v = 0f;
  public float m_w = 0f;

  //動作の雑音パラメータ
  public float m_a1 = 0f;
  public float m_a2 = 0f;
  public float m_a3 = 0f;
  public float m_a4 = 0f;
  public float m_a5 = 0f;
  public float m_a6 = 0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    
    if(m_simStart)
    {
      //ノイズを追加
      float v = m_v + sample_normal_distribution(m_a1 * m_v * m_v + m_a2 * m_w * m_w);
      float w = m_w + sample_normal_distribution(m_a3 * m_v * m_v + m_a4 * m_w * m_w);
      float gamma = sample_normal_distribution(m_a5 * m_v * m_v + m_a6 * m_w * m_w);

      Vector3 euler = transform.rotation.eulerAngles; 
      float theta = euler.z;
      float newTheta = theta + w * Time.deltaTime + gamma * Time.deltaTime;

      float p = 0f;
      int counter = 0; //無限ループを回避する(解が見つからない場合は動かない)
      while(p == 0f && counter < 100)
      {
        //次の位置を求める
        Vector3 currentPos = transform.position;
        Vector3 nextPos = new Vector3(
          transform.position.x - (v / w) * Mathf.Sin(theta) + (v / w) * Mathf.Sin(theta + w * Time.deltaTime),
          transform.position.y + (v / w) * Mathf.Cos(theta) - (v / w) * Mathf.Cos(theta + w * Time.deltaTime),
          transform.position.z);

        //障害物判定を行う
        p = m_map.isInObstacles(currentPos, nextPos);
        if(p > 0f)
        {
          //位置と向きの更新
          transform.position = new Vector3(
              transform.position.x - (v / w) * Mathf.Sin(theta) + (v / w) * Mathf.Sin(theta + w * Time.deltaTime),
              transform.position.y + (v / w) * Mathf.Cos(theta) - (v / w) * Mathf.Cos(theta + w * Time.deltaTime),
              transform.position.z);
          transform.rotation = Quaternion.AngleAxis(newTheta, Vector3.forward);
        }
        counter++;
      }

      //時間を更新
      m_simTime -= Time.deltaTime;
      //停止条件
      if(m_simTime < 0f)
      {
          m_simStart = false;
      }
    }      	
	}

  //シミュレーションを開始
  public void Simulation(float v, float w, float simTime
      , float a1, float a2, float a3, float a4, float a5, float a6, Map map)
  {
    m_map = map;
    m_v = v;
    m_w = w;
    m_a1 = a1;
    m_a2 = a2;
    m_a3 = a3;
    m_a4 = a4;
    m_a5 = a5;
    m_a6 = a6;
    m_simTime = simTime;
    m_simStart = true;
  }

  //正規分布
  public float sample_normal_distribution( float b_2)
  {
      float ret = 0f;
      float b = Mathf.Sqrt(b_2);
      for (int i = 0; i < 12; ++i)
      {
          ret += Random.Range(-b, b);
      }
      return ret * 0.5f;
  }

  //三角分布
  public float sample_triangular_distribution( float b_2)
  {
      float b = Mathf.Sqrt(b_2);
      return Mathf.Sqrt(6f) / 2f * (Random.Range(-b, b) + Random.Range(-b, b));
  }
}
