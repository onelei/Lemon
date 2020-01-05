using UnityEngine;

namespace Lemon
{
    public class BaseMonoClass : MonoBehaviour
    {
        private GameObject _CacheGameObject = null;
        public GameObject CacheGameObject { get { if (_CacheGameObject == null) { _CacheGameObject = gameObject; } return _CacheGameObject; } }

        private Transform _CacheTransform = null;
        public Transform CacheTransform { get { if (_CacheTransform == null) { _CacheTransform = transform; } return _CacheTransform; } }
    }

    public class BaseMonoUIClass : BaseMonoClass
    { 
        protected virtual void MonoStart() { }
        protected virtual void MonoUpdate() { }
        protected virtual void MonoDestroy() { }

        private void Start()
        {
            MonoStart();
        }

        private void Update()
        {
            MonoUpdate();
        }

        private void OnDestroy()
        {
            MonoDestroy();
        }

    }
}
