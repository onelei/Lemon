using LemonFramework.Format;
using TMPro;
using UnityEngine;

namespace LemonFramework.Samples.Format
{
    public class NumberFormatterSample : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public long _number = 1234567890;

        void Update()
        {
            //_number += 50;
            text.SetText(NumberFormatter.GetSplitNumber(_number, 4));
        }
    }
}