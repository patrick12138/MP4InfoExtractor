using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Globalization;

namespace MP4InfoExtractor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<SongInfo> songList = new List<SongInfo>();
        private bool isScanning = false;

        public MainWindow()
        {
            InitializeComponent();
            LogMessage("程序启动完成，请选择包含MP4歌曲文件的文件夹。");
        }

        /// <summary>
        /// 浏览文件夹按钮事件
        /// </summary>
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog()
            {
                Description = "请选择包含MP4歌曲文件的文件夹",
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFolderPath.Text = dialog.SelectedPath;
                btnScan.IsEnabled = true;
                LogMessage($"已选择文件夹: {dialog.SelectedPath}");
            }
        }

        /// <summary>
        /// 开始扫描按钮事件
        /// </summary>
        private async void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            if (isScanning || string.IsNullOrEmpty(txtFolderPath.Text))
                return;

            try
            {
                isScanning = true;
                SetUIState(false);
                UpdateStatus("正在扫描...");
                
                songList.Clear();
                txtLog.Clear();
                lblFileCount.Text = "找到 0 个文件";

                LogMessage("开始扫描文件夹...");
                
                var folderPath = txtFolderPath.Text;
                await Task.Run(() => ScanFolder(folderPath));

                // 在UI线程上更新界面
                LogMessage($"扫描完成！共找到 {songList.Count} 首歌曲。");
                UpdateStatus($"扫描完成，找到 {songList.Count} 首歌曲");
                lblFileCount.Text = $"找到 {songList.Count} 个文件";

                if (songList.Count > 0)
                {
                    btnExport.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"扫描过程中发生错误: {ex.Message}");
                UpdateStatus("扫描失败");
            }
            finally
            {
                isScanning = false;
                SetUIState(true);
            }
        }

        /// <summary>
        /// 导出CSV按钮事件
        /// </summary>
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (songList.Count == 0)
            {
                System.Windows.MessageBox.Show("没有可导出的数据，请先进行扫描。", "提示", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "CSV文件 (*.csv)|*.csv",
                FileName = $"歌曲列表_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                DefaultExt = "csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    ExportToCsv(saveDialog.FileName);
                    LogMessage($"CSV文件导出成功: {saveDialog.FileName}");
                    System.Windows.MessageBox.Show("CSV文件导出成功！", "成功", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    LogMessage($"导出CSV文件时发生错误: {ex.Message}");
                    System.Windows.MessageBox.Show($"导出失败: {ex.Message}", "错误", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 扫描文件夹
        /// </summary>
        private void ScanFolder(string folderPath)
        {
            try
            {
                // 支持多种视频文件格式
                var supportedExtensions = new[] { "*.mp4", "*.mpg", "*.mpeg", "*.avi", "*.mkv", "*.mov", "*.wmv", "*.flv", "*.webm" };
                var allFiles = new List<string>();
                
                foreach (var extension in supportedExtensions)
                {
                    try
                    {
                        var files = Directory.GetFiles(folderPath, extension, SearchOption.AllDirectories);
                        allFiles.AddRange(files);
                    }
                    catch (Exception ex)
                    {
                        // 使用Dispatcher在UI线程上记录日志
                        this.Dispatcher.Invoke(() => LogMessage($"扫描 {extension} 文件时出错: {ex.Message}"));
                    }
                }
                
                // 在UI线程上记录找到的文件数量
                this.Dispatcher.Invoke(() => LogMessage($"在文件夹中找到 {allFiles.Count} 个视频文件"));

                foreach (var file in allFiles)
                {
                    try
                    {
                        var fileName = Path.GetFileName(file);
                        var songInfo = ParseFileName(fileName, file);
                        
                        if (songInfo != null)
                        {
                            lock (songList)
                            {
                                songList.Add(songInfo);
                            }
                            
                            // 在UI线程上更新界面
                            this.Dispatcher.Invoke(() => 
                            {
                                LogMessage($"解析成功: {songInfo.Artist} - {songInfo.SongTitle}");
                                lblFileCount.Text = $"找到 {songList.Count} 个文件";
                            });
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => LogMessage($"解析失败: {fileName}"));
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(() => LogMessage($"处理文件时出错 {file}: {ex.Message}"));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() => LogMessage($"扫描文件夹时出错: {ex.Message}"));
            }
        }

        /// <summary>
        /// 解析文件名
        /// </summary>
        private SongInfo ParseFileName(string fileName, string filePath)
        {
            try
            {
                // 移除扩展名
                var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                var extension = Path.GetExtension(fileName);
                
                // 尝试新格式：艺术家 - 歌曲名称--性别/组合类型--类型--语言--地区--质量
                if (nameWithoutExt.Contains("--"))
                {
                    return ParseNewFormat(nameWithoutExt, fileName, filePath, extension);
                }
                else
                {
                    // 尝试简单解析
                    return TrySimpleParse(nameWithoutExt, fileName, filePath, extension);
                }
            }
            catch (Exception ex)
            {
                // 注意：这里不能直接调用Dispatcher，因为可能在后台线程中
                // 这个错误会在调用方法中处理
                return null;
            }
        }

        /// <summary>
        /// 解析新格式文件名（使用--分隔符）
        /// </summary>
        private SongInfo ParseNewFormat(string nameWithoutExt, string fileName, string filePath, string extension)
        {
            try
            {
                // 首先分离艺术家和歌曲名称部分
                var firstPart = nameWithoutExt.Split(new string[] { "--" }, StringSplitOptions.None);
                if (firstPart.Length < 5)
                {
                    return null; // 不符合新格式
                }

                // 解析艺术家和歌曲名称
                var artistSongPart = firstPart[0].Trim();
                var dashIndex = artistSongPart.IndexOf(" - ");
                if (dashIndex == -1)
                {
                    return null;
                }

                var artist = artistSongPart.Substring(0, dashIndex).Trim();
                var songTitle = artistSongPart.Substring(dashIndex + 3).Trim();

                // 不再解析节目信息，保留完整的歌曲名称
                var songInfo = new SongInfo
                {
                    Artist = artist,
                    SongTitle = songTitle,
                    ProgramName = "", // 暂时不解析
                    EpisodeNumber = "", // 暂时不解析
                    GenderGroupType = firstPart.Length > 1 ? firstPart[1].Trim() : "",
                    Type = firstPart.Length > 2 ? firstPart[2].Trim() : "",
                    Language = firstPart.Length > 3 ? firstPart[3].Trim() : "",
                    Region = firstPart.Length > 4 ? firstPart[4].Trim() : "",
                    VideoQuality = firstPart.Length > 5 ? firstPart[5].Trim() : "",
                    OriginalFileName = fileName,
                    FilePath = filePath,
                    FileExtension = extension
                };

                // 生成歌曲首字母缩写、全拼音和统计字数
                var cleanSongTitle = TextHelper.RemoveParenthesesContent(songInfo.SongTitle);
                songInfo.SongAbbreviation = TextHelper.GenerateAbbreviation(cleanSongTitle, songInfo.Language);
                songInfo.SongFullPinyin = TextHelper.GenerateFullPinyin(cleanSongTitle, songInfo.Language);
                songInfo.WordCount = TextHelper.CountWords(cleanSongTitle, songInfo.Language);

                return songInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 解析旧格式文件名（使用空格-分隔符）
        /// </summary>
        private SongInfo ParseOldFormat(string nameWithoutExt, string fileName, string filePath, string extension)
        {
            try
            {
                // 正则表达式模式，用于匹配文件名格式
                // [艺术家] - [歌曲名称] ([节目名称] 第[期数]期) - [性别/组合类型] - 现场 - [语言] - [地区] - 质量
                var pattern = @"^(.+?)\s*-\s*(.+?)\s*\((.+?)\s*第(\d+)期\)\s*-\s*(.+?)\s*-\s*(.+?)\s*-\s*(.+?)\s*-\s*(.+?)\s*-\s*(\w+)$";
                
                var match = Regex.Match(nameWithoutExt, pattern, RegexOptions.IgnoreCase);
                
                if (match.Success)
                {
                    var songInfo = new SongInfo
                    {
                        Artist = match.Groups[1].Value.Trim(),
                        SongTitle = match.Groups[2].Value.Trim(),
                        ProgramName = match.Groups[3].Value.Trim(),
                        EpisodeNumber = match.Groups[4].Value.Trim(),
                        GenderGroupType = match.Groups[5].Value.Trim(),
                        Type = match.Groups[6].Value.Trim(), // 现场
                        Language = match.Groups[7].Value.Trim(),
                        Region = match.Groups[8].Value.Trim(),
                        VideoQuality = match.Groups[9].Value.Trim(),
                        OriginalFileName = fileName,
                        FilePath = filePath,
                        FileExtension = extension
                    };

                    // 生成歌曲首字母缩写、全拼音和统计字数
                    var cleanSongTitle = TextHelper.RemoveParenthesesContent(songInfo.SongTitle);
                    songInfo.SongAbbreviation = TextHelper.GenerateAbbreviation(cleanSongTitle, songInfo.Language);
                    songInfo.SongFullPinyin = TextHelper.GenerateFullPinyin(cleanSongTitle, songInfo.Language);
                    songInfo.WordCount = TextHelper.CountWords(cleanSongTitle, songInfo.Language);

                    return songInfo;
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 简单解析（回退方案）
        /// </summary>
        private SongInfo TrySimpleParse(string nameWithoutExt, string fileName, string filePath, string extension)
        {
            try
            {
                var parts = nameWithoutExt.Split('-');
                if (parts.Length >= 2)
                {
                    var songInfo = new SongInfo
                    {
                        OriginalFileName = fileName,
                        FilePath = filePath,
                        FileExtension = extension
                    };

                    // 提取艺术家
                    songInfo.Artist = parts[0].Trim();

                    // 提取歌曲名称（可能包含节目信息）
                    var songTitlePart = parts[1].Trim();
                    var openParenIndex = songTitlePart.IndexOf('(');
                    if (openParenIndex > 0)
                    {
                        songInfo.SongTitle = songTitlePart.Substring(0, openParenIndex).Trim();
                        
                        // 尝试提取节目信息
                        var programPart = songTitlePart.Substring(openParenIndex);
                        var programMatch = Regex.Match(programPart, @"\((.+?)\s*第(\d+)期\)");
                        if (programMatch.Success)
                        {
                            songInfo.ProgramName = programMatch.Groups[1].Value.Trim();
                            songInfo.EpisodeNumber = programMatch.Groups[2].Value.Trim();
                        }
                    }
                    else
                    {
                        songInfo.SongTitle = songTitlePart;
                    }

                    // 尝试提取其他信息
                    if (parts.Length > 2) songInfo.GenderGroupType = parts[2].Trim();
                    if (parts.Length > 3) songInfo.Type = parts[3].Trim();
                    if (parts.Length > 4) songInfo.Language = parts[4].Trim();
                    if (parts.Length > 5) songInfo.Region = parts[5].Trim();
                    if (parts.Length > 6) songInfo.VideoQuality = parts[6].Trim();

                    // 生成歌曲首字母缩写、全拼音和统计字数
                    var cleanSongTitle = TextHelper.RemoveParenthesesContent(songInfo.SongTitle);
                    songInfo.SongAbbreviation = TextHelper.GenerateAbbreviation(cleanSongTitle, songInfo.Language);
                    songInfo.SongFullPinyin = TextHelper.GenerateFullPinyin(cleanSongTitle, songInfo.Language);
                    songInfo.WordCount = TextHelper.CountWords(cleanSongTitle, songInfo.Language);

                    return songInfo;
                }
            }
            catch
            {
                // 忽略简单解析的错误
            }

            return null;
        }

        /// <summary>
        /// 导出到CSV文件
        /// </summary>
        private void ExportToCsv(string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // 写入CSV头部 - 按照新要求：文件名,歌手,歌名,语言,笔画,拼音,字数,视频画面
                writer.WriteLine("文件名,歌手,歌名,语言,笔画,拼音,字数,视频画面");

                // 写入数据
                foreach (var song in songList)
                {
                    var line = string.Join(",",
                        EscapeCsvField(song.OriginalFileName),
                        EscapeCsvField(song.Artist),
                        EscapeCsvField(song.SongTitle),
                        EscapeCsvField(song.Language),
                        "0", // 笔画默认为0，作为占位符
                        EscapeCsvField(song.SongAbbreviation), // 使用拼音首字母
                        EscapeCsvField(song.WordCount.ToString()),
                        EscapeCsvField(song.Type)); // 视频画面使用之前的类型字段
                     
                    writer.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// 转义CSV字段
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "\"\"";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }

        /// <summary>
        /// 记录日志消息
        /// </summary>
        private void LogMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logEntry = $"[{timestamp}] {message}\r\n";
            
            if (txtLog.Dispatcher.CheckAccess())
            {
                txtLog.AppendText(logEntry);
                txtLog.ScrollToEnd();
            }
            else
            {
                txtLog.Dispatcher.Invoke(() =>
                {
                    txtLog.AppendText(logEntry);
                    txtLog.ScrollToEnd();
                });
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        private void UpdateStatus(string status)
        {
            if (lblStatus.Dispatcher.CheckAccess())
            {
                lblStatus.Text = status;
                lblStatusBar.Text = status;
            }
            else
            {
                lblStatus.Dispatcher.Invoke(() =>
                {
                    lblStatus.Text = status;
                    lblStatusBar.Text = status;
                });
            }
        }

        /// <summary>
        /// 设置UI状态
        /// </summary>
        private void SetUIState(bool enabled)
        {
            if (Dispatcher.CheckAccess())
            {
                btnBrowse.IsEnabled = enabled;
                btnScan.IsEnabled = enabled && !string.IsNullOrEmpty(txtFolderPath.Text);
                if (!enabled)
                    btnExport.IsEnabled = false;
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    btnBrowse.IsEnabled = enabled;
                    btnScan.IsEnabled = enabled && !string.IsNullOrEmpty(txtFolderPath.Text);
                    if (!enabled)
                        btnExport.IsEnabled = false;
                });
            }
        }
    }
}
