using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Th : MonoBehaviour
{
    public GameObject sonBomb;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDestroy(){
        Vector3 posicao = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        Instantiate(sonBomb, posicao, Quaternion.identity);
    }
}
