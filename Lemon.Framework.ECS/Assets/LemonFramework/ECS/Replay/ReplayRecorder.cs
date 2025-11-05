using LemonFramework.ECS.Snapshot;
using UnityEngine;

namespace LemonFramework.ECS.Replay
{
    /// <summary>
    /// 录像录制器，负责录制游戏过程
    /// </summary>
    public class ReplayRecorder
    {
        private World _world;
        private ReplayData _currentReplay;
        private bool _isRecording;
        
        /// <summary>
        /// 是否正在录制
        /// </summary>
        public bool IsRecording => _isRecording;
        
        /// <summary>
        /// 当前录像数据
        /// </summary>
        public ReplayData CurrentReplay => _currentReplay;
        
        /// <summary>
        /// 已录制的帧数
        /// </summary>
        public int RecordedFrames => _currentReplay?.TotalFrames ?? 0;
        
        public ReplayRecorder(World world)
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
            
            _currentReplay = new ReplayData(replayName, targetFrameRate);
            _isRecording = true;
            
            Debug.Log($"<color=green>开始录制: {replayName}</color>");
        }
        
        /// <summary>
        /// 停止录制
        /// </summary>
        public ReplayData StopRecording()
        {
            if (!_isRecording)
            {
                Debug.LogWarning("没有正在进行的录制！");
                return null;
            }
            
            _isRecording = false;
            var replay = _currentReplay;
            
            Debug.Log($"<color=green>录制完成: {replay.ReplayName}, 共 {replay.TotalFrames} 帧, 时长 {replay.Duration:F2} 秒</color>");
            
            return replay;
        }
        
        /// <summary>
        /// 记录当前帧（需要在每一帧调用）
        /// </summary>
        public void RecordFrame()
        {
            if (!_isRecording || _currentReplay == null)
                return;
            
            var snapshot = _world.CreateSnapshot();
            _currentReplay.AddFrame(snapshot);
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
            Debug.Log($"<color=green>录像已保存到: {filePath}</color>");
        }
    }
}

