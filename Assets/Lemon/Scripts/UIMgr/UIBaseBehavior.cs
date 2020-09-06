using UnityEngine;

namespace Lemon
{ 
    public class UIBaseBehavior : BaseBehavior
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
