using System.Text;

namespace LemonFramework.Format
{
    public class NumberFormatter
    {
        public static StringBuilder GetThousandNumber(long value)
        {
            return GetSplitNumber(value, 3);
        }

        public static StringBuilder GetTenThousandNumber(long value)
        {
            return GetSplitNumber(value, 4);
        }

        public static StringBuilder GetSplitNumber(long value, int count, char split = ',')
        {
            var sb = StringUtility.GetSharedStringBuilder();
            if (value <= 0)
            {
                return sb;
            }

            var index = 0;
            while (value > 0)
            {
                ++index;
                //get the last digit
                long digit = value % 10;
                //insert the digit to the head of the StringBuilder
                sb.Insert(0, (char)('0' + digit));
                //remove the last digit
                value /= 10;
                if (index % count == 0 && value > 0)
                {
                    sb.Insert(0, split);
                }
            }

            return sb;
        }
    }
}