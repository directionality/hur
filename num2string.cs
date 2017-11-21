using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace tester
{
    public class Program
    {
	//this prigram converts any list of numbers (int array) to a string. these strings can be used in various algorithms
	//to give the distance from a particular metric. so, the calculated array of star calculus decriptors can be transformed 
	//into a string array by the function here
        
        public static String num2string(int[] nums) {
            String x = "";
            foreach (int n in nums) {
                char c = Convert.ToChar(n + 72);
                x+=c;
            }
            return x;
        }
        
        public static void Main(string[] args)
        {
            //Your code goes here
            Console.WriteLine("Hello, world!");
            
            int[] numbers = { 4, 5, 6, 1, 2, 3, -2, -1, 0 };
            
            String a = num2string(numbers);
            Console.WriteLine(a);
            
        }
    }
}