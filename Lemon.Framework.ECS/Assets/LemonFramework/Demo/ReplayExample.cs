// using System;
// using FrameSyncDemo.Components;
// using LemonECS;
// using LemonFrameSync.Logger;
// using UnityEngine;
// using Vector3 = FrameSyncDemo.Components.Vector3;
//
// namespace FrameSyncDemo
// {
//     /// <summary>
//     /// 帧同步录像功能使用示例
//     /// 这个类展示了如何在你的项目中使用录像功能
//     /// </summary>
//     public class ReplayExample : MonoBehaviour
//     {
//         private World _world;
//         private ReplayManager _replayManager;
//         private Entity _testEntity;
//         
//         void Start()
//         {
//             LogManager.Instance.Initialize();
//             // 1. 创建ECS世界
//             _world = new World();
//             
//             // 2. 初始化录像管理器
//             _replayManager = new ReplayManager(_world.ComponentManager, _world.EntityManager);
//             
//             // 3. 创建测试实体
//             _testEntity = _world.CreateEntity();
//             _world.AddComponent(_testEntity, new Translation(new Vector3(0, 0, 0)));
//             _world.AddComponent(_testEntity, new Velocity(new Vector3(1, 0, 0)));
//             
//             Debug.Log("Replay Example initialized!");
//         }
//         
//         void Update()
//         {
//             // 示例：按键控制录像功能
//             HandleInput();
//             var timeData = new TimeData(60) { DeltaTime = Time.deltaTime };
//             // 如果正在录制，记录当前帧
//             if (_replayManager.IsRecording)
//             {
//                 //_replayManager.RecordFrame(timeData.deltaTime);
//             }
//             
//             // 更新回放系统
//             _replayManager.Update(timeData);
//         }
//         
//         void HandleInput()
//         {
//             // R键：开始/停止录制
//             if (Input.GetKeyDown(KeyCode.R))
//             {
//                 if (_replayManager.IsRecording)
//                 {
//                     _replayManager.StopRecording();
//                     Debug.Log("Recording stopped. Frames: " + _replayManager.RecordedFrameCount);
//                 }
//                 else
//                 {
//                     _replayManager.StartRecording();
//                     Debug.Log("Recording started!");
//                 }
//             }
//             
//             // P键：开始回放
//             if (Input.GetKeyDown(KeyCode.P))
//             {
//                 if (_replayManager.RecordedFrameCount > 0)
//                 {
//                     _replayManager.StartReplay();
//                     Debug.Log("Replay started!");
//                 }
//                 else
//                 {
//                     Debug.Log("No recorded data to replay!");
//                 }
//             }
//             
//             // S键：停止回放
//             if (Input.GetKeyDown(KeyCode.S))
//             {
//                 _replayManager.StopReplay();
//                 Debug.Log("Replay stopped!");
//                 SaveReplay("FrameSyncLog");
//                 LogManager.Instance.Shutdown();
//             }
//             
//             // 录制输入数据（在录制期间）
//             if (_replayManager.IsRecording)
//             {
//                 // 记录键盘输入
//                 _replayManager.RecordKeyInput("W", Input.GetKey(KeyCode.W));
//                 _replayManager.RecordKeyInput("A", Input.GetKey(KeyCode.A));
//                 _replayManager.RecordKeyInput("S", Input.GetKey(KeyCode.S));
//                 _replayManager.RecordKeyInput("D", Input.GetKey(KeyCode.D));
//                 
//                 // 记录鼠标输入
//                 var mousePos = Input.mousePosition;
//                 _replayManager.RecordMouseInput(mousePos.x, mousePos.y, 
//                     Input.GetMouseButton(0), Input.GetMouseButton(1));
//                 
//                 // 记录自定义输入
//                 if (Input.GetKeyDown(KeyCode.Space))
//                 {
//                     _replayManager.RecordInput("jump", true);
//                 }
//             }
//         }
//         
//         // 保存录像到文件
//         public void SaveReplay(string fileName)
//         {
//             string filePath = Application.persistentDataPath + "/" + fileName + ".replay";
//             _replayManager.SaveReplayToFile(filePath);
//         }
//         
//         // 从文件加载录像
//         public void LoadReplay(string fileName)
//         {
//             string filePath = Application.persistentDataPath + "/" + fileName + ".replay";
//             _replayManager.LoadReplayFromFile(filePath);
//         }
//         
//         void OnGUI()
//         {
//             GUILayout.BeginArea(new Rect(10, 10, 300, 200));
//             
//             GUILayout.Label("帧同步录像功能演示");
//             GUILayout.Label("按键说明：");
//             GUILayout.Label("R - 开始/停止录制");
//             GUILayout.Label("P - 开始回放");
//             GUILayout.Label("S - 停止回放");
//             GUILayout.Label("");
//             
//             if (_replayManager != null)
//             {
//                 GUILayout.Label($"录制状态: {(_replayManager.IsRecording ? "录制中" : "未录制")}");
//                 GUILayout.Label($"回放状态: {(_replayManager.IsReplaying ? "回放中" : "未回放")}");
//                 GUILayout.Label($"已录制帧数: {_replayManager.RecordedFrameCount}");
//                 
//                 if (_replayManager.IsReplaying)
//                 {
//                     GUILayout.Label($"回放进度: {(_replayManager.GetReplayProgress() * 100):F1}%");
//                 }
//             }
//             
//             GUILayout.EndArea();
//         }
//     }
// }