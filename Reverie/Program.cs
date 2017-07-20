using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Reverie.CodeGeneration;

namespace Reverie
{
    class Program
    {
        static void Main(string[] args)
        {
            //var code = File.ReadAllText("ast.json");
            //dynamic ast = JsonConvert.DeserializeObject(code);
            var a = new Variable("rsp", 0, VariableSize.Qword);
            var b = new Variable("rsp", 8, VariableSize.Qword);
            var output = new Variable("rsp", 16, VariableSize.Qword);
            var ctx = new Context();
            var add = BasicBinaryOp.Add(a, b, output);
            var sub = BasicBinaryOp.Subtract(a, b, output);


            var p1 = new Greater(a, b);
            var p2 = new Greater(b, output);
            var p3 = new Greater(a, output);
            var p4 = new Relation(p2, p3, RelationType.And);
            var p5 = new Relation(p1, p4, RelationType.And);
            var P = p5;
            var trueBlock = new CodeBlock(new List<ICode>() { add, sub }, null);
            var falseBlock = new CodeBlock(new List<ICode>() { sub, add }, null);
            var @if = new If(P, trueBlock, falseBlock);
            Console.WriteLine(@if.Generate(ctx));
            Console.WriteLine("-------------------------");
            Console.WriteLine(add.Generate(ctx));


            //Console.Read();
        }
    }
}