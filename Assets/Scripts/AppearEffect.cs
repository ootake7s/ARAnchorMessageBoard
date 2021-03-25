﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearEffect : MonoBehaviour
{

    //　出現させるエフェクト
    [SerializeField]
    private GameObject effectObject;
    //　エフェクトを消す秒数
    [SerializeField]
    private float deleteTime;
    //　エフェクトの出現位置のオフセット値
    [SerializeField]
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
      //　ゲームオブジェクト登場時にエフェクトをインスタンス化
      var instantiateEffect = GameObject.Instantiate (effectObject, transform.position + new Vector3(0f, offset, 0f), Quaternion.identity) as GameObject;
      Destroy (instantiateEffect, deleteTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
