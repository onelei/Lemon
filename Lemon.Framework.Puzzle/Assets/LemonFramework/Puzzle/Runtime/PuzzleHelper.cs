using OpenCvSharp;
using OpenCvSharp.Features2D;
using UnityEngine;
using UnityEngine.Diagnostics;
using Rect = OpenCvSharp.Rect;

namespace LemonFramework.Puzzle
{
    public static class PuzzleHelper
    {
        public static Mat UnityTextureToMat(Texture2D texture)
        {
            // Convert Texture2D to byte array
            byte[] bytes = texture.GetRawTextureData();

            // Create Mat from byte array
            Mat mat = new Mat(texture.height, texture.width, MatType.CV_8UC4, bytes);

            return mat;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentTextureFilePath">"Assets/***/***.png"</param>
        /// <param name="textureFilePath">"Assets/***/***.png"</param>
        /// <param name="relativePosition"></param>
        /// <returns></returns>
        public static bool TryGetRelativePosition(string parentTextureFilePath, string textureFilePath, out Vector2 relativePosition)
        {
            // 加载图像A和图像B
            Mat matA = Cv2.ImRead(parentTextureFilePath);
            Mat matB = Cv2.ImRead(textureFilePath);

            // 创建一个匹配结果的Mat
            Mat result = new Mat();

            // 执行模板匹配
            Cv2.MatchTemplate(matA, matB, result, TemplateMatchModes.CCoeffNormed);

            // 获取匹配结果中最大值的位置
            Cv2.MinMaxLoc(result, out _, out _, out Point minLoc, out Point maxLoc);
            relativePosition = new Vector2(maxLoc.X, maxLoc.Y);
            return true;
        }
         
        public static bool TryGetTransparentRelativePosition(string parentTextureFilePath, string textureFilePath, out Vector2 relativePosition)
        {
            // 加载图像A和图像B
            Mat sourceImage = Cv2.ImRead(parentTextureFilePath);
            Mat templateImage = Cv2.ImRead(textureFilePath);
            // 转换图像为灰度
            Mat sourceGray = new Mat();
            Mat templateGray = new Mat();
            Cv2.CvtColor(sourceImage, sourceGray, ColorConversionCodes.BGRA2GRAY);
            Cv2.CvtColor(templateImage, templateGray, ColorConversionCodes.BGRA2GRAY);

            // 进行模板匹配
            Mat result = new Mat();
            Cv2.MatchTemplate(sourceGray, templateGray, result, TemplateMatchModes.CCoeffNormed);

            // 获取最佳匹配位置
            Cv2.MinMaxLoc(result, out _, out double maxVal, out Point minLoc, out Point maxLoc2);

            // 绘制矩形框表示匹配位置
            Point maxLoc = new Point(minLoc.X + templateImage.Width, minLoc.Y + templateImage.Height);
            Cv2.Rectangle(sourceImage, minLoc, maxLoc, Scalar.Red, 2);

            relativePosition = Vector2.one;//new Vector2(maxLoc.X, maxLoc.Y);
            return true;
        }
    }
}