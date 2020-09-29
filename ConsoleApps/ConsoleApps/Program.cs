using System;

namespace ConsoleApps
{
    class Program
    {
        public static int GetNthFibonacci_Rec(int n)
        {
            if ((n == 0) || (n == 1))
            {
                return n;
            }
            else
            {
                return GetNthFibonacci_Rec(n - 1) + GetNthFibonacci_Rec(n - 2);
            }
        }
        static void Main(string[] args)
        {
          var a=  GetNthFibonacci_Rec(10);
            Console.WriteLine(a);
            Console.ReadLine();
        }
    }
}
