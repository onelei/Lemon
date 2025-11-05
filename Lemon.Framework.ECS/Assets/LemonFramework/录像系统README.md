# 🎬 ECS录像系统

## 📦 已创建的文件

### 核心系统类
1. **ReplayData.cs** - 录像数据类，存储所有帧的快照
2. **ReplayRecorder.cs** - 录像录制器，负责录制功能  
3. **ReplayPlayer.cs** - 录像播放器，负责播放和控制

### 演示示例
4. **ReplayDemo.cs** - 完整的演示Demo，展示所有功能

### 文档
5. **录像系统使用指南.md** - 详细的使用文档和API参考

## 🚀 快速开始

### 1. 运行Demo

1. 在Unity中打开场景
2. 创建一个空GameObject
3. 添加 `ReplayDemo` 组件
4. 点击Play运行

### 2. 操作说明

**录制录像：**
- 点击"开始录制"
- 使用WASD控制红色立方体移动
- 点击"停止录制"
- 点击"保存录像"保存到文件

**播放录像：**
- 点击"加载录像"
- 点击"播放"开始播放
- 可以拖动进度条跳转
- 可以改变播放速度（0.25x - 8x）
- 可以逐帧前进/后退

## ✨ 核心功能

✅ **录制录像** - 实时录制游戏每一帧  
✅ **保存/加载** - 保存到文件和从文件加载  
✅ **播放控制** - 播放、暂停、停止  
✅ **快速跳转** - 跳转到任意帧、时间或进度  
✅ **变速播放** - 支持0.25x到8x倍速  
✅ **进度条** - 可拖动的播放进度条  
✅ **逐帧控制** - 单帧前进/后退  

## 📝 代码示例

### 简单录制

```csharp
// 初始化
ReplayRecorder recorder = new ReplayRecorder(world);

// 开始录制
recorder.StartRecording("我的录像", 60);

// 每帧记录
recorder.RecordFrame();

// 停止并保存
recorder.StopRecording();
recorder.SaveReplay("replay.dat");
```

### 简单播放

```csharp
// 初始化
ReplayPlayer player = new ReplayPlayer(world);

// 加载并播放
player.LoadReplayFromFile("replay.dat");
player.Play();

// 每帧更新
player.Update(Time.deltaTime);

// 控制
player.SetPlaybackSpeed(2.0f);  // 2倍速
player.SeekToProgress(0.5f);    // 跳转到50%
```

## 📚 详细文档

完整的API参考和使用说明请查看：**录像系统使用指南.md**

## 🎮 UI控制说明

### 左侧信息面板
- 显示当前帧数
- 显示FPS
- 显示当前状态
- 显示录制/播放信息

### 右侧控制面板

**录制控制：**
- 开始录制 / 停止录制 / 取消录制

**文件操作：**
- 保存录像
- 加载录像

**播放控制：**
- 播放 / 暂停 / 停止

**播放速度：**
- 0.25x, 0.5x, 1x, 2x, 4x, 8x

**帧控制：**
- ◄◄ 前10帧
- ◄ 上一帧
- 下一帧 ►
- 后10帧 ►►

### 底部进度条
- 显示当前播放进度
- 可拖动跳转到任意位置
- 显示当前时间/总时长

## 🎯 使用场景

1. **游戏回放** - 观看精彩瞬间
2. **调试工具** - 重现bug，逐帧分析  
3. **AI训练** - 录制玩家操作数据
4. **教程系统** - 制作教学录像
5. **竞技分析** - 分析操作和策略

## 💡 技术特点

- 基于ECS快照系统
- 支持帧精确控制
- 完整的序列化支持
- 线程安全设计
- 易于集成和扩展

## 📄 文件位置

```
LemonFramework/
├── ECS/
│   └── Replay/
│       ├── ReplayData.cs          # 录像数据类
│       ├── ReplayRecorder.cs      # 录制器
│       └── ReplayPlayer.cs        # 播放器
├── Demo/
│   └── ReplayDemo.cs              # 演示Demo
├── 录像系统使用指南.md             # 详细文档
└── 录像系统README.md              # 本文件
```

## 🔧 系统要求

- Unity 2019.4 或更高版本
- 依赖现有的ECS框架
- 需要 `LemonFramework.ECS` 命名空间

## 📞 问题反馈

如有问题请参考 **录像系统使用指南.md** 中的"常见问题"章节。

---

**祝您使用愉快！** 🎉

