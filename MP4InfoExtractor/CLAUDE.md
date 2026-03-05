# MP4InfoExtractor 主模块文档

[根目录](../CLAUDE.md) > **MP4InfoExtractor**

---

## 模块职责

MP4InfoExtractor 主模块是整个应用的核心实现，包含：
- WPF UI 界面定义与交互逻辑
- 文件名多策略解析引擎
- CSV 导出功能
- 中文拼音转换与文本处理工具
- 歌曲信息数据模型

## 入口与启动

### 应用入口
- **App.xaml/cs**：WPF 应用程序入口点
- **启动流程**：
  1. `App.xaml.cs` 的 `Application.Run()` 方法启动
  2. 实例化 `MainWindow`
  3. 初始化 UI 组件（`InitializeComponent()`）
  4. 记录启动日志

### 主窗口
- **MainWindow.xaml**：UI 布局定义
  - 文件夹选择区域（TextBox + 浏览按钮）
  - 操作按钮区域（扫描、导出）
  - 日志显示区域（黑底绿字控制台风格）
  - 状态栏（显示文件计数与状态）

- **MainWindow.xaml.cs**：UI 交互逻辑
  - `BtnBrowse_Click`：打开文件夹选择对话框
  - `BtnScan_Click`：启动异步扫描流程
  - `BtnExport_Click`：导出 CSV 文件

## 对外接口

### 公共类
1. **SongInfo**：歌曲信息数据传输对象（DTO）
   - 属性：Artist, SongTitle, Language, Region, VideoQuality 等
   - 方法：无（纯数据类）

2. **PinyinHelper**：拼音转换静态工具类
   - `GetFirstLetter(char)`：获取单字符拼音首字母
   - `GetFullPinyin(string)`：获取完整拼音
   - `GetFirstLetters(string)`：批量获取首字母缩写
   - `ContainsChinese(string)`：检测是否包含中文

3. **TextHelper**：文本处理静态工具类
   - `RemoveParenthesesContent(string)`：移除括号内容
   - `CountWords(string, string)`：统计字数（中英文自适应）
   - `GenerateAbbreviation(string, string)`：生成缩写

4. **LanguageHelper**：语言检测静态工具类
   - `IsChineseLanguage(string)`：判断是否为中文语言
   - `IsEnglishLanguage(string)`：判断是否为英文语言

### 测试入口
- **TestProgram.cs**：独立测试程序（包含 `Main` 方法）
  - 测试中文歌曲解析（我爱中国、月半小夜曲等）
  - 测试英文歌曲解析（Water、Just Like Fire 等）
  - 验证拼音/缩写生成与字数统计

## 关键依赖与配置

### NuGet 依赖
```xml
<!-- packages.config -->
<package id="NPinyin" version="0.2.6321.26573" targetFramework="net45" />
```

### 配置文件
- **App.config**：运行时配置
  ```xml
  <configuration>
      <startup>
          <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
      </startup>
  </configuration>
  ```

- **MP4InfoExtractor.csproj**：项目配置
  - 目标框架：.NET Framework 4.5
  - 输出类型：WinExe（Windows 应用）
  - 引用：NPinyin.dll + WPF/Windows.Forms 系统库

### 系统引用
- `System.Windows.Forms`：FolderBrowserDialog
- `PresentationFramework`：WPF UI
- `System.Xaml`：XAML 解析

## 数据模型

### SongInfo 类结构
```csharp
public class SongInfo
{
    // 基本信息
    public string Artist { get; set; }              // 艺术家
    public string SongTitle { get; set; }           // 歌曲名称
    public string Language { get; set; }            // 语言
    public string Region { get; set; }              // 地区

    // 节目信息
    public string ProgramName { get; set; }         // 节目名称
    public string EpisodeNumber { get; set; }       // 期数

    // 分类信息
    public string GenderGroupType { get; set; }     // 性别/组合类型
    public string Type { get; set; }                // 类型（现场/动态歌词版）
    public string VideoQuality { get; set; }        // 视频质量

    // 文件信息
    public string OriginalFileName { get; set; }    // 原始文件名
    public string FilePath { get; set; }            // 完整路径
    public string FileExtension { get; set; }       // 文件扩展名

    // 计算字段
    public string SongAbbreviation { get; set; }    // 首字母缩写
    public string SongFullPinyin { get; set; }      // 完整拼音
    public int WordCount { get; set; }              // 字数
}
```

### 解析策略
1. **ParseNewFormat**：新格式解析（`--` 分隔符）
   - 格式：`艺术家 - 歌名--类型--语言--地区--质量`
   - 策略：先分离 `--`，再处理第一部分的 ` - `

2. **TrySimpleParse**：简单解析（回退方案）
   - 格式：`艺术家 - 歌名 - 其他字段...`
   - 策略：按 `-` 分割，尽可能提取字段

3. **ParseOldFormat**（已注释，保留接口）
   - 格式：`艺术家 - 歌名 (节目 第X期) - 类型...`
   - 使用正则表达式匹配

## 测试与质量

### 单元测试
- **TestProgram.cs**：功能测试程序
  - 测试用例覆盖中文、英文、日语歌曲
  - 验证拼音转换准确性
  - 验证缩写生成逻辑
  - 验证字数统计（中英文不同策略）

### 测试数据
- 位于 `../TestFiles/` 目录
  - `动态歌词版/`：包含新格式测试文件
  - `多语言歌曲/`：包含多语言测试文件
  - 根目录：混合格式测试文件

### 质量保证
- **异常处理**：所有解析方法使用 try-catch 包裹
- **线程安全**：`songList` 使用 lock 保护
- **UI 响应性**：扫描使用异步 Task，日志使用 Dispatcher
- **编码规范**：UTF-8 编码，XML 文档注释

## 常见问题 (FAQ)

### Q: 如何添加新的文件格式支持？
A: 在 `ScanFolder` 方法中修改 `supportedExtensions` 数组：
```csharp
var supportedExtensions = new[] {
    "*.mp4", "*.mpg", "*.avi", // ... 现有格式
    "*.新格式"  // 添加新格式
};
```

### Q: 如何自定义解析逻辑？
A: 在 `MainWindow.xaml.cs` 中：
1. 创建新方法 `ParseCustomFormat(string nameWithoutExt, ...)`
2. 在 `ParseFileName` 方法中调用新方法
3. 遵循现有的返回值约定（成功返回 SongInfo，失败返回 null）

### Q: 如何调整 CSV 导出字段？
A:
1. 修改 `ExportToCsv` 方法的头行字符串
2. 调整 `string.Join` 中的字段顺序/内容
3. 如需新字段，先在 `SongInfo` 中添加属性

### Q: 拼音转换不准确怎么办？
A: 检查 `PinyinHelper.cs`：
- NPinyin 库覆盖常见汉字（20000+ 字）
- 对于生僻字，会使用 Unicode 范围估算（`GetFirstLetterByUnicode`）
- 可以在 NPinyin 失败时自定义映射表

### Q: 如何处理特殊文件名？
A: 当前解析器已支持：
- 包含括号的文件名（自动移除括号内容）
- 艺术家合作标记（`&`、`&`）
- 多语言混合歌曲名
- 特殊字符（会在 CSV 导出时转义）

## 相关文件清单

### 核心代码（5 个文件）
- `MainWindow.xaml.cs` (507 行)：主逻辑与 UI 交互
- `SongInfo.cs` (104 行)：数据模型
- `PinyinHelper.cs` (249 行)：拼音转换工具
- `TextHelper.cs` (192 行)：文本处理工具
- `LanguageHelper.cs` (35 行)：语言检测工具

### UI 定义（2 个文件）
- `MainWindow.xaml` (70 行)：主窗口布局
- `App.xaml` (9 行)：应用资源定义

### 配置与项目（3 个文件）
- `App.config` (6 行)：运行时配置
- `packages.config` (4 行)：NuGet 依赖
- `MP4InfoExtractor.csproj` (106 行)：项目配置

### 测试与属性（2 个文件）
- `TestProgram.cs` (66 行)：功能测试程序
- `Properties/AssemblyInfo.cs` (56 行)：程序集元数据

## 变更记录 (Changelog)

### 2025-11-28 14:53:22
- **初始化模块文档**：完成主模块详细文档
- 记录核心类、方法、配置信息
- 添加 FAQ 和文件清单
- 建立与根文档的导航关系

---

**模块维护者**: Claude Code
**最后更新**: 2025-11-28 14:53:22
**代码行数**: ~1200 行（不含生成代码）
