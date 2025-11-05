using LemonFramework.ECS.Snapshot;
using UnityEngine;

namespace LemonFramework.ECS.Replay
{
    /// <summary>
    /// 基于输入的录像录制器
    /// 只记录玩家输入，大幅减小文件大小
    /// </summary>
    public class InputRecorder
    {
        private World _world;
        private InputReplayData _currentReplay;
        private bool _isRecording;
        private int _currentFrame;
        
        /// <summary>
        /// 是否正在录制
        /// </summary>
        public bool IsRecording => _isRecording;
        
        /// <summary>
        /// 当前录像数据
        /// </summary>
        public InputReplayData CurrentReplay => _currentReplay;
        
        /// <summary>
        /// 已录制的总帧数
        /// </summary>
        public int RecordedFrames => _currentReplay != null ? _currentReplay.TotalFrames : 0;
        
        /// <summary>
        /// 已记录的输入帧数
        /// </summary>
        public int InputFrameCount => _currentReplay?.InputFrameCount ?? 0;
        
        public InputRecorder(World world)
        {
            _world = world;
        }
        
        /// <summary>
        /// 开始录制
        /// </summary>
        /// <param name="replayName">录像名称</param>
        /// <param name="targetFrameRate">目标帧率</param>
        public void StartRecording(string replayName, int targetFrameRate = 60)
        {
            if (_isRecording)
            {
                Debug.LogWarning("已经在录制中！");
                return;
            }
            
            // 创建初始快照
            var initialSnapshot = _world.CreateSnapshot();
            _currentReplay = new InputReplayData(replayName, targetFrameRate, initialSnapshot);
            _currentFrame = _world.CurrentFrame;
            _isRecording = true;
            
            Debug.Log($"<color=green>开始录制(输入模式): {replayName}</color>");
            Debug.Log($"<color=green>初始帧: {_currentFrame}</color>");
        }
        
        /// <summary>
        /// 停止录制
        /// </summary>
        public InputReplayData StopRecording()
        {
            if (!_isRecording)
            {
                Debug.LogWarning("没有正在进行的录制！");
                return null;
            }
            
            _isRecording = false;
            var replay = _currentReplay;
            
            Debug.Log($"<color=green>录制完成: {replay.ReplayName}</color>");
            Debug.Log($"<color=green>总帧数: {replay.TotalFrames}, 输入帧数: {replay.InputFrameCount}</color>");
            Debug.Log($"<color=green>压缩率: {(1f - (float)replay.InputFrameCount / replay.TotalFrames) * 100:F1}%</color>");
            Debug.Log($"<color=green>时长: {replay.Duration:F2} 秒</color>");
            
            return replay;
        }
        
        /// <summary>
        /// 记录当前帧的输入
        /// </summary>
        /// <param name="movement">移动输入</param>
        /// <param name="buttonStates">按键状态</param>
        public void RecordInput(Vector2 movement, int buttonStates = 0)
        {
            if (!_isRecording || _currentReplay == null)
                return;
            
            var input = new FrameInput(_currentFrame, movement, buttonStates);
            _currentReplay.AddInput(input);
            _currentFrame++;
        }
        
        /// <summary>
        /// 取消录制
        /// </summary>
        public void CancelRecording()
        {
            if (_isRecording)
            {
                _isRecording = false;
                _currentReplay?.Clear();
                _currentReplay = null;
                Debug.Log("<color=yellow>录制已取消</color>");
            }
        }
        
        /// <summary>
        /// 保存录像到文件
        /// </summary>
        public void SaveReplay(string filePath)
        {
            if (_currentReplay == null)
            {
                Debug.LogError("没有可保存的录像数据！");
                return;
            }
            
            _currentReplay.SaveToFile(filePath);
            
            // 计算文件大小
            var fileInfo = new System.IO.FileInfo(filePath);
            float fileSizeKB = fileInfo.Length / 1024f;
            
            Debug.Log($"<color=green>录像已保存到: {filePath}</color>");
            Debug.Log($"<color=green>文件大小: {fileSizeKB:F2} KB</color>");
        }
    }
}

