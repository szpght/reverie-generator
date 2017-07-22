﻿using Newtonsoft.Json;
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
            var cc = new SysVABICallingConvention();
            var ctx = new Context(cc);
            var add = BasicBinaryOp.Add(a, b, output);
            var sub = BasicBinaryOp.Subtract(a, b, output);

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
            var call = new FunctionCall(new Label("dupa", LabelType.Global), arguments);
            call.Result = output;
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