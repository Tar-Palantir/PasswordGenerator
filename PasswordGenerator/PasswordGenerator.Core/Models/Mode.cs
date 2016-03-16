using System;
using PasswordGenerator.Core.Enums;

namespace PasswordGenerator.Core.Models
{
    /// <summary>
    /// 模式
    /// </summary>
    public struct Mode
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rangType">取值范围</param>
        /// <param name="stateStr">状态字符串</param>
        public Mode(EnumValueRangeType rangType, string stateStr)
        {
            RangeType = rangType;
            State = (EnumChooseState)Convert.ToInt32(stateStr, 2);
        }

        /// <summary>
        /// 取值范围
        /// </summary>
        public EnumValueRangeType RangeType { get; }

        /// <summary>
        /// 状态
        /// </summary>
        public EnumChooseState State { get; }
    }
}
