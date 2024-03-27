using System;
using System.Collections.Generic;
using Omnix.CharaCon.Collections;
using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    [Serializable]
    public class ConnectedObjects
    {
        public List<GameObject> activate;
        public List<GameObject> deactivate;

        public void Set(bool value) 
        {
            activate.SetActive(value);
            deactivate.SetActive(!value);   
        }
    }
}