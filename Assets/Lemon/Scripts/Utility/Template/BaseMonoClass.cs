using UnityEngine;

namespace Lemon
{
    public class BaseMonoClass : MonoBehaviour
    {
        private GameObject _CacheGameObject = null;
        public GameObject CacheGameObject { get { if (_CacheGameObject == null) { _CacheGameObject = gameObject; } return _CacheGameObject; } }

        private Transform _CacheTransform = null;
        public Transform CacheTransform { get { if (_CacheTransform == null) { _CacheTransform = transform; } return _CacheTransform; } }

        public bool activeInHierarchy { get { return CacheGameObject.activeInHierarchy; } }
        public bool activeSelf { get { return CacheGameObject.activeSelf; } }
    }
}
