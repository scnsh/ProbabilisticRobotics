using UnityEngine;
using System.Collections;

public class SampleOdometry : MonoBehaviour {

  public bool m_simStart = false;
  public float m_simTime = 0f;

  //速度パラメータ(オドメトリを計算するために代用として使う)
  public float m_v = 0f;
  public float m_w = 0f;
  //オドメトリ量
  public float m_rot1 = 0f;
  public float m_trans = 0f;
  public float m_rot2 = 0f;

  //動作の雑音パラメータ
  public float m_a1 = 0f;
  public float m_a2 = 0f;
  public float m_a3 = 0f;
  public float m_a4 = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    
    if(m_simStart)
    {
      //ノイズを含まずに進んだ場合の位置と姿勢を求める
      Vector3 euler = transform.rotation.eulerAngles; 
      float theta = euler.z;
      Vector3 diff = new Vector3(
         -(m_v / m_w) * Mathf.Sin(theta) + (m_v / m_w) * Mathf.Sin(theta + m_w * Time.deltaTime),
         (m_v / m_w) * Mathf.Cos(theta) - (m_v / m_w) * Mathf.Cos(theta + m_w * Time.deltaTime),
         0f);
      //理想値から計算されたオドメトリ(ノイズを含まない)
      m_rot1 = Mathf.Atan2(diff.y, diff.x) - theta;
      m_trans = diff.magnitude;
      m_rot2 = m_w * Time.deltaTime - m_rot1;
      //オドメトリにノイズを含める(理想値 -> 測定値)
      float rot1 = m_rot1 - sample_normal_distribution(m_a1 * m_rot1 * m_rot1 + m_a2 * m_trans * m_trans);
      float trans = m_trans - sample_normal_distribution(m_a3 * m_trans * m_trans + m_a4 * m_rot1 * m_rot1 + m_a4 * m_rot2 * m_rot2);
      float rot2 = m_rot2 - sample_normal_distribution(m_a1 * m_rot2 * m_rot2 + m_a2 * m_trans * m_trans);

      //位置と姿勢をオドメトリ基準で更新
      transform.position = new Vector3(
        transform.position.x + trans * Mathf.Cos(theta + rot1),
        transform.position.y + trans * Mathf.Sin(theta + rot1),
        transform.position.z);
      transform.rotation = Quaternion.AngleAxis(theta + rot1 + rot2, Vector3.forward);

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
      , float a1, float a2, float a3, float a4)
  {
      m_v = v;
      m_w = w;
      m_a1 = a1;
      m_a2 = a2;
      m_a3 = a3;
      m_a4 = a4;
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
