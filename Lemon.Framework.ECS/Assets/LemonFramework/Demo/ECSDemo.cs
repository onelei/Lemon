using System.Threading;
using LemonFramework.ECS;
using LemonFrameSync.Logger;
using LemonFramework.ECS.Demo.Components;
using LemonFramework.ECS.Demo.Systems;
using UnityEngine;
using Vector3 = LemonFramework.ECS.Demo.Components.Vector3;

namespace LemonFramework.ECS.Demo
{
    public class ECSDemo : MonoBehaviour
    {
        private World _world;
        private Thread _thread;
        private TimeData _timeData;
        private ReplayManager _replayManager;
        private Entity _playerEntity;
        
        // 录像控制变量
        private bool _recordingStarted = false;
        private float _recordingTime = 0f;
        private const float RECORDING_DURATION = 5f; // 录制5秒
        private const float REPLAY_DELAY = 1f; // 录制结束后1秒开始回放

        private void Start()
        {
            LogManager.Instance.Initialize();
            //Frame
            var targetFrame = 60;
            //World
            _world = new World();
            //Time
            _timeData = new TimeData(targetFrame);
            
            // 初始化录像管理器
            _replayManager = new ReplayManager(_world.ComponentManager, _world.EntityManager);
            
            //Entity
            _playerEntity = _world.CreateEntity();
            _world.AddComponent(_playerEntity, new Translation(new Components.Vector3(0, 0, 0)));
            _world.AddComponent(_playerEntity, new Velocity(new Components.Vector3(1, 2, 3)));
            
            //System
            _world.AddSystem(new MovementSystem());
            
            //Thread
            _thread = new Thread(OnUpdate) {IsBackground = true};
            _thread.Start();
            
            Debug.Log("ECS Demo started with Replay functionality!");
            Debug.Log("Recording will start automatically and last for 5 seconds.");
            Debug.Log("After recording, replay will start automatically.");
        }

        private void OnDestroy()
        {
            LogManager.Instance.Shutdown();
            _thread.Abort();
        }

        private void OnUpdate()
        {
            while (true)
            {
                // 处理录像逻辑
                HandleReplayLogic();
                
                // 模拟输入（在实际项目中，这里应该是真实的输入处理）
                SimulateInput();
                
                // 更新世界
                _world.Update(_timeData);
                
                // 更新回放系统
                _replayManager.Update(_timeData);
                
                Thread.Sleep((int) _timeData.MilliSeconds);
                ++_timeData.Frame;
            }
        }
        
        private void HandleReplayLogic()
        {
            _recordingTime += _timeData.DeltaTime;
            
            // 开始录制
            if (!_recordingStarted && _recordingTime >= 1f)
            {
                _replayManager.StartRecording();
                _recordingStarted = true;
                Debug.Log("Recording started!");
            }
            
            // 录制过程中记录帧数据
            if (_replayManager.IsRecording)
            {
                //_replayManager.RecordFrame(_timeData.DeltaTime);
                
                // 停止录制
                if (_recordingTime >= RECORDING_DURATION + 1f)
                {
                    _replayManager.StopRecording();
                    Debug.Log("Recording stopped! Frames recorded: " + _replayManager.RecordedFrameCount);
                }
            }
            
            // 开始回放
            if (!_replayManager.IsRecording && !_replayManager.IsReplaying && 
                _recordingTime >= RECORDING_DURATION + 1f + REPLAY_DELAY)
            {
                Debug.Log("Starting replay...");
                _replayManager.StartReplay();
            }
        }
        
        private void SimulateInput()
        {
            // 模拟一些输入数据用于演示
            if (_replayManager.IsRecording)
            {
                // 模拟键盘输入
                _replayManager.RecordKeyInput("W", _timeData.Frame % 120 < 60); // 每2秒按一次W键
                _replayManager.RecordKeyInput("A", _timeData.Frame % 180 < 90); // 每3秒按一次A键
                
                // 模拟鼠标输入
                float mouseX = Mathf.Sin(_timeData.Frame * 0.1f) * 100f;
                float mouseY = Mathf.Cos(_timeData.Frame * 0.1f) * 100f;
                _replayManager.RecordMouseInput(mouseX, mouseY, _timeData.Frame % 240 < 120, false);
                
                // 模拟自定义输入
                _replayManager.RecordInput("player_action", _timeData.Frame % 300 < 150 ? "jump" : "idle");
            }
        }
    }
}