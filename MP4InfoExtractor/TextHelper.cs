using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MP4InfoExtractor
{
    /// <summary>
    /// 文本处理工具类
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        /// 移除括号内容
        /// </summary>
        public static string RemoveParenthesesContent(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // 移除各种括号及其内容
            var result = text;
            result = Regex.Replace(result, @"\([^)]*\)", ""); // 圆括号
            result = Regex.Replace(result, @"\[[^\]]*\]", ""); // 方括号
            result = Regex.Replace(result, @"\{[^}]*\}", ""); // 花括号
            result = Regex.Replace(result, @"（[^）]*）", ""); // 中文圆括号
            
            return result.Trim();
        }

        /// <summary>
        /// 统计字数
        /// </summary>
        public static int CountWords(string text, string language)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            try
            {
                if (LanguageHelper.IsChineseLanguage(language))
                {
                    // 中文按字符计算
                    return text.Where(c => PinyinHelper.IsChinese(c)).Count();
                }
                else
                {
                    // 其他语言按单词计算
                    var words = text.Split(new char[] { ' ', '-', '_', '.', '&', '\'', '"' }, 
                        StringSplitOptions.RemoveEmptyEntries);
                    return words.Where(word => !string.IsNullOrWhiteSpace(word) && 
                        word.Any(c => char.IsLetter(c))).Count();
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 生成歌曲首字母缩写
        /// </summary>
        public static string GenerateAbbreviation(string songTitle, string language)
        {
            if (string.IsNullOrEmpty(songTitle))
                return "";

            try
            {
                // 判断语言类型
                if (LanguageHelper.IsChineseLanguage(language))
                {
                    return GenerateChineseAbbreviation(songTitle);
                }
                else if (LanguageHelper.IsEnglishLanguage(language))
                {
                    return GenerateEnglishAbbreviation(songTitle);
                }
                else
                {
                    // 其他语言不生成缩写
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 生成全拼音
        /// </summary>
        public static string GenerateFullPinyin(string songTitle, string language)
        {
            if (string.IsNullOrEmpty(songTitle))
                return "";

            try
            {
                if (LanguageHelper.IsChineseLanguage(language))
                {
                    return PinyinHelper.GetFullPinyin(songTitle);
                }
                else
                {
                    // 非中文歌曲返回原文
                    return songTitle;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 生成中文歌曲名称首字母缩写
        /// </summary>
        private static string GenerateChineseAbbreviation(string songTitle)
        {
            return PinyinHelper.GetFirstLetters(songTitle);
        }

        /// <summary>
        /// 生成英文歌曲名称首字母缩写
        /// </summary>
        private static string GenerateEnglishAbbreviation(string songTitle)
        {
            var result = new System.Text.StringBuilder();
            
            // 更彻底地清理文本，移除多余空格和特殊字符
            var cleanedTitle = Regex.Replace(songTitle, @"[^\w\s\-\.&']", " ");
            cleanedTitle = Regex.Replace(cleanedTitle, @"\s+", " ").Trim();
            
            // 分割单词
            var words = cleanedTitle.Split(new char[] { ' ', '-', '_', '.', '(', ')', '[', ']', '&', '\'', ',' , ';', ':' }, 
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                // 清理单词中的特殊字符
                var cleanWord = Regex.Replace(word, @"[^\w]", "").Trim();
                
                if (!string.IsNullOrEmpty(cleanWord) && cleanWord.All(c => char.IsLetter(c)))
                {
                    // 转为小写进行虚词检查
                    var lowerWord = cleanWord.ToLower();
                    
                    // 过滤常见的虚词（可选）
                    if (!IsCommonStopWord(lowerWord))
                    {
                        result.Append(char.ToUpper(cleanWord[0]));
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 判断是否为常见虚词（针对歌曲标题优化）
        /// </summary>
        private static bool IsCommonStopWord(string word)
        {
            // 针对歌曲标题缩写生成优化的停用词列表
            // 只包含在歌曲标题中通常不重要的虚词
            var stopWords = new[] { 
                "a", "an", "the", "of", "and", "or", "but", "for", "with", "by", "from",
                "to", "at", "in", "on", "as", "vs", "versus", "per", "en", "de"
            };
            return stopWords.Contains(word);
        }
        
        /// <summary>
        /// 用于测试的公共方法，暴露私有方法
        /// </summary>
        public static string TestGenerateEnglishAbbreviation(string songTitle)
        {
            return GenerateEnglishAbbreviation(songTitle);
        }

        /// <summary>
        /// 用于测试的公共方法，暴露私有方法
        /// </summary>
        public static bool IsCommonStopWordForTest(string word)
        {
            return IsCommonStopWord(word);
        }
    }
}
