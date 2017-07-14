using Newtonsoft.Json;
using System;
using System.IO;
using Reverie.CodeGeneration;

namespace Reverie
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("ast.json");
            dynamic ast = JsonConvert.DeserializeObject(code);

            var a = new Variable("rsp", 8, VariableSize.Dword);
            var b = new Variable("rsp", 16, VariableSize.Qword);
            var output = new Variable("rsp", 24, VariableSize.Qword);
            var alloc = new RegisterAllocator();
            var add = new Add(alloc, a, b, output);
            Console.WriteLine(add.Generate());

            Console.Read();
        }
    }
}