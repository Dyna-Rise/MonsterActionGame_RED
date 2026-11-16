using System.IO.IsolatedStorage;
using UnityEngine;

public class BowArrow : MonoBehaviour
{


    public void Shoot(Vector3 dir, float dmg, GameObject own)
    {
        Rigidbody rbody = obj.GetComponent<Rigidbody>();
        rbody.AddForce(new Vector3(0, 0, arrowSpeed), ForceMode.Impulse);
        return ;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
