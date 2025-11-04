using System.Threading;
using LemonFrameSync.Logger;
using LemonFramework.ECS;
using LemonFramework.ECS.Actors;
using LemonFramework.ECS.Components;
using LemonFramework.ECS.Entitys;
using LemonFramework.ECS.Systems;
using UnityEngine;

namespace LemonFramework.Demo
{
    /// <summary>
    /// 多线程ECS演示
    /// ECS在独立线程中运行，Unity主线程从ECS读取数据并同步到GameObject
    /// </summary>
    public class MultiThreadEcsDemo : MonoBehaviour
    {
        [Header("设置")]
        [Tooltip("生成多少个实体")]
        public int entityCount = 100;
        
        [Tooltip("使用的预制体")]
        public GameObject entityPrefab;
        
        private World _ecsWorld;
        private Thread _ecsThread;
        private TimeData _timeData;
        private bool _isRunning = true;
        
        private EntityActorManager _manager;
        
        void Start()
        {
            LogManager.Instance.Initialize();
            
            // 注册组件类型
            ComponentManager.RegisterComponentType<EcsTransform>();
            
            // 创建同步管理器
            _manager = gameObject.AddComponent<EntityActorManager>();
            
            // 初始化ECS场景
            InitializeECS();
            
            // 创建实体和对应的Unity GameObject
            CreateEntities();
            
            // 启动ECS线程
            StartEcsThread();
            
            Debug.Log($"=== 多线程ECS演示已启动 ===");
            Debug.Log($"创建了 {entityCount} 个实体");
            Debug.Log($"ECS运行在独立线程，Unity主线程同步显示");
        }
        
        void InitializeECS()
        {
            // 创建ECS场景
            _ecsWorld = new World("MultiThreadWorld");
            _timeData = new TimeData(60); // 60 FPS
            
            // 设置同步管理器的场景
            _manager.SetWorld(_ecsWorld);
            
            // 添加移动系统（在ECS线程中运行）
            _ecsWorld.AddSystem(new CircleMovementSystem());
        }
        
        void CreateEntities()
        {
            // 如果没有指定预制体，使用默认立方体
            if (entityPrefab == null)
            {
                entityPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                entityPrefab.transform.localScale = Vector3.one * 0.5f;
            }
            
            // 创建实体网格
            int gridSize = Mathf.CeilToInt(Mathf.Sqrt(entityCount));
            float spacing = 2f;
            
            for (int i = 0; i < entityCount; i++)
            {
                int x = i % gridSize;
                int z = i / gridSize;
                
                // 在ECS中创建实体
                Entity entity = _ecsWorld.CreateEntity();
                
                Vector3 startPos = new Vector3(
                    (x - gridSize / 2f) * spacing,
                    0,
                    (z - gridSize / 2f) * spacing
                );
                
                // 添加ECS Transform组件
                _ecsWorld.AddComponent(entity, new EcsTransform(startPos, Quaternion.identity, Vector3.one * 0.5f));
                
                // 在Unity中创建对应的GameObject
                GameObject go = Instantiate(entityPrefab, startPos, Quaternion.identity);
                go.name = $"Entity_{entity.Id}";
                
                // 设置随机颜色
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var mat = new Material(Shader.Find("Standard"));
                    mat.color = new Color(
                        Random.value,
                        Random.value,
                        Random.value
                    );
                    renderer.material = mat;
                }
                
                // 注册到同步管理器
                _manager.RegisterEntity(go, entity);
            }
        }
        
        void StartEcsThread()
        {
            _ecsThread = new Thread(EcsUpdateLoop)
            {
                IsBackground = true,
                Name = "ECS-Thread",
                Priority = System.Threading.ThreadPriority.Normal
            };
            _ecsThread.Start();
            
            Debug.Log("ECS线程已启动");
        }
        
        void EcsUpdateLoop()
        {
            while (_isRunning)
            {
                // ECS更新（在独立线程中）
                _ecsWorld.Update(_timeData);
                
                // 休眠以维持帧率
                Thread.Sleep((int)_timeData.MilliSeconds);
                _timeData.Frame++;
            }
            
            Debug.Log("ECS线程已停止");
        }
        
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = new GUIStyleState { textColor = Color.white }
            };
            
            GUILayout.Label("=== ECS状态 ===", style);
            GUILayout.Label($"ECS帧: {_timeData.Frame}", style);
            GUILayout.Label($"实体数: {entityCount}", style);
            GUILayout.Label($"ECS线程: 运行中", style);
            GUILayout.Label($"Unity FPS: {(int)(1f / Time.deltaTime)}", style);
            
            GUILayout.EndArea();
        }
        
        void OnDestroy()
        {
            _isRunning = false;
            
            // 等待ECS线程结束
            if (_ecsThread != null && _ecsThread.IsAlive)
            {
                _ecsThread.Join(1000);
            }
            
            // 清理场景
            _ecsWorld?.Dispose();
            
            LogManager.Instance.Shutdown();
            Debug.Log("多线程ECS演示已停止");
        }
    }
    
}

