using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace lcstester
{
    public class Program
    {
        
         public static int lcs(string a, string b)
        {
            var lengths = new int[a.Length, b.Length];
            int greatestLength = 0;
            string output = "";
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    if (a[i] == b[j])
                    {
                        lengths[i, j] = i == 0 || j == 0 ? 1 : lengths[i - 1, j - 1] + 1;
                        if (lengths[i, j] > greatestLength)
                        {
                            greatestLength = lengths[i, j];
                            output = a.Substring(i - greatestLength + 1, greatestLength);
                        }
                    }
                    else
                    {
                        lengths[i, j] = 0;
                    }
                }
            }
            return output.Length;
        }
        
        public static void Main(string[] args)
        {
            //Your code goes here
            Console.WriteLine("Hello, world!");
            String a = "helloworld";
            String b = "hellofromtheotherside";
            
            Console.WriteLine(lcs(a,b));
            
        }
    }
}