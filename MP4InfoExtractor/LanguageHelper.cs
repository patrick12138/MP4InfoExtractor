using System.Linq;

namespace MP4InfoExtractor
{
    /// <summary>
    /// 语言判断工具类
    /// </summary>
    public static class LanguageHelper
    {
        /// <summary>
        /// 判断是否为中文语言
        /// </summary>
        public static bool IsChineseLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                return false;

            var chineseIndicators = new[] { "国语", "中文", "普通话", "汉语" };
            return chineseIndicators.Any(indicator => language.Contains(indicator));
        }

        /// <summary>
        /// 判断是否为英文语言
        /// </summary>
        public static bool IsEnglishLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                return false;

            var englishIndicators = new[] { "英语", "英文", "English" };
            return englishIndicators.Any(indicator => language.Contains(indicator));
        }
    }
}
