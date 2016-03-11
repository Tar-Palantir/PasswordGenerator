using System;
using PasswordGenerator.Core.Enums;

namespace PasswordGenerator.Core.Models
{
    public struct Mode
    {
        public Mode(EnumValueRangeType rangType, string stateStr)
        {
            RangeType = rangType;
            State = (EnumChooseState)Convert.ToInt32(stateStr, 2);
        }

        public EnumValueRangeType RangeType { get; }

        public EnumChooseState State { get; }
    }
}
