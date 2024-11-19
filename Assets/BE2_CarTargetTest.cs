using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Environment
{
    public class BE2_CarTargetTest : BE2_TargetObject
    {
        GameObject _bullet;

        public new Transform Transform => transform;

        void Awake()
        {
            // v2.6 - changed way to find "bullet" child of Target Object
            foreach (Transform child in transform)
            {
                if (child.name == "Bullet")
                    _bullet = child.gameObject;
            }

        }

        //void Start()
        //{
        //
        //}

        //void Update()
        //{
        //
        //}

        public void SetMotorPower(float powerPercentage)
        {
            // 
        }
    }
}