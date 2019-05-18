using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Tools.Number
{
    public static class NumberTools
    {
        public static IList<int> CountUp(int start, int end, int stepSize = 10)
        {
            var countList = new List<int>();

            for(int i = start; i <= end; i += stepSize)
            {
                countList.Add(i);
            }

            return countList;
        }

        public static IList<int> CountFrom(this int end, int start, int stepSize = 10)
        {
            return CountUp(start, end, stepSize);
        }
    }
}
