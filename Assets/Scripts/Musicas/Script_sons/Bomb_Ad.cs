using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Ad : MonoBehaviour
{
    public GameObject sonBomba;
    void Start()
    {
        
    }


    void Update()
    {
        
    }
    void OnDestroy(){
        Vector3 posicao = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        Instantiate(sonBomba, posicao, Quaternion.identity);
    }
}
