﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPointsTransform : MonoBehaviour
{
    [SerializeField] private float distance = -0.01f;
    
    private Vector3 _scaleChange;
    private Transform _myTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        _scaleChange = new Vector3(distance, -0.01f, distance);
        _myTransform = transform;
        _myTransform.localScale += _scaleChange;
    }
}