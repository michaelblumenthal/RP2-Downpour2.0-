using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blumenthalit.SocialUproar.TwitterQueryer
{
    public static class UproarTweets
    {
        /// <summary>
        /// Executes a baseline Twitter query and returns the SinceId.
        /// </summary>
        /// <returns>The SinceId, a number but really a string.</returns>
        public static string getBaseline()
        {
            return "00000"; //TODO return SinceID
        }

        /// <summary>
        /// Pass in the SinceId, and a variable for the Red Count and a variable for the Blue Count, and this function
        /// will set the red count and blue count to the correct values.
        /// </summary>
        /// <param name="sinceId"></param>
        /// <param name="redCount"></param>
        /// <param name="blueCount"></param>
        public static void GetRedAndBlueDeltas(string sinceId, ref int redCount, ref int blueCount)
        {
            //TODO Query Twitter and count
            redCount =  (new Random()).Next(10);
            blueCount = (new Random()).Next(10);
        }
    }
}
