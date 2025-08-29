using System;
using System.Collections.Generic;
using System.IO;
using LemonFramework.ECS.Demo.Components;
using LemonFramework.ECS;
using LemonFrameSync.Logger;
using Newtonsoft.Json;

namespace LemonFramework.ECS.Demo.Systems
{
    // 录制系统，负责录制帧数据和输入
    public class RecordSystem : ComponentSystem
    {
        private InputRecorder _inputRecorder;
        private List<FrameData> _recordedFrames;
        private ComponentManager _componentManager;
        private EntityManager _entityManager;
        private int _currentFrame;
        private bool _isRecording;

        public bool IsRecording => _isRecording;
        public int CurrentFrame => _currentFrame;
        public int RecordedFrameCount => _recordedFrames?.Count ?? 0;
        public List<FrameData> RecordedFrames => _recordedFrames;

        public RecordSystem()
        {
            _inputRecorder = new InputRecorder();
            _recordedFrames = new List<FrameData>();
            _currentFrame = 0;
            _isRecording = false;
        }
 
        // 开始录制
        public void StartRecording()
        {
            if (_isRecording)
            {
                LogManager.Instance.Log("Already recording!");
                return;
            }

            _isRecording = true;
            _currentFrame = 0;
            _recordedFrames.Clear();
            _inputRecorder.StartRecording();
            LogManager.Instance.Log("Started recording");
        }

        // 停止录制
        public void StopRecording()
        {
            if (!_isRecording)
            {
                LogManager.Instance.Log("Not currently recording!");
                return;
            }

            _isRecording = false;
            _inputRecorder.StopRecording();
            LogManager.Instance.Log($"Stopped recording. Total frames: {_recordedFrames.Count}");
        }

        // 暂停录制
        public void PauseRecording()
        {
            if (_isRecording)
            {
                _inputRecorder.StopRecording();
                LogManager.Instance.Log("Recording paused");
            }
        }

        // 恢复录制
        public void ResumeRecording()
        {
            if (_isRecording)
            {
                _inputRecorder.StartRecording();
                LogManager.Instance.Log("Recording resumed");
            }
        }

        protected override void OnUpdate(TimeData deltaTime)
        {
            if (_isRecording)
            {
                RecordFrame(deltaTime.DeltaTime);
            }
        }

        // 记录当前帧
        public void RecordFrame(float deltaTime)
        {
            if (!_isRecording) return;

            var frameData = new FrameData(_currentFrame, deltaTime);

            // 记录输入数据
            var inputs = _inputRecorder.GetAndClearCurrentFrameInputs();
            foreach (var input in inputs)
            {
                frameData.AddInput(input.Key, input.Value);
            }

            // 记录实体状态
            RecordEntityStates(ref frameData);

            _recordedFrames.Add(frameData);
            _currentFrame++;
        }

        // 记录实体状态
        private void RecordEntityStates(ref FrameData frameData)
        {
            if (_componentManager == null) return;
            
            var allTypeComponentData = _componentManager.GetAllComponentData();
            foreach (var typeComponentData in allTypeComponentData)
            {
                foreach (var componentData in typeComponentData.Value)
                {
                    frameData.AddEntityState(componentData.Key, typeComponentData.Key, componentData.Value);
                }
            }
        }

        // 记录输入
        public void RecordInput(string key, object value)
        {
            _inputRecorder.RecordInput(key, value);
        }

        // 记录键盘输入
        public void RecordKeyInput(string keyName, bool isPressed)
        {
            _inputRecorder.RecordKeyInput(keyName, isPressed);
        }

        // 记录鼠标输入
        public void RecordMouseInput(float x, float y, bool leftButton, bool rightButton)
        {
            _inputRecorder.RecordMouseInput(x, y, leftButton, rightButton);
        }

        // 记录自定义输入
        public void RecordCustomInput(string inputName, object value)
        {
            _inputRecorder.RecordCustomInput(inputName, value);
        }

        // 保存录像到文件
        public void SaveReplayToFile(string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(_recordedFrames, Formatting.Indented);
                File.WriteAllText(filePath, json);
                LogManager.Instance.Log($"Replay saved to: {filePath}");
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Failed to save replay: {ex.Message}");
            }
        }

        // 从文件加载录像数据
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
                _recordedFrames = JsonConvert.DeserializeObject<List<FrameData>>(json) ?? new List<FrameData>();
                LogManager.Instance.Log($"Replay loaded from: {filePath}. Frames: {_recordedFrames.Count}");
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Failed to load replay: {ex.Message}");
            }
        }

        // 清空录像数据
        public void ClearReplayData()
        {
            _recordedFrames.Clear();
            _currentFrame = 0;
            LogManager.Instance.Log("Replay data cleared");
        }

        // 获取录制进度信息
        public float GetRecordingProgress()
        {
            return _currentFrame;
        }
    }
}