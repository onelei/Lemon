# Unity GameObject 与多线程 ECS 同步指南

## 架构说明

本ECS框架采用**单向同步**设计：
- **ECS在独立线程运行**（高性能逻辑计算）
- **Unity在主线程**从ECS读取数据（显示渲染）
- **数据流向**: ECS（多线程） → Unity（主线程）

## 核心组件

### 1. ECSTransform 组件

在ECS中存储Transform数据，Unity会读取这个组件同步到GameObject：

```csharp
public struct ECSTransform : IComponentData, ISerializable
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 LocalScale;
}
```

### 2. UnityEntityBridge

每个GameObject上的桥接组件，负责从ECS读取数据：

```csharp
public class UnityEntityBridge : MonoBehaviour
{
    // 自动从ECS读取Transform数据并更新GameObject
    public void SyncFromECS();
}
```

### 3. UnityECSSyncManager

全局同步管理器，管理所有实体的同步：

```csharp
public class UnityECSSyncManager : MonoBehaviour
{
    // 注册GameObject与ECS实体的关联
    public UnityEntityBridge RegisterEntity(GameObject go, Entity entity);
}
```

## 快速开始

### 第一步：创建同步管理器

```csharp
using LemonFramework.ECS;
using LemonFramework.ECS.Unity;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Scene _ecsScene;
    private UnityECSSyncManager _syncManager;
    
    void Start()
    {
        // 1. 注册组件类型
        ComponentManager.RegisterComponentType<ECSTransform>();
        
        // 2. 创建ECS场景
        _ecsScene = new Scene("GameScene");
        
        // 3. 创建同步管理器
        _syncManager = gameObject.AddComponent<UnityECSSyncManager>();
        _syncManager.SetScene(_ecsScene);
    }
}
```

### 第二步：创建实体和GameObject

```csharp
void CreatePlayer()
{
    // 1. 在ECS中创建实体
    Entity playerEntity = _ecsScene.CreateEntity();
    
    // 2. 添加ECS Transform组件
    _ecsScene.AddComponent(playerEntity, new ECSTransform(
        new Vector3(0, 0, 0),     // 位置
        Quaternion.identity,       // 旋转
        Vector3.one               // 缩放
    ));
    
    // 3. 创建Unity GameObject
    GameObject playerGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
    playerGO.name = "Player";
    
    // 4. 注册到同步管理器（会自动添加UnityEntityBridge组件）
    _syncManager.RegisterEntity(playerGO, playerEntity);
    
    // 现在GameObject会自动同步ECS中的Transform数据！
}
```

### 第三步：在ECS中更新数据

```csharp
// 创建一个移动系统
public class PlayerMoveSystem : ComponentSystem
{
    protected override void OnUpdate(TimeData timeData)
    {
        var entities = ComponentManager.GetEntitiesWithComponents(typeof(ECSTransform));
        
        foreach (var entity in entities)
        {
            if (ComponentManager.TryGetComponent(entity, out ECSTransform transform))
            {
                // 在ECS中修改位置
                transform.Position += new Vector3(1, 0, 0) * timeData.DeltaTime;
                
                // 更新组件
                ComponentManager.SetComponent(entity, transform);
                
                // Unity GameObject会自动同步这个新位置！
            }
        }
    }
}

// 添加系统
_ecsScene.AddSystem(new PlayerMoveSystem());
```

### 第四步：在多线程中运行ECS

```csharp
using System.Threading;

private Thread _ecsThread;
private bool _isRunning = true;
private TimeData _timeData;

void StartECSThread()
{
    _timeData = new TimeData(60); // 60 FPS
    
    _ecsThread = new Thread(() =>
    {
        while (_isRunning)
        {
            // ECS更新（在独立线程中运行）
            _ecsScene.Update(_timeData);
            
            Thread.Sleep((int)_timeData.MilliSeconds);
            _timeData.Frame++;
        }
    });
    
    _ecsThread.IsBackground = true;
    _ecsThread.Start();
}

void OnDestroy()
{
    _isRunning = false;
    _ecsThread?.Join(1000);
    _ecsScene?.Dispose();
}
```

## 完整示例

```csharp
using System.Threading;
using LemonFramework.ECS;
using LemonFramework.ECS.Unity;
using UnityEngine;

public class SimpleECSGame : MonoBehaviour
{
    private Scene _ecsScene;
    private Thread _ecsThread;
    private TimeData _timeData;
    private bool _isRunning = true;
    private UnityECSSyncManager _syncManager;
    
    void Start()
    {
        // 注册组件
        ComponentManager.RegisterComponentType<ECSTransform>();
        
        // 创建ECS场景
        _ecsScene = new Scene("Game");
        _timeData = new TimeData(60);
        
        // 创建同步管理器
        _syncManager = gameObject.AddComponent<UnityECSSyncManager>();
        _syncManager.SetScene(_ecsScene);
        
        // 添加游戏系统
        _ecsScene.AddSystem(new CircleMovementSystem());
        
        // 创建100个实体
        for (int i = 0; i < 100; i++)
        {
            CreateEntity(i);
        }
        
        // 启动ECS线程
        StartECSThread();
    }
    
    void CreateEntity(int index)
    {
        // ECS实体
        Entity entity = _ecsScene.CreateEntity();
        _ecsScene.AddComponent(entity, new ECSTransform(
            new Vector3(index % 10, 0, index / 10),
            Quaternion.identity,
            Vector3.one * 0.5f
        ));
        
        // Unity GameObject
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = Vector3.one * 0.5f;
        
        // 注册同步
        _syncManager.RegisterEntity(go, entity);
    }
    
    void StartECSThread()
    {
        _ecsThread = new Thread(() =>
        {
            while (_isRunning)
            {
                _ecsScene.Update(_timeData);
                Thread.Sleep(16);
                _timeData.Frame++;
            }
        });
        _ecsThread.IsBackground = true;
        _ecsThread.Start();
    }
    
    void OnDestroy()
    {
        _isRunning = false;
        _ecsThread?.Join(1000);
        _ecsScene?.Dispose();
    }
}

// 圆周运动系统
public class CircleMovementSystem : ComponentSystem
{
    protected override void OnUpdate(TimeData timeData)
    {
        var entities = ComponentManager.GetEntitiesWithComponents(typeof(ECSTransform));
        
        foreach (var entity in entities)
        {
            if (ComponentManager.TryGetComponent(entity, out ECSTransform transform))
            {
                float angle = timeData.Frame * 0.02f;
                float radius = 2f;
                
                transform.Position = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );
                
                ComponentManager.SetComponent(entity, transform);
            }
        }
    }
}
```

## 性能说明

### 线程安全
- ✅ `ComponentManager` 使用读写锁，支持多线程安全读取
- ✅ Unity主线程只读取ECS数据，不写入
- ✅ ECS线程只写入数据，Unity读取时线程安全

### 同步时机
- ECS在独立线程中以固定帧率更新（如60FPS）
- Unity在主线程`LateUpdate`中读取ECS数据
- 即使ECS和Unity帧率不同也能正常工作

### 性能优化
- 使用`Transform`缓存避免重复`GetComponent`
- 批量同步所有实体
- ECS可以使用`UpdateParallel()`并行执行多个系统

## 常见问题

**Q: 如果我想在Unity中手动移动GameObject怎么办？**

A: 当前设计是ECS驱动Unity，如果需要Unity输入，建议：
1. 在Unity收集输入数据
2. 将输入传递给ECS
3. ECS处理后更新Transform
4. Unity同步显示

**Q: 可以同步其他数据吗？**

A: 可以！创建自定义组件和桥接脚本：
```csharp
public struct HealthData : IComponentData, ISerializable
{
    public int HP;
}

// 在UnityEntityBridge中添加
public void SyncHealth()
{
    if (_ecsScene.ComponentManager.TryGetComponent(_ecsEntity, out HealthData health))
    {
        // 更新UI或其他显示
    }
}
```

**Q: 性能如何？**

A: 在测试中，可以轻松处理1000+实体，ECS在独立线程不影响Unity主线程。

## 参考示例

完整示例请查看：
- `MultiThreadECSDemo.cs` - 多线程ECS与Unity同步完整示例
- `SceneDemo.cs` - ECS场景和快照使用示例

