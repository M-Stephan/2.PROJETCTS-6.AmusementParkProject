using Spectre.Console;
using System;

namespace Park
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Menu menu = new();
            menu.DisplayMenu();
        }
    }
}