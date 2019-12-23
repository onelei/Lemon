using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon.UI
{
    public class UIBase : MonoClass
    {
        public virtual bool IsCanEnter() { return true; }

        public virtual void OnEnter() { }

        public virtual void OnPause() { }

        public virtual void OnResume() { }

        public virtual void OnExit() { }

    }
}
