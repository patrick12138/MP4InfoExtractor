using System;

namespace MP4InfoExtractor
{
    /// <summary>
    /// 歌曲信息数据模型
    /// </summary>
    public class SongInfo
    {
        /// <summary>
        /// 艺术家
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// 歌曲名称
        /// </summary>
        public string SongTitle { get; set; }

        /// <summary>
        /// 节目名称
        /// </summary>
        public string ProgramName { get; set; }

        /// <summary>
        /// 期数
        /// </summary>
        public string EpisodeNumber { get; set; }

        /// <summary>
        /// 性别/组合类型
        /// </summary>
        public string GenderGroupType { get; set; }

        /// <summary>
        /// 类型（现场/动态歌词版/动画等）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 视频质量
        /// </summary>
        public string VideoQuality { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// 歌曲首字母缩写
        /// </summary>
        public string SongAbbreviation { get; set; }

        /// <summary>
        /// 歌曲全拼音
        /// </summary>
        public string SongFullPinyin { get; set; }

        /// <summary>
        /// 歌曲字数
        /// </summary>
        public int WordCount { get; set; }

        public SongInfo()
        {
            Artist = string.Empty;
            SongTitle = string.Empty;
            ProgramName = string.Empty;
            EpisodeNumber = string.Empty;
            GenderGroupType = string.Empty;
            Type = string.Empty;
            Language = string.Empty;
            Region = string.Empty;
            VideoQuality = string.Empty;
            OriginalFileName = string.Empty;
            FilePath = string.Empty;
            FileExtension = string.Empty;
            SongAbbreviation = string.Empty;
            SongFullPinyin = string.Empty;
            WordCount = 0;
        }
    }
}
