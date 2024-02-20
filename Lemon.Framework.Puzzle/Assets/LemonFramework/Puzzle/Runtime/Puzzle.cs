using System.Collections.Generic;
using LemonFramework.Puzzle;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lemon.Framework.Puzzle
{
    public class Puzzle : MonoBehaviour
    {
        private readonly Vector2 _leftTopPivot = new Vector2(0, 1);
        private readonly Vector2 _centerPivot = new Vector2(0.5f, 0.5f);
        
        [SerializeField] private Sprite mParent;
        [SerializeField] private List<Sprite> mChildren = new List<Sprite>();
    
        private void Generate(Sprite sprite, Vector2 position)
        {
            var go = new GameObject($"Image_{sprite.name}");
            go.transform.SetParent(transform);
            go.AddComponent<Image>().sprite = sprite;
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.pivot = _leftTopPivot;
            rectTransform.anchorMin = _leftTopPivot;
            rectTransform.anchorMax = _leftTopPivot;
            rectTransform.anchoredPosition3D = new Vector3(position.x, -position.y, 0);
            rectTransform.sizeDelta = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
            //adjust pivot
            var oldPos = rectTransform.position;
            var oldPivot = rectTransform.pivot;
            var rect = rectTransform.rect;
            var deltaPivotX = rect.width * (_centerPivot.x - oldPivot.x);
            var deltaPivotY = rect.height * (_centerPivot.y - oldPivot.y);
            var deltaPivot = new Vector3(deltaPivotX, deltaPivotY, 0);
            rectTransform.pivot = _centerPivot;
            rectTransform.anchorMin = _centerPivot;
            rectTransform.anchorMax = _centerPivot;
            rectTransform.position = oldPos + deltaPivot;
        }

        public void GenerateAll()
        {
            var parentAssetPath = AssetDatabase.GetAssetPath(mParent);
            foreach (var child in mChildren)
            {
                var assetPath = AssetDatabase.GetAssetPath(child);
                // if(PuzzleHelper.TryGetRelativePosition(parentAssetPath, assetPath, out var position))
                // {
                //     Generate(child, position);
                // }
                if(PuzzleHelper.TryGetTransparentRelativePosition(parentAssetPath, assetPath, out var position))
                {
                    Generate(child, position);
                }
            }
        }
    }
}
