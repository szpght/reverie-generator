using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Tasks;
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
            //Console.WriteLine(add.Generate(ctx));
            output.Size = VariableSize.Word;
            a.Sign = true;
            a.Size = VariableSize.Byte;
            var sub = new Sub(a, b, output);
            //Console.WriteLine(sub.Generate(ctx));


            var p1 = new Greater(a, b);
            var p2 = new Greater(b, output);
            var p3 = new Greater(a, output);
            var p4 = new Relation(p2, p3, RelationType.And);
            var p5 = new Relation(p1, p4, RelationType.Or);
            var P = p5;
            var @if = new If();
            @if.Predicate = P;
            @if.Code = new CodeBlock(new List<ICode>() { add }, null);
            @if.Else = new CodeBlock(new List<ICode>() { sub }, null);
            Console.Out.WriteLine(@if.Generate(ctx));


            Console.Read();
        }
    }
}