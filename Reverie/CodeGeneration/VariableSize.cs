using System;

namespace Reverie.CodeGeneration
{
    public enum VariableSize
    {
        Byte = 1,
        Word = 2,
        Dword = 4,
        Qword = 8
    }

    public static class VariableSizeExtensions
    {
        public static string Asm(this VariableSize size)
        {
            switch (size)
            {
                case VariableSize.Byte:
                    return "BYTE";
                case VariableSize.Dword:
                    return "DWORD";
                case VariableSize.Qword:
                    return "QWORD";
                case VariableSize.Word:
                    return "WORD";
            }
            throw new Exception();
        }

        public static string RegisterSuffix(this VariableSize size)
        {
            switch (size)
            {
                case VariableSize.Byte:
                    return "b";
                case VariableSize.Word:
                    return "w";
                case VariableSize.Dword:
                    return "d";
                default:
                    return "";
            }
        }
    }

}