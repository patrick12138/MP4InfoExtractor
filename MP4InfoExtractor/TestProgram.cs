using System;
using System.IO;

namespace MP4InfoExtractor
{
    /// <summary>
    /// 简单的功能测试程序
    /// </summary>
    class TestProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MP4InfoExtractor 功能测试 ===");
            Console.WriteLine();

            // 测试中文歌曲
            TestSong("我爱中国", "国语");
            TestSong("月半小夜曲", "国语");
            TestSong("有趣", "国语");
            TestSong("落叶归根", "国语");
            TestSong("天堂·煎熬", "国语");
            TestSong("燃烧", "国语");

            Console.WriteLine();

            // 测试英文歌曲
            TestSong("Water", "英语");
            TestSong("Just Like Fire", "英语");
            TestSong("Better Than You Left Me", "英语");
            TestSong("Never Enough", "英语");
            TestSong("crazy girl", "英语");

            Console.WriteLine();

            // 测试其他语言
            TestSong("桜", "日语");

            Console.WriteLine();
            Console.WriteLine("测试完成！");
            Console.ReadKey();
        }

        static void TestSong(string songTitle, string language)
        {
            // 移除括号内容
            var cleanTitle = TextHelper.RemoveParenthesesContent(songTitle);
            
            // 生成首字母缩写
            var abbreviation = TextHelper.GenerateAbbreviation(cleanTitle, language);
            
            // 生成全拼音
            var fullPinyin = TextHelper.GenerateFullPinyin(cleanTitle, language);
            
            // 统计字数
            var wordCount = TextHelper.CountWords(cleanTitle, language);

            Console.WriteLine($"歌曲名称: {songTitle}");
            Console.WriteLine($"语言: {language}");
            Console.WriteLine($"首字母缩写: {abbreviation}");
            Console.WriteLine($"全拼音: {fullPinyin}");
            Console.WriteLine($"字数: {wordCount}");
            Console.WriteLine("---");
        }
    }
}
