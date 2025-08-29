using System;
using System.Collections.Generic;
using System.IO;
using LemonFramework.ECS;
using LemonFrameSync.Logger;
using LemonFramework.ECS.Demo.Components;
using Newtonsoft.Json;

namespace LemonFramework.ECS.Demo.Systems
{
    // 回放系统，负责回放录制的帧数据
    public class ReplaySystem : ComponentSystem
    {
        private List<FrameData> _replayData;
        private int _currentReplayFrame;
        private bool _isReplaying;
        private float _replaySpeed;
        private ComponentManager _componentManager;
        private EntityManager _entityManager;
        
        public bool IsReplaying => _isReplaying;
        public int CurrentFrame => _currentReplayFrame;
        public int TotalFrames => _replayData?.Count ?? 0;
        public float ReplaySpeed { get => _replaySpeed; set => _replaySpeed = value; }
        
        public ReplaySystem()
        {
            _replayData = new List<FrameData>();
            _currentReplayFrame = 0;
            _isReplaying = false;
            _replaySpeed = 1.0f;
        }
         
        // 设置回放数据
        public void SetReplayData(List<FrameData> replayData)
        {
            _replayData = new List<FrameData>(replayData);
            _currentReplayFrame = 0;
        }
        
        // 开始回放
        public void StartReplay()
        {
            if (_replayData == null || _replayData.Count == 0)
            {
                LogManager.Instance.Log("No replay data available");
                return;
            }
            
            _isReplaying = true;
            _currentReplayFrame = 0;
            LogManager.Instance.Log($"Starting replay with {_replayData.Count} frames");
        }
        
        // 停止回放
        public void StopReplay()
        {
            _isReplaying = false;
            _currentReplayFrame = 0;
            LogManager.Instance.Log("Replay stopped");
        }
        
        // 暂停回放
        public void PauseReplay()
        {
            _isReplaying = false;
        }
        
        // 恢复回放
        public void ResumeReplay()
        {
            _isReplaying = true;
        }
        
        // 跳转到指定帧
        public void SeekToFrame(int frameNumber)
        {
            if (frameNumber >= 0 && frameNumber < _replayData.Count)
            {
                _currentReplayFrame = frameNumber;
                ApplyFrameData(_replayData[_currentReplayFrame]);
            }
        }
        
        protected override void OnUpdate(TimeData deltaTime)
        {
            if (!_isReplaying || _replayData == null || _replayData.Count == 0)
                return;
                
            // 检查是否到达回放结束
            if (_currentReplayFrame >= _replayData.Count)
            {
                StopReplay();
                return;
            }
            
            // 应用当前帧数据
            var frameData = _replayData[_currentReplayFrame];
            ApplyFrameData(frameData);
            
            // 移动到下一帧
            _currentReplayFrame++;
        }
        
        // 应用帧数据到当前世界状态
        private void ApplyFrameData(FrameData frameData)
        {
            // 恢复实体状态
            if (frameData.entityStates != null)
            {
                foreach (var entityState in frameData.entityStates)
                {
                    var entity = new Entity(entityState.Key);
                    foreach (var componentState in entityState.Value)
                    {
                        // 这里需要根据具体的组件类型来恢复状态
                        // 由于泛型限制，这里使用反射或者预定义的类型映射
                        RestoreComponentState(entity, componentState.Key, componentState.Value);
                    }
                }
            }
        }
        
        // 恢复组件状态（需要根据具体项目扩展）
        private void RestoreComponentState(Entity entity, Type componentType, IComponentData componentData)
        {
            // 这里可以根据具体的组件类型来处理
            // 例如：
            if (componentType == typeof(Translation))
            {
                _componentManager.SetComponent(entity, (Translation)componentData);
            }
            else if (componentType == typeof(Velocity))
            {
                _componentManager.SetComponent(entity, (Velocity)componentData);
            }
            // 可以继续添加其他组件类型的处理
        }
        
        // 获取当前帧的输入数据
        public Dictionary<string, object> GetCurrentFrameInputs()
        {
            if (_isReplaying && _currentReplayFrame < _replayData.Count)
            {
                return _replayData[_currentReplayFrame].inputs;
            }
            return new Dictionary<string, object>();
        }

        // 从文件加载录像
        public void LoadReplayFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    LogManager.Instance.Log($"Replay file not found: {filePath}");
                    return;
                }

                var json = File.ReadAllText(filePath);
                var replayData = JsonConvert.DeserializeObject<List<FrameData>>(json);
                if (replayData != null)
                {
                    SetReplayData(replayData);
                    LogManager.Instance.Log($"Replay loaded from: {filePath}. Frames: {replayData.Count}");
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Failed to load replay: {ex.Message}");
            }
        }

        // 保存当前回放数据到文件
        public void SaveReplayToFile(string filePath)
        {
            try
            {
                if (_replayData == null || _replayData.Count == 0)
                {
                    LogManager.Instance.Log("No replay data to save");
                    return;
                }

                var json = JsonConvert.SerializeObject(_replayData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                LogManager.Instance.Log($"Replay saved to: {filePath}");
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Failed to save replay: {ex.Message}");
            }
        }

        // 清空回放数据
        public void ClearReplayData()
        {
            _replayData?.Clear();
            _currentReplayFrame = 0;
            LogManager.Instance.Log("Replay data cleared");
        }

        // 获取回放进度（0-1）
        public float GetReplayProgress()
        {
            if (_replayData == null || _replayData.Count == 0) return 0f;
            return (float)_currentReplayFrame / _replayData.Count;
        }

        // 设置回放速度
        public void SetReplaySpeed(float speed)
        {
            _replaySpeed = Math.Max(0.1f, speed); // 最小速度0.1x
        }

        // 跳转到回放开始
        public void SeekToStart()
        {
            SeekToFrame(0);
        }

        // 跳转到回放结束
        public void SeekToEnd()
        {
            if (_replayData != null && _replayData.Count > 0)
            {
                SeekToFrame(_replayData.Count - 1);
            }
        }

        // 获取指定帧的数据
        public FrameData? GetFrameData(int frameNumber)
        {
            if (_replayData != null && frameNumber >= 0 && frameNumber < _replayData.Count)
            {
                return _replayData[frameNumber];
            }
            return null;
        }
    }
}