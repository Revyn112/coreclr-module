﻿using System;

namespace AltV.Net.Executable.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            new ResourceBuilder(args, new MyResource()).Start();
        }
    }
}