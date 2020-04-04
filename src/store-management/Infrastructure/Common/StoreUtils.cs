using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Infrastructure.Common
{
    public static class StoreUtils
    {
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static string GenerateRandomSequence()
        {
            Random rnd = new Random();
            int myRandomNo= rnd.Next(10000000, 99999999);
            return myRandomNo.ToString();
        }
    }

}
