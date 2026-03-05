using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPinyin;

namespace MP4InfoExtractor
{
    /// <summary>
    /// 中文拼音转换工具类 - 基于NPinyin库实现
    /// </summary>
    public static class PinyinHelper
    {
        /// <summary>
        /// 获取中文字符的拼音首字母
        /// </summary>
        /// <param name="c">中文字符</param>
        /// <returns>拼音首字母，大写</returns>
        public static string GetFirstLetter(char c)
        {
            if (!IsChinese(c))
            {
                return char.IsLetter(c) ? c.ToString().ToUpper() : "";
            }

            try
            {
                // 使用NPinyin获取拼音首字母
                string pinyin = Pinyin.GetInitials(c.ToString(), Encoding.UTF8);
                return string.IsNullOrEmpty(pinyin) ? "" : pinyin.ToUpper();
            }
            catch (Exception)
            {
                // 如果NPinyin处理失败，使用备用方案
                return GetFirstLetterByUnicode(c);
            }
        }

        /// <summary>
        /// 获取中文字符的完整拼音
        /// </summary>
        /// <param name="c">中文字符</param>
        /// <returns>完整拼音，小写</returns>
        public static string GetFullPinyin(char c)
        {
            if (!IsChinese(c))
            {
                return char.IsLetter(c) ? c.ToString().ToLower() : "";
            }

            try
            {
                // 使用NPinyin获取完整拼音
                string pinyin = Pinyin.GetPinyin(c.ToString(), Encoding.UTF8);
                return string.IsNullOrEmpty(pinyin) ? "" : pinyin.ToLower().Trim();
            }
            catch (Exception)
            {
                // 如果NPinyin处理失败，返回空字符串
                return "";
            }
        }

        /// <summary>
        /// 获取字符串的拼音首字母缩写
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>拼音首字母缩写，大写</returns>
        public static string GetFirstLetters(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            try
            {
                // 使用NPinyin批量处理
                string initials = Pinyin.GetInitials(text, Encoding.UTF8);
                return initials.ToUpper();
            }
            catch (Exception)
            {
                // 备用方案：逐字符处理
                var result = new StringBuilder();
                foreach (char c in text)
                {
                    if (IsChinese(c))
                    {
                        result.Append(GetFirstLetter(c));
                    }
                    else if (char.IsLetter(c))
                    {
                        result.Append(char.ToUpper(c));
                    }
                }
                return result.ToString();
            }
        }

        /// <summary>
        /// 获取字符串的完整拼音
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>完整拼音，用空格分隔</returns>
        public static string GetFullPinyin(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            try
            {
                // 使用NPinyin批量处理
                string pinyin = Pinyin.GetPinyin(text, Encoding.UTF8);
                return pinyin.ToLower().Trim();
            }
            catch (Exception)
            {
                // 备用方案：逐字符处理
                var result = new StringBuilder();
                foreach (char c in text)
                {
                    if (IsChinese(c))
                    {
                        var charPinyin = GetFullPinyin(c);
                        if (!string.IsNullOrEmpty(charPinyin))
                        {
                            result.Append(charPinyin);
                            result.Append(" ");
                        }
                    }
                    else if (char.IsLetter(c))
                    {
                        result.Append(c.ToString().ToLower());
                        result.Append(" ");
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        result.Append(" ");
                    }
                }
                return result.ToString().Trim();
            }
        }

        /// <summary>
        /// 判断字符是否为中文
        /// </summary>
        /// <param name="c">字符</param>
        /// <returns>是否为中文字符</returns>
        public static bool IsChinese(char c)
        {
            return c >= 0x4e00 && c <= 0x9fff;
        }

        /// <summary>
        /// 通过Unicode范围估算拼音首字母（备用方案）
        /// </summary>
        /// <param name="c">中文字符</param>
        /// <returns>估算的拼音首字母</returns>
        private static string GetFirstLetterByUnicode(char c)
        {
            int code = (int)c;
            
            if (code >= 0x4e00 && code <= 0x4fff) return "A"; 
            if (code >= 0x5000 && code <= 0x51ff) return "B"; 
            if (code >= 0x5200 && code <= 0x53ff) return "C"; 
            if (code >= 0x5400 && code <= 0x55ff) return "D"; 
            if (code >= 0x5600 && code <= 0x57ff) return "F"; 
            if (code >= 0x5800 && code <= 0x59ff) return "G"; 
            if (code >= 0x5a00 && code <= 0x5bff) return "H"; 
            if (code >= 0x5c00 && code <= 0x5dff) return "J"; 
            if (code >= 0x5e00 && code <= 0x5fff) return "K"; 
            if (code >= 0x6000 && code <= 0x61ff) return "L"; 
            if (code >= 0x6200 && code <= 0x63ff) return "M"; 
            if (code >= 0x6400 && code <= 0x65ff) return "N"; 
            if (code >= 0x6600 && code <= 0x67ff) return "P"; 
            if (code >= 0x6800 && code <= 0x69ff) return "Q"; 
            if (code >= 0x6a00 && code <= 0x6bff) return "R"; 
            if (code >= 0x6c00 && code <= 0x6dff) return "S"; 
            if (code >= 0x6e00 && code <= 0x6fff) return "T"; 
            if (code >= 0x7000 && code <= 0x71ff) return "W"; 
            if (code >= 0x7200 && code <= 0x73ff) return "X"; 
            if (code >= 0x7400 && code <= 0x75ff) return "Y"; 
            if (code >= 0x7600 && code <= 0x9fff) return "Z"; 
            
            return "";
        }

        /// <summary>
        /// 获取字符串的拼音首字母，去除重复
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>去重后的拼音首字母</returns>
        public static string GetFirstLettersUnique(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            var letters = GetFirstLetters(text);
            var uniqueLetters = new StringBuilder();
            var seen = new HashSet<char>();

            foreach (char c in letters)
            {
                if (char.IsLetter(c) && !seen.Contains(c))
                {
                    uniqueLetters.Append(c);
                    seen.Add(c);
                }
            }

            return uniqueLetters.ToString();
        }

        /// <summary>
        /// 获取字符串的拼音，不带声调
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>无声调拼音</returns>
        public static string GetPinyinWithoutTone(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            try
            {
                // NPinyin默认就是无声调的
                return GetFullPinyin(text);
            }
            catch (Exception)
            {
                return GetFullPinyin(text);
            }
        }

        /// <summary>
        /// 检查文本是否包含中文字符
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>是否包含中文</returns>
        public static bool ContainsChinese(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return text.Any(IsChinese);
        }
    }
}
