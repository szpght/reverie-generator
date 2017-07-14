using Newtonsoft.Json;
using System;
using System.IO;

namespace Reverie
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("ast.json");
            dynamic ast = JsonConvert.DeserializeObject(code);



            Console.WriteLine("Hello World!");
            Console.Read();
        }
    }
}