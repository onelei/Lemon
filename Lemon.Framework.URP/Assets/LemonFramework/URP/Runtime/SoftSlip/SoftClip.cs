using UnityEngine;
using UnityEngine.UI;

namespace Lemon.Framework.URP
{
    [RequireComponent(typeof(Image))]
    public class SoftClip : MonoBehaviour
    {
        /// <summary>
        /// 默认裁剪区域
        /// </summary>
        public Rect clipRect = new Rect(0, 0, 1, 1); // left, bottom, right, top
        /// <summary>
        /// 默认软裁切范围
        /// </summary>
        public float Softness = 0.1f;
        private Material material;

        void Awake()
        {
            // Get or create the material
            var graphic = GetComponent<Image>();
            if (graphic.material == null)
            {
                material = new Material(Shader.Find("Custom/SoftClipShader"));
                graphic.material = material;
            }
            else
            {
                material = graphic.material;
            }
        }

        void Update()
        {
            // Update the clip rect in the shader
            if (material != null)
            {
                material.SetVector("_ClipRect", new Vector4(clipRect.x, clipRect.y, clipRect.width, clipRect.height));
                material.SetFloat("_Softness", Softness);
            }
        }
        
        // void OnValidate()
        // {
        //     Update();
        // }
    }
}