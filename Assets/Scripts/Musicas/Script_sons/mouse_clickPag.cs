using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class test_clickPag : MonoBehaviour
{
    public GameObject clickpag;
    void Start()
    {
        
    }

    
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 posicao = new Vector3(this.gameObject.transform.position.x,this.gameObject.transform.position.y,this.gameObject.transform.position.z);
            Instantiate(clickpag, posicao, Quaternion.identity);
        }
    }
}
