using Lemon.Framework.DiskSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Lemon.Framework.Samples.DiskSpace
{
    public class DiskSpaceSample : MonoBehaviour
    {
        public Text text;

        void Start()
        {
            var availableFreeSpace = DiskSpaceUtility.GetAvailableFreeSpace(Application.persistentDataPath);
            var availableFreeSpaceText =
                $"{Application.persistentDataPath}\n\nAvailable Free Space: {availableFreeSpace}";
            Debug.Log(availableFreeSpaceText);
            text.text = availableFreeSpaceText;
        }
    }
}