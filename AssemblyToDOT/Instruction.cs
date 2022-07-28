using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AssemblyToDOT
{
    public class Instruction
    {
        public string Address;
        public string Opcode;
        public int OperandsCount;
        public string[] Operands;

        public Instruction(string address, string opcode)
        {
            Address = address;
            Opcode = opcode;
            OperandsCount = 0;
            Operands = new string[3];
        }

        public Instruction(string address, string opcode, string operand1)
        {
            Address = address;
            Opcode = opcode;
            OperandsCount = 1;
            Operands = new string[3];
            Operands[0] = operand1;
        }
        public Instruction(string address, string opcode, string operand1, string operand2)
        {
            Address = address;
            Opcode = opcode;
            OperandsCount = 2;
            Operands = new string[3];
            Operands[0] = operand1;
            Operands[1] = operand2;
        }
        public Instruction(string address, string opcode, string operand1, string operand2, string operand3)
        {
            Address = address;
            Opcode = opcode;
            OperandsCount = 3;
            Operands = new string[3];
            Operands[0] = operand1;
            Operands[1] = operand2;
            Operands[2] = operand3;
        }
        public override string ToString()
        {
            if (OperandsCount == 0)
            {
                return string.Format("{0}: {1}",
                    Address, Opcode);
            }
            else if (OperandsCount == 1)
            {
                return string.Format("{0}: {1} {2}",
                    Address, Opcode, Operands[0]);
            }
            else if (OperandsCount == 2)
            {
                return string.Format("{0}: {1} {2}, {3}",
                    Address, Opcode, Operands[0], Operands[1]);
            }
            else
            {
                return string.Format("{0}: {1} {2}, {3}, {4}",
                    Address, Opcode, Operands[0], Operands[1], Operands[2]);
            }
        }

        public static Instruction Parse(string address, string disasm)
        {
            disasm = Regex.Replace(disasm, @"\s+", " "); //Replace consecutive whitespaces by one space
            disasm = Regex.Replace(disasm, @";.*", ""); //Remove comments
            disasm = disasm.Trim();

            string[] values = disasm.Split(" ");
            if (values.Length > 0)
            {
                if (string.IsNullOrWhiteSpace(values[0])
                    || values[0] == ";"
                    || values[0].StartsWith("loc_")
                    || values[0].StartsWith("sub_"))
                {
                    return null;
                }

                if (values.Length == 1)
                {
                    return new Instruction(address, values[0]);
                }
                else if (values.Length == 2)
                {
                    return new Instruction(address, values[0], values[1]);
                }
                else if (values.Length == 3)
                {
                    if (OpcodeHelper.IsJump(values[0]) && values[1] == "short")
                    {
                        return Parse(address, disasm.Replace("short", ""));
                    }
                    else
                    {
                        return new Instruction(address, values[0], values[1].Replace(",", ""), values[2]);
                    }
                }
            }
            return null;
        }
    }
}
