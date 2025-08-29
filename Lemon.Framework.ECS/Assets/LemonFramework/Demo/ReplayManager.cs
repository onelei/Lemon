using System;
using System.Collections.Generic;
using LemonFramework.ECS;
using LemonFrameSync.Logger;
using LemonFramework.ECS.Demo.Systems;

namespace LemonFramework.ECS.Demo
{
    // 录像管理器，统一管理录制和回放功能
    public class ReplayManager
    {
        private RecordSystem _recordSystem;
        private ReplaySystem _replaySystem;
        private ComponentManager _componentManager;
        private EntityManager _entityManager;

        public bool IsRecording => _recordSystem?.IsRecording ?? false;
        public bool IsReplaying => _replaySystem?.IsReplaying ?? false;
        public int CurrentRecordFrame => _recordSystem?.CurrentFrame ?? 0;
        public int CurrentReplayFrame => _replaySystem?.CurrentFrame ?? 0;
        public int RecordedFrameCount => _recordSystem?.RecordedFrameCount ?? 0;
        public int ReplayFrameCount => _replaySystem?.TotalFrames ?? 0;

        public ReplayManager(ComponentManager componentManager, EntityManager entityManager)
        {
            _componentManager = componentManager;
            _entityManager = entityManager;
            
            // 初始化录制系统
            _recordSystem = new RecordSystem();
            _recordSystem.Initialize(componentManager, entityManager);
            
            // 初始化回放系统
            _replaySystem = new ReplaySystem();
            _replaySystem.Initialize(componentManager, entityManager);
        }

        // === 录制控制函数 ===
        
        // 开始录制
        public void StartRecording()
        {
            _recordSystem?.StartRecording();
        }

        // 停止录制
        public void StopRecording()
        {
            _recordSystem?.StopRecording();
        }
        
        // 暂停录制
        public void PauseRecording()
        {
            _recordSystem?.PauseRecording();
        }
        
        // 恢复录制
        public void ResumeRecording()
        {
            _recordSystem?.ResumeRecording();
        }

        // === 回放控制函数 ===
        
        // 开始回放（使用录制系统的数据）
        public void StartReplay()
        {
            if (_recordSystem?.RecordedFrames != null && _recordSystem.RecordedFrames.Count > 0)
            {
                _replaySystem?.SetReplayData(_recordSystem.RecordedFrames);
                _replaySystem?.StartReplay();
            }
            else
            {
                LogManager.Instance.Log("No recorded data to replay!");
            }
        }
        
        // 开始回放（使用指定的数据）
        public void StartReplayWithData(List<FrameData> replayData)
        {
            _replaySystem?.SetReplayData(replayData);
            _replaySystem?.StartReplay();
        }

        // 停止回放
        public void StopReplay()
        {
            _replaySystem?.StopReplay();
        }

        // 暂停回放
        public void PauseReplay()
        {
            _replaySystem?.PauseReplay();
        }

        // 恢复回放
        public void ResumeReplay()
        {
            _replaySystem?.ResumeReplay();
        }

        // 跳转到指定帧
        public void SeekToFrame(int frameNumber)
        {
            _replaySystem?.SeekToFrame(frameNumber);
        }
        
        // 跳转到开始
        public void SeekToStart()
        {
            _replaySystem?.SeekToStart();
        }
        
        // 跳转到结束
        public void SeekToEnd()
        {
            _replaySystem?.SeekToEnd();
        }
        
        // 设置回放速度
        public void SetReplaySpeed(float speed)
        {
            _replaySystem?.SetReplaySpeed(speed);
        }

        // 更新系统（应该在主循环中调用）
        public void Update(TimeData deltaTime)
        {
            _recordSystem?.Update(deltaTime);
            _replaySystem?.Update(deltaTime);
        }

        // === 输入记录函数 ===
        
        // 记录输入
        public void RecordInput(string key, object value)
        {
            _recordSystem?.RecordInput(key, value);
        }

        // 记录键盘输入
        public void RecordKeyInput(string keyName, bool isPressed)
        {
            _recordSystem?.RecordKeyInput(keyName, isPressed);
        }

        // 记录鼠标输入
        public void RecordMouseInput(float x, float y, bool leftButton, bool rightButton)
        {
            _recordSystem?.RecordMouseInput(x, y, leftButton, rightButton);
        }
        
        // 记录自定义输入
        public void RecordCustomInput(string inputName, object value)
        {
            _recordSystem?.RecordCustomInput(inputName, value);
        }

        // === 文件操作函数 ===
        
        // 保存录制数据到文件
        public void SaveRecordToFile(string filePath)
        {
            _recordSystem?.SaveReplayToFile(filePath);
        }
        
        // 保存回放数据到文件
        public void SaveReplayToFile(string filePath)
        {
            _replaySystem?.SaveReplayToFile(filePath);
        }

        // 从文件加载到录制系统
        public void LoadRecordFromFile(string filePath)
        {
            _recordSystem?.LoadReplayFromFile(filePath);
        }
        
        // 从文件加载到回放系统
        public void LoadReplayFromFile(string filePath)
        {
            _replaySystem?.LoadReplayFromFile(filePath);
        }

        // 清空录制数据
        public void ClearRecordData()
        {
            _recordSystem?.ClearReplayData();
        }
        
        // 清空回放数据
        public void ClearReplayData()
        {
            _replaySystem?.ClearReplayData();
        }

        // === 状态查询函数 ===
        
        // 获取录制进度
        public float GetRecordingProgress()
        {
            return _recordSystem?.GetRecordingProgress() ?? 0f;
        }
        
        // 获取回放进度（0-1）
        public float GetReplayProgress()
        {
            return _replaySystem?.GetReplayProgress() ?? 0f;
        }
        
        // 获取当前帧输入数据
        public Dictionary<string, object> GetCurrentFrameInputs()
        {
            return _replaySystem?.GetCurrentFrameInputs() ?? new Dictionary<string, object>();
        }
        
        // 获取指定帧数据
        public FrameData? GetFrameData(int frameNumber)
        {
            return _replaySystem?.GetFrameData(frameNumber);
        }
    }
}