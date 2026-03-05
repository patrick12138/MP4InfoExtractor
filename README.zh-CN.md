# MP4InfoExtractor

[English](./README.md) | [简体中文](./README.zh-CN.md)

MP4InfoExtractor 是一个 Windows 桌面工具（WPF，.NET Framework 4.5），用于扫描视频文件并按文件名规则提取歌曲信息，然后导出为 CSV。

## 项目简介

- 目标：批量将音乐视频文件名解析为结构化歌曲数据。
- 平台：Windows（.NET Framework 4.5）。
- 输出：CSV 报表，便于后续整理、归档或导入。

## 主要功能

- 递归扫描常见视频格式：
  - `*.mp4`, `*.mpg`, `*.mpeg`, `*.avi`, `*.mkv`, `*.mov`, `*.wmv`, `*.flv`, `*.webm`
- 解析命名格式，例如：
  - `艺术家 - 歌曲名--性别/组合--类型--语言--地区--质量`
- 提供文本辅助能力（如首字母/拼音生成支持）。
- 支持导出解析结果为 CSV。

## 目录结构

- `MP4InfoExtractor/`：WPF 主程序源码。
- `packages.config`：NuGet 依赖声明。
- `release/`：本地发布产物（已在 Git 忽略）。

## 环境要求

- Windows
- Visual Studio（建议 2019+）
- .NET Framework 4.5 Developer Pack

## 构建方式

1. 用 Visual Studio 打开 `MP4InfoExtractor/MP4InfoExtractor.csproj`。
2. 执行 NuGet 依赖还原。
3. 在 `Debug` 或 `Release` 模式下编译。

## 使用方式

1. 启动程序。
2. 点击 **Browse** 选择视频文件目录。
3. 点击 **Scan** 进行解析。
4. 点击 **Export CSV** 导出结果。
