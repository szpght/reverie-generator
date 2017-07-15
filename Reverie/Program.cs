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
            var ctx = new Context();
            var add = new Add(a, b, output);
            Console.WriteLine(add.Generate(ctx));
            output.Size = VariableSize.Word;
            a.Sign = true;
            a.Size = VariableSize.Byte;
            Console.WriteLine(add.Generate(ctx));


            Console.Read();
        }
    }
}