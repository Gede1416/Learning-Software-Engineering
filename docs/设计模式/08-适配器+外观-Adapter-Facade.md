# 适配器模式 + 外观模式（Adapter + Facade）

> 来源：《设计模式》GoF 第 4 章 + 《Head First 设计模式》第 7 章

---

## 适配器模式（Adapter）

### 一、书中定义

GoF 的定义：

> **"将一个类的接口转换成客户希望的另一个接口。适配器模式让原本由于接口不兼容而不能一起工作的类可以协同工作。"**

Head First 的直觉版：**适配器就是电源转换插头——把 220V 圆孔转成 110V 扁头，中间那坨东西。**

---

### 二、坏代码场景

假设你的游戏原本用 Unity 内置音频 API 播放音效。整个项目有 47 处调用：

```csharp
// 47 个文件里散落着：
AudioSource.PlayClipAtPoint(fireSound, playerPosition);
AudioSource.PlayClipAtPoint(explosionSound, enemyPosition);
```

现在要接入第三方音频中间件 **Wwise**（大项目标配），API 完全不同：

```csharp
// Wwise 的调用方式：
AkSoundEngine.PostEvent("FireEvent", gameObject);
AkSoundEngine.SetRTPCValue("Volume", volume);
```

你有两个选择：

> **A：把 47 个文件全改一遍。**  
> **B：不动 47 个文件，在中间加一层翻译。**

---

### 问题

1. 方案 A 的问题在哪？（提示：不只是工作量大）

2. 如果半年后策划说「Wwise 太贵了，切回 Unity 原生音频」，方案 A 的项目要怎么办？

---

## 外观模式（Facade）

### 一、书中定义

GoF 的定义：

> **"为子系统中的一组接口提供一个统一的接口。外观模式定义了一个高层接口，使子系统更容易使用。"**

Head First 的直觉版：**家里有一个总电闸开关，你不用分别去拉每个房间的灯。**

---

### 二、坏代码场景

你的游戏**存档系统**涉及四个子系统：

```csharp
public class SaveManager
{
    public void SaveGame(int slot)
    {
        // ① 收集所有需要保存的数据
        var playerData = Player.Instance.Serialize();
        var worldData = WorldManager.Instance.Serialize();
        var questData = QuestSystem.Instance.Serialize();
        var settingData = SettingsManager.Instance.Serialize();

        // ② 压缩
        var compressed = Compression.Compress(playerData, worldData, questData, settingData);

        // ③ 加密
        var encrypted = Encryption.Encrypt(compressed, "secret-key-123");

        // ④ 写入磁盘
        File.WriteAllBytes($"save_{slot}.dat", encrypted);

        // ⑤ 更新 UI
        UIManager.ShowTip("保存成功！");
    }
}
```

调用方（如暂停菜单的"保存按钮"）就得知道这五个步骤的顺序、依赖、异常处理。如果有 10 个不同的保存入口（自动保存、手动保存、退出时保存、云存档……），这五个步骤的代码要重复 10 遍。

---

## 问题

3. 如果「加密算法」升级（AES → ChaCha20），改 10 处调用方，怎么解决？

4. 外观模式和适配器模式都包装了其他类。区别在哪？——提示：适配器改变接口，外观简化接口。

---

## 你的回答（2026-07-27）

1. **违背 OCP，不支持后续更换** ✅ — 每次换音频引擎 = 全项目扫一遍。
2. **回退工作量更大** ✅ — 调用点可能比当初更多，重写两次第三方接入是噩梦。
3. **给加密算法加接口，替换实现** — 方向对，但只解了加密这一个步骤。10 个调用方仍然知道「收集数据 → 压缩 → 加密 → 写磁盘 → 弹 UI」这个五步流程。**Facade 要封装的不是单个步骤，是整个流程。**
4. **适配器做兼容和输入转化，外观统一对外表现，把流程转化为参数传入接口统一处理** ✅ — 精准。

---

## 适配器模式（Adapter）—— 标准解

### 核心思路

```
之前：47 个调用方 → 直接调 Unity AudioSource API
之后：47 个调用方 → 调 IAudioService 接口 → Adapter 翻译 → Wwise / Unity / FMOD
```

### 代码

```csharp
// ① 定义自己的接口 —— 不依赖任何第三方
public interface IAudioService
{
    void PlaySFX(string clipName, Vector3 position);
    void SetVolume(float volume);
    void StopAll();
}

// ② Unity 原生适配器
public class UnityAudioAdapter : IAudioService
{
    private Dictionary<string, AudioClip> _clips;

    public void PlaySFX(string clipName, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(_clips[clipName], position);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void StopAll()
    {
        // Unity 没有直接的全停 API，自己实现
    }
}

// ③ Wwise 适配器
public class WwiseAudioAdapter : IAudioService
{
    public void PlaySFX(string clipName, Vector3 position)
    {
        AkSoundEngine.PostEvent(clipName, Camera.main.gameObject);
    }

    public void SetVolume(float volume)
    {
        AkSoundEngine.SetRTPCValue("MasterVolume", volume);
    }

    public void StopAll()
    {
        AkSoundEngine.StopAll();
    }
}

// ④ 使用方 —— 只依赖 IAudioService，不关心底层是哪个引擎
public class Explosion : MonoBehaviour
{
    private IAudioService _audio;  // 注入

    void OnExplode()
    {
        _audio.PlaySFX("Explosion", transform.position);
        // ← 这行代码永远不变，不管是 Unity 还是 Wwise
    }
}

// ⑤ 切换引擎？只改一行：注入哪个 Adapter
// _audio = new UnityAudioAdapter();   // 原生
// _audio = new WwiseAudioAdapter();    // Wwise
```

**加了 Adapter 后，切换音频引擎：47 个文件零修改。**

---

## 外观模式（Facade）—— 标准解

### Q3 你漏掉的部分

你说「给加密算法加接口」——这解决了单个步骤的可替换性，但没解决「10 个调用方都知道 5 个步骤的流程」。Facade 要做的是：

> **把五步流程变成一个方法调用。**

### 代码

```csharp
// Facade —— 10 个调用方只调这一个方法
public class SaveFacade
{
    private ISerializer _serializer;    // 策略：序列化
    private ICompressor _compressor;    // 策略：压缩
    private IEncryptor _encryptor;      // 策略：加密（你说的接口替换就在这里）

    public SaveFacade(ISerializer serializer, ICompressor compressor, IEncryptor encryptor)
    {
        _serializer = serializer;
        _compressor = compressor;
        _encryptor = encryptor;
    }

    // ← 五个步骤封装成一个方法
    public void Save(int slot, SaveContext ctx)
    {
        // ① 收集数据（ctx 里带了需要保存的对象引用）
        var raw = _serializer.Serialize(ctx);

        // ② 压缩
        var compressed = _compressor.Compress(raw);

        // ③ 加密
        var encrypted = _encryptor.Encrypt(compressed);

        // ④ 写磁盘
        File.WriteAllBytes($"save_{slot}.dat", encrypted);

        // ⑤ 通知 UI（用观察者模式解耦更好，但这里先简单写）
        EventBus.Publish(new SaveCompletedEvent(slot));
    }
}

// 调用方 —— 10 个入口变成一行
public class PauseMenu : MonoBehaviour
{
    private SaveFacade _saveFacade;

    public void OnSaveButtonClicked()
    {
        _saveFacade.Save(1, BuildContext());  // ← 五个步骤变成一行
        // 加密换了？压缩换了？不关 PauseMenu 的事。它甚至不知道这些步骤存在。
    }
}
```

| 场景 | 之前 | 之后 |
|------|------|------|
| 加新保存入口 | 复制粘贴五个步骤 | 调 `_saveFacade.Save()` |
| 加密升级 | 改 10 处的加密调用 | 只改 Facade 里注入的 `_encryptor` |
| 加新步骤（如上传云存档） | 改 10 处 | Facade 里加一行，10 个入口零感知 |
| 改步骤顺序（先加密再压缩） | 改 10 处 | Facade 里改一行 |

---

## 适配器 vs 外观 —— 你的第四条

你回答的「适配器做兼容和输入转化，外观统一对外表现」总结得很准。展开对比：

| | 适配器（Adapter） | 外观（Facade） |
|------|-------------------|----------------|
| **目的** | 兼容不匹配的接口 | 简化复杂的子系统 |
| **改变什么** | **接口形式**（A 接口 → B 接口） | **接口粒度**（N 个细粒度调用 → 1 个粗粒度调用） |
| **方向** | 横向翻译（电压转换） | 纵向收口（总电闸） |
| **被包装方** | 通常是**一个**外部类/库 | 通常是**多个**内部类组成的子系统 |
| **客户端关心** | 想用 B 的接口，但手里只有 A 的实现 | 想要一个简单的入口，不想管内部有多复杂 |
| **你的话** | "做兼容，接口后面要添加输入转化" | "统一对外表现，流程转化为参数传入接口" |

---

## 现实中的例子

| 模式 | 游戏实例 |
|------|----------|
| Adapter | 接 Steam API / 微信 SDK / 广告 SDK —— 第三方接口不由你设计 |
| Facade | 存档系统 / 网络层 / 资源加载 —— 内部一堆复杂逻辑，对外一个 `LoadAsync()` |

---

## 跨书关联

| 关联概念 | 来源 |
|----------|------|
| OCP —— Adapter 让切换外部库不改已有代码 | 《敏捷》第 9 章 |
| 最少知识原则（Least Knowledge）—— Facade 让客户端知道的越少越好 | 《Head First》第 7 章 |
| 策略模式 —— Adapter 内部常配合策略（如 `IEncryptor`） | GoF 第 5 章 |
| 代理模式 —— 三种「包装」模式的区别（Adapter 改接口，Decorator 加行为，Proxy 控访问） | GoF 第 4 章 |

---

## 八、作业（预计 15 分钟）

你的游戏接了两个**广告 SDK**（穿山甲和优量汇），API 完全不同：

```csharp
// 穿山甲
BytedanceAd.ShowRewardVideo("reward_001", OnRewardCallback);

// 优量汇
YLHAd.LoadAndShow(YLHAdType.Reward, new YLHConfig { placementId = "reward_001" });
```

要求：
1. 定义 `IRewardAdService` 接口（方法：`ShowRewardAd(Action onComplete)`）
2. 为两个 SDK 各写一个 Adapter
3. 用**外观模式**做 `AdManager`，内部持有 `IRewardAdService`，对外暴露 `ShowRewardAd()`
4. 思考：如果加一个新的变现方式「插屏广告（Interstitial）」，Facade 要改还是 Adapter 要改？

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 ✓ / ⑦状态 ✓ / ⑧适配器+外观 → 核心讲解完成，等待作业 ✓]`
