using UnityEngine;
using UnityEngine.UI;
using System;

namespace Lemon.UI
{
    [AddComponentMenu("UI/QText", 10)]
    public class QText : Text
    {
        /// <summary>
        /// 多语言key
        /// </summary>
        public string key = string.Empty;

        [HideInInspector]
        public bool bInit = false;

        public override string text
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    if (String.IsNullOrEmpty(m_Text))
                        return;
                    m_Text = "";
                    SetVerticesDirty();
                }
                else if (m_Text != value)
                {
                    m_Text = value;
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

        public void Refresh()
        {
            if (string.IsNullOrEmpty(key))
                return;
            text = LanguageManager.Instance.Get(key);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            bInit = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //Refresh();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}
