using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public GameObject Player;
    public Vector3 offset = new Vector3(-1.3f, -3f, 13f);
    public float followSpeed = 3f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Player.transform.position - offset, followSpeed * Time.deltaTime);
    }
}
