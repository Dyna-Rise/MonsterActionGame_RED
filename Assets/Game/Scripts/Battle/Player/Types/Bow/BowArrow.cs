using System.IO.IsolatedStorage;
using UnityEngine;

public class BowArrow : MonoBehaviour
{
    public float arrowSpeed=10.0f; //矢のスピード

    //PlayerTypeB_Bow.csから引数を受け取る
    public void Shoot(Vector3 dir, float dmg, GameObject own,bool flin)
    {
        //矢のRigidbody使って矢を飛ばす
        Rigidbody rbody = GetComponent<Rigidbody>();
        rbody.AddForce(dir*arrowSpeed, ForceMode.Impulse);
        //transform.forward(オブジェクト(ここでは矢)の正面)をdir(プレイヤーの正面)に合わせる
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
