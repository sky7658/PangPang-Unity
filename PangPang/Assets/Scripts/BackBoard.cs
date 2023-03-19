using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    float interval = 3.9f;
    void Start()
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                var back = Instantiate(prefab);
                back.transform.position = new Vector2((interval / 3 * j) - interval, (-i * interval / 3) + interval);
            }
        }
        
    }

    void Update()
    {
        
    }
}
