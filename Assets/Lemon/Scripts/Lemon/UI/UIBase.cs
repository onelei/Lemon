using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Lemon.UI
{
    public class UIBase : MonoClass
    {
        private GameObject _CacheGameObject = null;
        public GameObject CacheGameObject { get { if (_CacheGameObject == null) { _CacheGameObject = gameObject; } return _CacheGameObject; } }

        private Transform _CacheTransform = null;
        public Transform CacheTransform { get { if (_CacheTransform == null) { _CacheTransform = transform; } return _CacheTransform; } }

        public bool activeInHierarchy { get { return CacheGameObject.activeInHierarchy; } }
        public bool activeSelf { get { return CacheGameObject.activeSelf; } }

        public virtual bool IsCanEnter() { return true; }

        public virtual void OnEnter() { }

        public virtual void OnPause() { }

        public virtual void OnResume() { }

        public virtual void OnExit() { }

    }
}
