using UnityEngine;
using System.Collections.Generic;

public class RobotVelocity : MonoBehaviour {

    //制御パラメータ
    public float m_v = 5f; //並進速度　速度
    public float m_w = 1f; //回転速度　角度(deg)

    public float m_x = 0f;
    public float m_y = 0f;
    public float m_theta = 0f;

    public float m_simTime = 1f;

    public int m_sampleCount = 0;
    public int m_sampleNum = 500;

    //動作の雑音パラメータ
    public float m_a1 = 0.1f;
    public float m_a2 = 0.1f;
    public float m_a3 = 0.1f;
    public float m_a4 = 0.1f;
    public float m_a5 = 0.1f;
    public float m_a6 = 0.1f;
    
    public GameObject m_sampleObjPrefab = null;
    public GameObject m_robotObjPrefab = null;
    List<GameObject> m_sampleObjs = new List<GameObject>();

    // Use this for initialization
    void Start () {

        if (m_sampleObjPrefab == null)
            Debug.LogError("no prefab");
        if (m_robotObjPrefab == null)
            Debug.LogError("no prefab");
    }

    // Update is called once per frame
    void Update () {

        //一個サンプルを追加する
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //シミュレーション用の個体を追加
            GameObject sampleObj = Instantiate(
                m_sampleObjPrefab, 
                transform.position + Vector3.back, 
                transform.rotation) as GameObject;
            //シミュレーションを開始
            sampleObj.GetComponent<SampleVelocity>().Simulation(
                m_v, m_w, m_simTime,
                m_a1, m_a2, m_a3, m_a4, m_a5, m_a6);
            //名前変更
            sampleObj.name = "sammple" + m_sampleCount.ToString();
            //サンプル数を更新
            m_sampleCount += 1;
            //リストに追加
            m_sampleObjs.Add(sampleObj);
        }

        //ノイズのない場合の挙動を作る
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ノイズのないシミュレーション用の個体を追加
            GameObject sampleObj = Instantiate(
                m_robotObjPrefab,
                transform.position,
                transform.rotation) as GameObject;
            //シミュレーションを開始
            sampleObj.GetComponent<SampleVelocity>().Simulation(
                m_v, m_w, m_simTime,
                0f, 0f, 0f, 0f, 0f, 0f);
            //名前変更
            sampleObj.name = "robot(sim)";
            //リストに追加
            m_sampleObjs.Add(sampleObj);
        }

        //指定した数だけばらまく
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for(int idx = 0; idx < m_sampleNum; ++idx)
            {
                //シミュレーション用の個体を追加
                GameObject sampleObj = Instantiate(
                    m_sampleObjPrefab,
                    transform.position + Vector3.back,
                    transform.rotation) as GameObject;
                //シミュレーションを開始
                sampleObj.GetComponent<SampleVelocity>().Simulation(
                    m_v, m_w, m_simTime,
                    m_a1, m_a2, m_a3, m_a4, m_a5, m_a6);
                //名前変更
                sampleObj.name = "sammple" + m_sampleCount.ToString();
                //サンプル数を更新
                m_sampleCount += 1;
                //リストに追加
                m_sampleObjs.Add(sampleObj);
            }
        }

        //削除
        if(Input.GetKeyDown(KeyCode.D))
        {
            deleteSampleObjs();
        }
    }

    void deleteSampleObjs()
    {
        for (int idx = 0; idx < m_sampleObjs.Count; ++idx)
        {
            Destroy(m_sampleObjs[idx]);
        }
        m_sampleObjs.Clear();
        m_sampleCount = 0;
    }
}
