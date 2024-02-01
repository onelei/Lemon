using UnityEngine;

namespace Lemon.Framework.UI.Widgets
{
    public class BaseBehavior : MonoBehaviour
    {
        /// <summary>
        /// CacheGameObject
        /// </summary>
        public GameObject CacheGameObject { get { if (_CacheGameObject == null) { _CacheGameObject = gameObject; } return _CacheGameObject; } }
        private GameObject _CacheGameObject = null;

        /// <summary>
        /// CacheTransform
        /// </summary>
        public Transform CacheTransform { get { if (_CacheTransform == null) { _CacheTransform = transform; } return _CacheTransform; } }
        private Transform _CacheTransform = null;

        /// <summary>
        /// activeInHierarchy
        /// </summary>
        public bool activeInHierarchy { get { return CacheGameObject.activeInHierarchy; } }

        /// <summary>
        /// activeSelf
        /// </summary>
        public bool activeSelf { get { return CacheGameObject.activeSelf; } }
    }
}
