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
            Console.WriteLine(File.ReadAllText("CodeHeader.asm"));
            var hello = new CString("Hello world!");
            var putsLabel = new Label("puts");
            var puts = new FunctionCall(putsLabel);
            puts.Arguments.Add(hello);
            var function = new Function("main");
            function.Code.Add(puts);
            var var1 = new StackVariable(VariableSize.Qword);
            var var2 = new StackVariable(VariableSize.Qword);
            var var3 = new StackVariable(VariableSize.Qword);
            var var4 = new StackVariable(VariableSize.Qword);
            var var5 = new StackVariable(VariableSize.Qword);
            var var6 = new StackVariable(VariableSize.Qword);
            var var7 = new StackVariable(VariableSize.Qword);
            var var8 = new StackVariable(VariableSize.Qword);
            function.Arguments.Add(var1);
            function.Arguments.Add(var2);
            function.Arguments.Add(var3);
            function.Arguments.Add(var4);
            function.Arguments.Add(var5);
            function.Arguments.Add(var6);
            function.Arguments.Add(var7);
            function.Arguments.Add(var8);
            var ad = BasicBinaryOp.Add(var7, var8, var7);
            function.Code.Add(ad);
            var cc = new SysVAbiCallingConvention();
            var ctx = new Context(cc);
            GenerateAndPrint(function, ctx);

            return;
            //var code = File.ReadAllText("ast.json");
            //dynamic ast = JsonConvert.DeserializeObject(code);
            //var a = new StackVariable("rsp", 0, VariableSize.Qword);
            //var b = new StackVariable("rsp", 8, VariableSize.Qword);
            var a = new ConstantInteger(666);
            var b = new CString("lelxd");
            var output = new StackVariable(VariableSize.Qword);
            var modulo = new StackVariable(VariableSize.Qword);
            var add = BasicBinaryOp.Add(a, b, output);
            var sub = BasicBinaryOp.Subtract(a, b, output);
            var mult = new Division(a, b, output, modulo);

            GenerateAndPrint(mult, ctx);

            var arguments = new List<Variable>
            {
                a,
                a,
                a,
                a,
                a,
                a,
                a,
                b,
                output
            };
            var call = new FunctionCall(new Label("dupa"), output, arguments);
            GenerateAndPrint(call, ctx);


            var p1 = new Greater(a, b);
            var p2 = new Greater(b, output);
            var p3 = new Greater(a, output);
            var p4 = new Relation(p2, p3, RelationType.And);
            var p5 = new Relation(p1, p4, RelationType.And);
            var P = p5;
            var trueBlock = new CodeBlock(new List<ICode>() { add, sub }, null);
            var falseBlock = new CodeBlock(new List<ICode>() { sub, add }, null);
            var @if = new If(P, trueBlock, falseBlock);
            GenerateAndPrint(@if, ctx);
            Console.WriteLine("-------------------------");

            var @while = new While(P, trueBlock);
            GenerateAndPrint(@while, ctx);


            //Console.Read();
        }

        static void GenerateAndPrint(ICode code, Context ctx)
        {
            var asm = new Assembly();
            code.Generate(asm, ctx);
            Console.WriteLine(asm);
        }
    }
}