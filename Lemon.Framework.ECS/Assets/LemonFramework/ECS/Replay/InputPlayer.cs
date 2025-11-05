using UnityEngine;

namespace LemonFramework.ECS.Replay
{
    /// <summary>
    /// 基于输入的录像播放器
    /// 通过重放输入并执行System来重现游戏过程
    /// </summary>
    public class InputPlayer
    {
        private World _world;
        private InputReplayData _currentReplay;
        private bool _isPlaying;
        private int _currentFrame;
        private TimeData _playbackTimeData;
        private float _accumulatedTime;
        
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
        /// 当前帧号
        /// </summary>
        public int CurrentFrame => _currentFrame;
        
        /// <summary>
        /// 开始帧号
        /// </summary>
        public int StartFrame => _currentReplay?.StartFrame ?? 0;
        
        /// <summary>
        /// 结束帧号
        /// </summary>
        public int EndFrame => _currentReplay?.EndFrame ?? 0;
        
        /// <summary>
        /// 总帧数
        /// </summary>
        public int TotalFrames => _currentReplay?.TotalFrames ?? 0;
        
        /// <summary>
        /// 当前播放进度（0.0 - 1.0）
        /// </summary>
        public float Progress
        {
            get
            {
                if (TotalFrames <= 0) return 0;
                return Mathf.Clamp01((float)(_currentFrame - StartFrame) / TotalFrames);
            }
        }
        
        /// <summary>
        /// 当前播放时间（秒）
        /// </summary>
        public float CurrentTime => _currentReplay != null ? (_currentFrame - StartFrame) / (float)_currentReplay.TargetFrameRate : 0;
        
        /// <summary>
        /// 总时长（秒）
        /// </summary>
        public float TotalDuration => _currentReplay?.Duration ?? 0;
        
        /// <summary>
        /// 当前帧的输入
        /// </summary>
        public FrameInput CurrentInput { get; private set; }
        
        public InputPlayer(World world)
        {
            _world = world;
            _playbackTimeData = new TimeData(60);
        }
        
        /// <summary>
        /// 加载录像数据
        /// </summary>
        public void LoadReplay(InputReplayData replay)
        {
            if (_isPlaying)
            {
                Stop();
            }
            
            _currentReplay = replay;
            _currentFrame = replay.StartFrame;
            _playbackTimeData = new TimeData(replay.TargetFrameRate);
            
            // 恢复到初始状态
            if (replay.InitialSnapshot != null)
            {
                _world.RestoreFromSnapshot(replay.InitialSnapshot);
            }
            
            Debug.Log($"<color=cyan>录像已加载: {replay.ReplayName}</color>");
            Debug.Log($"<color=cyan>总帧数: {replay.TotalFrames}, 输入帧数: {replay.InputFrameCount}</color>");
        }
        
        /// <summary>
        /// 从文件加载录像
        /// </summary>
        public void LoadReplayFromFile(string filePath)
        {
            var replay = InputReplayData.LoadFromFile(filePath);
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
            _accumulatedTime = 0;
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
            _currentFrame = _currentReplay?.StartFrame ?? 0;
            _accumulatedTime = 0;
            
            // 恢复到初始状态
            if (_currentReplay?.InitialSnapshot != null)
            {
                _world.RestoreFromSnapshot(_currentReplay.InitialSnapshot);
            }
            
            Debug.Log("<color=yellow>录像已停止</color>");
        }
        
        /// <summary>
        /// 更新播放（需要在每一帧调用）
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void Update(float deltaTime)
        {
            if (!_isPlaying || _currentReplay == null)
                return;
            
            _accumulatedTime += deltaTime * PlaybackSpeed;
            float frameTime = 1f / _currentReplay.TargetFrameRate;
            
            // 执行累积的帧
            while (_accumulatedTime >= frameTime)
            {
                _accumulatedTime -= frameTime;
                
                // 获取当前帧的输入
                CurrentInput = _currentReplay.GetInput(_currentFrame);
                
                // 更新TimeData
                _playbackTimeData.Frame = _currentFrame;
                
                // 执行ECS系统更新（由System根据输入计算状态）
                _world.Update(_playbackTimeData);
                
                _currentFrame++;
                
                // 检查是否到达结尾
                if (_currentFrame > _currentReplay.EndFrame)
                {
                    if (Loop)
                    {
                        SeekToFrame(_currentReplay.StartFrame);
                    }
                    else
                    {
                        Stop();
                        Debug.Log("<color=cyan>录像播放完成</color>");
                        return;
                    }
                }
            }
        }
        
        /// <summary>
        /// 跳转到指定帧
        /// </summary>
        public void SeekToFrame(int frameNumber)
        {
            if (_currentReplay == null)
            {
                Debug.LogError("没有加载录像数据！");
                return;
            }
            
            frameNumber = Mathf.Clamp(frameNumber, _currentReplay.StartFrame, _currentReplay.EndFrame);
            
            // 恢复到初始状态
            _world.RestoreFromSnapshot(_currentReplay.InitialSnapshot);
            _currentFrame = _currentReplay.StartFrame;
            _playbackTimeData.Frame = _currentFrame;
            
            // 快进到目标帧
            while (_currentFrame < frameNumber)
            {
                CurrentInput = _currentReplay.GetInput(_currentFrame);
                _playbackTimeData.Frame = _currentFrame;
                _world.Update(_playbackTimeData);
                _currentFrame++;
            }
            
            Debug.Log($"<color=cyan>跳转到帧 {frameNumber}</color>");
        }
        
        /// <summary>
        /// 跳转到指定进度（0.0 - 1.0）
        /// </summary>
        public void SeekToProgress(float progress)
        {
            if (_currentReplay == null)
            {
                Debug.LogError("没有加载录像数据！");
                return;
            }
            
            progress = Mathf.Clamp01(progress);
            int targetFrame = _currentReplay.StartFrame + Mathf.RoundToInt(progress * _currentReplay.TotalFrames);
            SeekToFrame(targetFrame);
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
            if (_currentFrame < EndFrame)
            {
                CurrentInput = _currentReplay.GetInput(_currentFrame);
                _playbackTimeData.Frame = _currentFrame;
                _world.Update(_playbackTimeData);
                _currentFrame++;
            }
        }
        
        /// <summary>
        /// 上一帧（需要从初始状态重新计算）
        /// </summary>
        public void StepBackward()
        {
            if (_currentFrame > StartFrame)
            {
                SeekToFrame(_currentFrame - 1);
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

