using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll_gemas : MonoBehaviour
{
    public GameObject scrollgemas;
    void Start()
    {
        
    }

    
    void Update()
    {
        
        
    }
    void OnDestroy(){
        if(Application.isPlaying && scrollgemas != null){
            Vector3 scroll = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
            Instantiate(scrollgemas, scroll, Quaternion.identity);
        }
        
    }
}
