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
var hello = new CString("My #%d program!\n");
var one = new ConstantInteger(1);
var printf = new FunctionCall(new Label("printf"));
printf.Arguments.Add(hello);
printf.Arguments.Add(one);
var main = new Function("main");
main.Code.Add(printf);
var callingConvention = new SysVAbiCallingConvention();
var ctx = new Context(callingConvention);
var asm = new Assembly();
main.Generate(asm, ctx);
Console.WriteLine(asm);
        }
    }
}