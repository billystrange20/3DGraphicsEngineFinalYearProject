using System;
using System.Collections.Generic;
using System.Text;

namespace _3DEngine
{
    class Logger 
    {
        public static void Info(string message) => Log(ConsoleColor.White, "Info", message);
        public static void Debug(string message) => Log(ConsoleColor.Gray, "Debug", message);
        public static void Error(string message) => Log(ConsoleColor.Red, "Error", message);

        public static void Execption(Exception ex)
            => Log(ConsoleColor.Yellow, "Exception", 
                $"An unhandled exception has been thrown: {ex.Message}\n\n{ex.StackTrace}\n");

        private static void Log(ConsoleColor color, string level, string message)
        {
            Console.ForegroundColor = color;
            Console.Write($"[{level}]: ");
            Console.ResetColor();
            Console.WriteLine(message);
        }
    }
}
