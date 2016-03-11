using PasswordGenerator.Core.Enums;

namespace PasswordGenerator.Core.Models
{
    public class Key
    {
        public const string Uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Lowers = "abcdefghijklmnopqrstuvwxyz";
        public const string Numbers = "0123456789";
        public const string Signals = "!@#$%^&*.?+=";

        public EnumValueRangeType RangeType { get; set; }

        public int ChaosIndex { get; set; }

        public int IndexValue { get; set; }

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
                    valueRange = Signals;
                    break;
                default:
                    valueRange = mixRange;
                    break;
            }

            return valueRange[IndexValue % valueRange.Length];
        }

    }
}