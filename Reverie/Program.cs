using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            //Console.WriteLine(add.Generate(ctx));
            output.Size = VariableSize.Word;
            a.Sign = true;
            a.Size = VariableSize.Byte;
            var sub = new Sub(a, b, output);
            //Console.WriteLine(sub.Generate(ctx));


            var r1 = new Relation() {Negated = false, Type = RelationType.And};
            var r2 = new Relation() {Negated = false, Type = RelationType.And, Left = new Lol(), Right = new Lol()};
            var r3 = new Relation() {Negated = false, Type = RelationType.And, Left = new Lol(), Right = new Lol()};
            r1.Left = r2;
            r1.Right = r3;

            var p1 = new Greater(a, b);
            var @if = new If();
            @if.Predicate = p1;
            @if.Code = new CodeBlock(new List<ICode>() { add }, null);
            @if.Else = new CodeBlock(new List<ICode>() { sub }, null);
            Console.Out.WriteLine(@if.Generate(ctx));


            Console.Read();
        }
    }
}