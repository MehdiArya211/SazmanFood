using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastracture.Chart
{
    public static class ColorGenerator
    {
        public static string GetColor()
        {
            return getRandColor();
        }

        public static List<string> GetColorList(int count)
        {
            var colors = new List<string>(count);
            for (int i = 0; i < count; i++)
                colors.Add(getRandColor());

            return colors;
        }

        private static string getRandColor()
        {
            var rnd = new Random();
            string hexOutput = string.Format("{0:X}", rnd.Next(0, 0xFFFFFF));
            while (hexOutput.Length < 6)
                hexOutput = "0" + hexOutput;
            return "#" + hexOutput;
        }
    }
}
