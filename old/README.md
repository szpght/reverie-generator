# Reverie generator

This is assembly generator for yet to exist compiler. It is designed with amd64 architecture in mind.

At this moment basic arithmetic operations, function calls, ifs and while loops are more-or-less supported.
Main calling convention is SysV ABI calling convention, support for Microsoft is planned.
Generated assembly is compatible with NASM assembler.

## Example

Main() contains some simple example code.

```csharp
var hello = new CString("My #%d program!");
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
```

This is translated to:

```assembly
SECTION .text
global main
main:
push rbp
mov rbp, rsp
sub rsp, 0
.L1:
mov rdi, L0
mov rsi, 1
xor rax, rax
extern printf
call printf
add rsp, 0
.L2:
mov rsp, rbp
pop rbp
ret
SECTION .rodata
L0:
db 77
db 121
db 32
db 35
db 37
db 100
db 32
db 112
db 114
db 111
db 103
db 114
db 97
db 109
db 33
db 10
db 0
```
This example omits header (from CodeHeader.asm file), which generally should be included in every program
as it contains some definitions that can be used by the generator.

Usage:

```
$ mono Reverie.exe > code.asm
$ nasm -felf64 code.asm
$ gcc code.o -o code
$ ./code 
My #1 program!
```
