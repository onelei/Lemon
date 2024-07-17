using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Lemon.Framework.GPUFingerprint
{
    public class GPUFingerprint : MonoBehaviour
    {
        // Render Texture dimensions
        private const int TextureWidth = 32;
        private const int TextureHeight = 32;

        // Reference to the shader
        public Shader fingerprintShader;
        public Text text;

        // Generated GPU fingerprint
        private string gpuFingerprint;

        void Start()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Create a temporary Render Texture
            RenderTexture renderTexture = new RenderTexture(TextureWidth, TextureHeight, 0, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            // Create a Material using the fingerprint shader
            Material fingerprintMaterial = new Material(fingerprintShader);

            // Render the special pattern to the Render Texture
            Graphics.Blit(null, renderTexture, fingerprintMaterial);

            // Read pixels from the Render Texture
            Texture2D tex = new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, TextureWidth, TextureHeight), 0, 0);
            tex.Apply();

            // Generate fingerprint based on pixels
            var gpuFingerprint = GenerateFingerprint(tex.GetPixels());
            // Clean up
            RenderTexture.active = null;
            Destroy(renderTexture);
            Destroy(tex);
            Destroy(fingerprintMaterial);
            sw.Stop();
            text.text = $"GPU Fingerprint: \n{gpuFingerprint}\n{sw.ElapsedMilliseconds} ms";
            Debug.Log("GPU Fingerprint: " + gpuFingerprint);
        }

        // Generate fingerprint from pixel data using MD5 hash
        private string GenerateFingerprint(Color[] pixels)
        {
            // Convert pixel data to string
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Color pixel in pixels)
            {
                stringBuilder.Append(pixel.r.ToString());
                stringBuilder.Append(pixel.g.ToString());
                stringBuilder.Append(pixel.b.ToString());
            }

            string pixelData = stringBuilder.ToString();

            // Calculate MD5 hash of the pixel data
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(pixelData);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert byte array to hex string
            stringBuilder.Remove(0, stringBuilder.Length);
            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}