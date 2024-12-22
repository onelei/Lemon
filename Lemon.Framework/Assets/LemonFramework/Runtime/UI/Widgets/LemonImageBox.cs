/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/

using UnityEngine;
using UnityEngine.UI;

namespace LemonFramework.UI.Widgets
{
    public class LemonImageBox : Graphic
    {
        private GameObject _CacheGameObject = null;
        public GameObject CacheGameObject { get { if (_CacheGameObject == null) { _CacheGameObject = gameObject; } return _CacheGameObject; } }

        private Transform _CacheTransform = null;
        public Transform CacheTransform { get { if (_CacheTransform == null) { _CacheTransform = transform; } return _CacheTransform; } }

        public override void Rebuild(CanvasUpdate update)
        {
            
        }
    }
} 
