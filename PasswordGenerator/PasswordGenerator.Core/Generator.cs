using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PasswordGenerator.Core.Enums;
using PasswordGenerator.Core.Models;

namespace PasswordGenerator.Core
{
    /// <summary>
    /// 生成器
    /// </summary>
    public class Generator
    {
        #region 公共方法

        /// <summary>
        /// 模式状态二进制转十六进制
        /// </summary>
        /// <param name="modeState">模式状态码，二进制</param>
        /// <returns></returns>
        public static string ModeStateOctToHex(string modeState)
        {
            return Convert.ToInt32(modeState, 2).ToString("X5");
        }

        /// <summary>
        /// 模式状态十六进制转二进制
        /// </summary>
        /// <param name="modeState">模式状态码，十六进制</param>
        /// <returns></returns>
        public static string ModeStateHexToOct(string modeState)
        {
            var octSb = new StringBuilder(Convert.ToString(Convert.ToInt32(modeState, 16), 2));
            //保证20位长度输出
            while (octSb.Length < 20)
            {
                octSb.Insert(0, "0");
            }
            return octSb.ToString();
        }

        /// <summary>
        /// 将十六进制模式状态转成模式类型集合
        /// </summary>
        /// <param name="modeState">模式状态码，十六进制</param>
        /// <returns>模式类型集合</returns>
        public static List<Mode> GetModesFromHex(string modeState)
        {
            modeState = ModeStateHexToOct(modeState);
            return GetModesFromOct(modeState);
        }

        /// <summary>
        /// 将二进制模式状态转成模式类型集合
        /// </summary>
        /// <param name="modeState">模式状态码，二进制</param>
        /// <returns>模式类型集合</returns>
        public static List<Mode> GetModesFromOct(string modeState)
        {
            return new List<Mode>
            {
                new Mode(EnumValueRangeType.UpperWord, modeState.Substring(0, 2)),
                new Mode(EnumValueRangeType.LowerWord, modeState.Substring(2, 2)),
                new Mode(EnumValueRangeType.Number, modeState.Substring(4, 2)),
                new Mode(EnumValueRangeType.Signal, modeState.Substring(6, 2))
            };
        }

        /// <summary>
        /// 将二进制模式状态中的子符号提取
        /// 获取选中字符集
        /// </summary>
        /// <param name="modeState">模式状态码，二进制</param>
        /// <returns>选中字符集</returns>
        public static string GetSignalsFromOct(string modeState)
        {
            StringBuilder sb = new StringBuilder();
            var signalsState = modeState.Substring(8);
            for (int i = 0; i < 12; i++)
            {
                if (signalsState[i] == '1')
                {
                    sb.Append(Key.Signals[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成密码
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="length">密码长度</param>
        /// <param name="modeState">模式状态码，十六进制</param>
        /// <returns>密码</returns>
        public static string Generate(string keyword, int length, string modeState)
        {
            modeState = ModeStateHexToOct(modeState);
            var modes = GetModesFromOct(modeState);

            //构建字符混合选择列表
            var mixRange = GetMixString(modes, modeState);

            //获取Key序列
            var keyList = GetKeyList(modes, length);

            //Key值处理
            KeyValueCalculate(keyword, length, modeState, keyList);

            var resultSBuilder = new StringBuilder();
            foreach (var key in keyList)
            {
                //根据Key的类型选择出对应字符
                resultSBuilder.Append(key.GetValue(ref mixRange));
            }

            return resultSBuilder.ToString();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 构建字符混合选择列表
        /// </summary>
        /// <param name="modes">模式集合</param>
        /// <param name="modeState">模式状态</param>
        /// <returns>混合选择列表</returns>
        private static string GetMixString(IList<Mode> modes, string modeState)
        {
            StringBuilder mixStrBuilder = new StringBuilder();
            if (modes.First(p => p.RangeType == EnumValueRangeType.UpperWord).State != EnumChooseState.None)
            {
                mixStrBuilder.Append(Key.Uppers);
            }
            if (modes.First(p => p.RangeType == EnumValueRangeType.LowerWord).State != EnumChooseState.None)
            {
                mixStrBuilder.Append(Key.Lowers);
            }
            if (modes.First(p => p.RangeType == EnumValueRangeType.Number).State != EnumChooseState.None)
            {
                mixStrBuilder.Append(Key.Numbers);
            }

            var signalMode = modes.First(p => p.RangeType == EnumValueRangeType.Signal);
            if (signalMode.State != EnumChooseState.None)
            {
                var signals = GetSignalsFromOct(modeState);
                if (signalMode.State == EnumChooseState.Must && string.IsNullOrEmpty(signals))
                {
                    throw new Exception("Sorry, you have to select at least one signal if you chosen \"Must\"");
                }
                mixStrBuilder.Append(signals);
            }

            return mixStrBuilder.ToString();
        }

        /// <summary>
        /// 获取字符序列
        /// </summary>
        /// <param name="modes">模式集合</param>
        /// <param name="length">长度</param>
        /// <returns>字符序列</returns>
        private static List<Key> GetKeyList(IList<Mode> modes, int length)
        {
            //以随机方式生成乱序数列，用固定种子值确保每次随机数列的一致性
            var r = new Random(length);
            var keyList = new List<Key>();
            for (int i = 0; i < length; i++)
            {
                //默认生成的Key的字符值类型为混合
                keyList.Insert(r.Next(0, keyList.Count), new Key { ChaosIndex = i, RangeType = EnumValueRangeType.Mixed });
            }

            //将必选字符的位置在乱序数列中标出
            var mustCount = modes.Count(p => p.State == EnumChooseState.Must);
            if (mustCount != 0)
            {
                if (length < mustCount)
                {
                    throw new Exception("必选类型数量超过总数量");
                }

                var currentIndex = 0;
                foreach (var mode in modes.Where(p => p.State == EnumChooseState.Must))
                {
                    //将对应索引处的Key标示为必选的数值范围
                    keyList.Find(p => p.ChaosIndex == currentIndex).RangeType = mode.RangeType;
                    currentIndex++;
                }
            }

            return keyList;
        }

        /// <summary>
        /// 字符值计算
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="length">长度</param>
        /// <param name="modeState">模式状态</param>
        /// <param name="keyList">字符序列</param>
        private static void KeyValueCalculate(string keyword, int length, string modeState, List<Key> keyList)
        {
            byte[] wordBytes = Encoding.UTF8.GetBytes(keyword);

            //获取信息摘要
            List<byte> n16List = new List<byte>();
            var md5 = MD5.Create();
            var n16 = md5.ComputeHash(wordBytes);

            var n16Count = length / 16;
            var offset = (n16Count + 1) * 16 - length;
            n16List.AddRange(n16);

            //密码长度超过16的时候，扩展信息进行补长
            var extWord = keyword;
            for (int i = 0; i < n16Count; i++)
            {
                extWord += length + modeState.ToUpperInvariant();
                var extWordBytes = Encoding.UTF8.GetBytes(extWord);
                n16List.AddRange(md5.ComputeHash(extWordBytes));
            }

            //将列表进行错位相加，取重合部分，重合长度为密码长度
            byte[] cutN16M = n16List.GetRange(offset, length).ToArray();
            for (int i = 0; i < length; i++)
            {
                var v1 = Convert.ToInt32(cutN16M[i]);
                var v2 = Convert.ToInt32(n16List[i]);
                keyList[i].IndexValue = (v1 + v2);
            }
        }

        #endregion
    }
}
