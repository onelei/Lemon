using System.Collections.Generic;
using UnityEngine;

namespace LemonFramework.ECS.Replay
{
    /// <summary>
    /// 录像播放器，支持播放、暂停、跳帧、加速等功能
    /// </summary>
    public class ReplayPlayer
    {
        private World _world;
        private ReplayData _currentReplay;
        private bool _isPlaying;
        private int _currentFrameIndex;
        private List<int> _frameNumbers;
        
        /// <summary>
        /// 播放速度倍数（1.0 = 正常速度）
        /// </summary>
        public float PlaybackSpeed { get; set; } = 1.0f;
        
        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool Loop { get; set; } = false;
        
        /// <summary>
        /// 是否正在播放
        /// </summary>
        public bool IsPlaying => _isPlaying;
        
        /// <summary>
        /// 当前帧索引
        /// </summary>
        public int CurrentFrameIndex => _currentFrameIndex;
        
        /// <summary>
        /// 当前帧号
        /// </summary>
        public int CurrentFrameNumber => _frameNumbers != null && _currentFrameIndex < _frameNumbers.Count 
            ? _frameNumbers[_currentFrameIndex] 
            : 0;
        
        /// <summary>
        /// 总帧数
        /// </summary>
        public int TotalFrames => _currentReplay?.TotalFrames ?? 0;
        
        /// <summary>
        /// 当前播放进度（0.0 - 1.0）
        /// </summary>
        public float Progress => TotalFrames > 0 ? (float)_currentFrameIndex / TotalFrames : 0;
        
        /// <summary>
        /// 当前播放时间（秒）
        /// </summary>
        public float CurrentTime => _currentReplay != null ? _currentFrameIndex / (float)_currentReplay.TargetFrameRate : 0;
        
        /// <summary>
        /// 总时长（秒）
        /// </summary>
        public float TotalDuration => _currentReplay?.Duration ?? 0;
        
        public ReplayPlayer(World world)
        {
            _world = world;
        }
        
        /// <summary>
        /// 加载录像数据
        /// </summary>
        public void LoadReplay(ReplayData replay)
        {
            if (_isPlaying)
            {
                Stop();
            }
            
            _currentReplay = replay;
            _frameNumbers = replay.GetAllFrameNumbers();
            _currentFrameIndex = 0;
            
            Debug.Log($"<color=cyan>录像已加载: {replay.ReplayName}, 共 {replay.TotalFrames} 帧</color>");
        }
        
        /// <summary>
        /// 从文件加载录像
        /// </summary>
        public void LoadReplayFromFile(string filePath)
        {
            var replay = ReplayData.LoadFromFile(filePath);
            LoadReplay(replay);
        }
        
        /// <summary>
        /// 开始播放
        /// </summary>
        public void Play()
        {
            if (_currentReplay == null)
            {
                Debug.LogError("没有加载录像数据！");
                return;
            }
            
            _isPlaying = true;
            Debug.Log("<color=cyan>开始播放录像</color>");
        }
        
        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause()
        {
            _isPlaying = false;
            Debug.Log("<color=yellow>录像已暂停</color>");
        }
        
        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _currentFrameIndex = 0;
            Debug.Log("<color=yellow>录像已停止</color>");
        }
        
        /// <summary>
        /// 更新播放（需要在每一帧调用）
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void Update(float deltaTime)
        {
            if (!_isPlaying || _currentReplay == null || _frameNumbers == null)
                return;
            
            // 根据播放速度计算应该前进多少帧
            float frameAdvance = deltaTime * _currentReplay.TargetFrameRate * PlaybackSpeed;
            int framesToAdvance = Mathf.RoundToInt(frameAdvance);
            
            if (framesToAdvance > 0)
            {
                _currentFrameIndex += framesToAdvance;
                
                // 检查是否到达结尾
                if (_currentFrameIndex >= _frameNumbers.Count)
                {
                    if (Loop)
                    {
                        _currentFrameIndex = 0;
                    }
                    else
                    {
                        _currentFrameIndex = _frameNumbers.Count - 1;
                        Stop();
                        Debug.Log("<color=cyan>录像播放完成</color>");
                        return;
                    }
                }
                
                // 恢复当前帧的快照
                var frameNumber = _frameNumbers[_currentFrameIndex];
                var snapshot = _currentReplay.GetFrame(frameNumber);
                if (snapshot != null)
                {
                    _world.RestoreFromSnapshot(snapshot);
                }
            }
        }
        
        /// <summary>
        /// 跳转到指定帧索引
        /// </summary>
        public void SeekToFrameIndex(int frameIndex)
        {
            if (_currentReplay == null || _frameNumbers == null)
            {
                Debug.LogError("没有加载录像数据！");
                return;
            }
            
            _currentFrameIndex = Mathf.Clamp(frameIndex, 0, _frameNumbers.Count - 1);
            
            var frameNumber = _frameNumbers[_currentFrameIndex];
            var snapshot = _currentReplay.GetFrame(frameNumber);
            if (snapshot != null)
            {
                _world.RestoreFromSnapshot(snapshot);
                Debug.Log($"<color=cyan>跳转到帧 {frameNumber} (索引 {_currentFrameIndex})</color>");
            }
        }
        
        /// <summary>
        /// 跳转到指定帧号
        /// </summary>
        public void SeekToFrame(int frameNumber)
        {
            if (_currentReplay == null || _frameNumbers == null)
            {
                Debug.LogError("没有加载录像数据！");
                return;
            }
            
            // 找到最接近的帧索引
            int closestIndex = 0;
            int minDiff = int.MaxValue;
            
            for (int i = 0; i < _frameNumbers.Count; i++)
            {
                int diff = Mathf.Abs(_frameNumbers[i] - frameNumber);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }
            
            SeekToFrameIndex(closestIndex);
        }
        
        /// <summary>
        /// 跳转到指定进度（0.0 - 1.0）
        /// </summary>
        public void SeekToProgress(float progress)
        {
            if (_currentReplay == null || _frameNumbers == null)
            {
                Debug.LogError("没有加载录像数据！");
                return;
            }
            
            progress = Mathf.Clamp01(progress);
            int targetIndex = Mathf.RoundToInt(progress * (_frameNumbers.Count - 1));
            SeekToFrameIndex(targetIndex);
        }
        
        /// <summary>
        /// 跳转到指定时间（秒）
        /// </summary>
        public void SeekToTime(float time)
        {
            if (_currentReplay == null)
            {
                Debug.LogError("没有加载录像数据！");
                return;
            }
            
            float progress = time / _currentReplay.Duration;
            SeekToProgress(progress);
        }
        
        /// <summary>
        /// 下一帧
        /// </summary>
        public void StepForward()
        {
            if (_currentFrameIndex < _frameNumbers.Count - 1)
            {
                SeekToFrameIndex(_currentFrameIndex + 1);
            }
        }
        
        /// <summary>
        /// 上一帧
        /// </summary>
        public void StepBackward()
        {
            if (_currentFrameIndex > 0)
            {
                SeekToFrameIndex(_currentFrameIndex - 1);
            }
        }
        
        /// <summary>
        /// 设置播放速度
        /// </summary>
        public void SetPlaybackSpeed(float speed)
        {
            PlaybackSpeed = Mathf.Max(0.1f, speed);
            Debug.Log($"<color=cyan>播放速度设置为: {PlaybackSpeed}x</color>");
        }
    }
}

