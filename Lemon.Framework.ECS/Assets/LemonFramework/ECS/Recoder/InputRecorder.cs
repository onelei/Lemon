using System;
using System.Collections.Generic;

namespace LemonFramework.ECS
{
    // 输入记录器，负责记录每帧的输入数据
    public class InputRecorder
    {
        private Dictionary<string, object> _currentFrameInputs;
        private bool _isRecording;
        
        public bool IsRecording => _isRecording;
        
        public InputRecorder()
        {
            _currentFrameInputs = new Dictionary<string, object>();
            _isRecording = false;
        }
        
        // 开始录制
        public void StartRecording()
        {
            _isRecording = true;
            _currentFrameInputs.Clear();
        }
        
        // 停止录制
        public void StopRecording()
        {
            _isRecording = false;
            _currentFrameInputs.Clear();
        }
        
        // 记录输入
        public void RecordInput(string key, object value)
        {
            if (!_isRecording) return;
            
            _currentFrameInputs[key] = value;
        }
        
        // 记录键盘输入
        public void RecordKeyInput(string keyName, bool isPressed)
        {
            RecordInput($"key_{keyName}", isPressed);
        }
        
        // 记录鼠标输入
        public void RecordMouseInput(float x, float y, bool leftButton, bool rightButton)
        {
            RecordInput("mouse_x", x);
            RecordInput("mouse_y", y);
            RecordInput("mouse_left", leftButton);
            RecordInput("mouse_right", rightButton);
        }
        
        // 记录自定义输入
        public void RecordCustomInput(string inputName, object value)
        {
            RecordInput($"custom_{inputName}", value);
        }
        
        // 获取当前帧的输入数据并清空
        public Dictionary<string, object> GetAndClearCurrentFrameInputs()
        {
            var result = new Dictionary<string, object>(_currentFrameInputs);
            _currentFrameInputs.Clear();
            return result;
        }
        
        // 获取输入值
        public T GetInput<T>(string key)
        {
            if (_currentFrameInputs.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default(T);
        }
        
        // 检查是否有输入
        public bool HasInput(string key)
        {
            return _currentFrameInputs.ContainsKey(key);
        }
    }
}