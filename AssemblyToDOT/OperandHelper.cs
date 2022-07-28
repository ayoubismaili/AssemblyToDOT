using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyToDOT
{
    public class OperandHelper
    {
        private static string[] byteRegisters = { "al", "ah", "bl", "bh", "cl", "ch", "dl", "dh"};
        private static string[] wordRegisters = { "ax", "bx", "cx", "dx", "sp", "bp", "si", "di"};
        private static string[] dwordRegisters = { "eax", "ebx", "ecx", "edx", "esp", "ebp", "esi", "edi"};
        public enum OperandSize
        {
            Byte,
            Word,
            Dword,
            Qword,
            Dqword,
            Unknown
        }

        public static bool IsRegister(string operand)
        {
            return (byteRegisters.Contains(operand)) 
                || (wordRegisters.Contains(operand)) 
                || (dwordRegisters.Contains(operand));
        }

        public static bool IsAddress(string operand)
        {
            return operand.StartsWith("[") && operand.EndsWith("]");
        }
        public static bool IsConstant(string operand)
        {
            bool hexParsed = false;
            try
            {
                operand = operand.Replace("h", "");
                int.Parse(operand, System.Globalization.NumberStyles.HexNumber);
                hexParsed = true;
            }
            catch (Exception)
            {
                hexParsed = false;
            }
            return hexParsed;
        }
        public static OperandSize GetOperandSize(string operand)
        {
            if (IsRegister(operand))
            {
                if (byteRegisters.Contains(operand))
                {
                    return OperandSize.Byte;
                }
                else if (wordRegisters.Contains(operand))
                {
                    return OperandSize.Word;
                }
                else if (dwordRegisters.Contains(operand))
                {
                    return OperandSize.Dword;
                }
                else
                {
                    return OperandSize.Unknown;
                }
            }
            return OperandSize.Unknown;
        }
    }
}
