using System.Text.RegularExpressions;
using PasswordGenerator.Core.Enums;

namespace PasswordGenerator.Core.Models
{
    /// <summary>
    /// 字符
    /// </summary>
    public class Key
    {
        /// <summary>
        /// 大写字母
        /// </summary>
        public const string Uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// 小写字母
        /// </summary>
        public const string Lowers = "abcdefghijklmnopqrstuvwxyz";
        /// <summary>
        /// 数字
        /// </summary>
        public const string Numbers = "0123456789";
        /// <summary>
        /// 符号
        /// </summary>
        public const string Signals = "!@#$%^&*.?+=";

        /// <summary>
        /// 取值范围类型
        /// </summary>
        public EnumValueRangeType RangeType { get; set; }

        /// <summary>
        /// 乱序索引
        /// </summary>
        public int ChaosIndex { get; set; }

        /// <summary>
        /// 取值用索引值
        /// </summary>
        public int IndexValue { get; set; }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="mixRange">混合取值</param>
        /// <returns>字符</returns>
        public char GetValue(ref string mixRange)
        {
            string valueRange;
            switch (RangeType)
            {
                case EnumValueRangeType.UpperWord:
                    valueRange = Uppers;
                    break;
                case EnumValueRangeType.LowerWord:
                    valueRange = Lowers;
                    break;
                case EnumValueRangeType.Number:
                    valueRange = Numbers;
                    break;
                case EnumValueRangeType.Signal:
                    //可选符号要从混合中取,因为混合中的符号是用户选择可用的符号
                    valueRange = Regex.Match(mixRange, @"[^A-Za-z\d]+$").Value;
                    break;
                default:
                    valueRange = mixRange;
                    break;
            }

            return valueRange[IndexValue % valueRange.Length];
        }

    }
}