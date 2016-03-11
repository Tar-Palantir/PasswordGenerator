using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PasswordGenerator.Core.Enums;
using PasswordGenerator.Core.Models;

namespace PasswordGenerator.Core
{
    public class Generator
    {

        public static string ModeState2To16(string modeState)
        {
            return Convert.ToInt32(modeState, 2).ToString("X5");
        }

        public static string ModeState10To16()
        {
            return null;
        }

        public static string ModeState16To2()
        {
            return null;
        }

        public static string Generate(string keyword, int length, string modeState)
        {
            StringBuilder sb = new StringBuilder();
            var modes = new Mode[4];
            modes[0] = new Mode(EnumValueRangeType.UpperWord, modeState.Substring(0, 2));
            modes[1] = new Mode(EnumValueRangeType.LowerWord, modeState.Substring(2, 2));
            modes[2] = new Mode(EnumValueRangeType.Number, modeState.Substring(4, 2));
            modes[3] = new Mode(EnumValueRangeType.Signal, modeState.Substring(6, 2));

            if (modes[0].State != EnumChooseState.None)
            {
                sb.Append(Key.Uppers);
            }
            if (modes[1].State != EnumChooseState.None)
            {
                sb.Append(Key.Lowers);
            }
            if (modes[2].State != EnumChooseState.None)
            {
                sb.Append(Key.Numbers);
            }
            if (modes[3].State != EnumChooseState.None)
            {
                var signalsState = modeState.Substring(8);
                for (int i = 0; i < 12; i++)
                {
                    if (signalsState[i] == '1')
                    {
                        sb.Append(Key.Signals[i]);
                    }
                }
            }

            var r = new Random(length);
            var keyList = new List<Key>();
            for (int i = 0; i < length; i++)
            {
                keyList.Insert(r.Next(0, keyList.Count), new Key { ChaosIndex = i, RangeType = EnumValueRangeType.Mixed });
            }

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
                    keyList.Find(p => p.ChaosIndex == currentIndex).RangeType = mode.RangeType;
                    currentIndex++;
                }
            }

            byte[] wordBytes = Encoding.UTF8.GetBytes(keyword);

            List<byte> n16List = new List<byte>();
            var md5 = MD5.Create();
            var n16 = md5.ComputeHash(wordBytes);

            var n16Count = length / 16;
            var offset = (n16Count + 1) * 16 - length;
            n16List.AddRange(n16);

            var extWord = keyword;
            for (int i = 0; i < n16Count; i++)
            {
                extWord += length;
                var extWordBytes = Encoding.UTF8.GetBytes(extWord);
                n16List.AddRange(md5.ComputeHash(extWordBytes));
            }

            byte[] cutN16M = n16List.GetRange(offset, length).ToArray();
            for (int i = 0; i < length; i++)
            {
                var v1 = Convert.ToInt32(cutN16M[i]);
                var v2 = Convert.ToInt32(n16List[i]);
                keyList[i].IndexValue = (v1 + v2);
            }

            var resultSBuilder = new StringBuilder();
            var mixRange = sb.ToString();
            foreach (var key in keyList)
            {
                resultSBuilder.Append(key.GetValue(ref mixRange));
            }

            return resultSBuilder.ToString();
        }
    }
}
