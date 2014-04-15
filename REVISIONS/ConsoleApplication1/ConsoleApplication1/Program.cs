using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {   
            //offset (sample size)
            var offSet = 2;
            //x to latitude, y to longitude
            var targX = 3; var targY = 3;
            var minX = targX - offSet; var minY = targY - offSet;
            var maxX = targX + offSet; var maxY = targY + offSet;
            string minSearch = (minX - 1).ToString();
            string maxSearch = (maxX + 1).ToString();

            string[] data = System.IO.File.ReadLines("C:\\Users\\Matt\\Documents\\Visual Studio 2012\\Projects\\ConsoleApplication1\\data.txt")
            .SkipWhile(line => !line.Contains(minSearch))
            //.Skip(1)
            .TakeWhile(line => !line.Contains(minX.ToString()))
            .SkipWhile(line => !line.Contains(maxSearch));
        }
    }
}
