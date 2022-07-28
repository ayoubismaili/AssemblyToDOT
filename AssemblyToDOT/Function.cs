using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyToDOT
{
    public class Function
    {
        public string Name;
        public List<Instruction> Instructions;
        public Function()
        {
            Instructions = new List<Instruction>();
        }
        public Function(string asmFile)
        {
            Instructions = ReadInstructions(asmFile);
        }
        private List<Instruction> ReadInstructions(string asmFile)
        {
            List<Instruction> result = new List<Instruction>();
            StreamReader sr = new StreamReader(asmFile);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(" ", 2);
                if (values.Length > 1)
                {
                    string address = values[0].Split(":")[1];
                    string disasm = values[1];
                    if (disasm.StartsWith("sub_") && Name == null)
                    {
                        Name = disasm;
                    }
                    else
                    {
                        Instruction insn = Instruction.Parse(address, disasm);
                        if (insn != null)
                        {
                            result.Add(insn);
                        }
                    }
                }
            }
            sr.Close();
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Instructions.Count; i++)
            {
                sb.AppendLine(Instructions[i].ToString());
            }
            return sb.ToString();
        }
    }
}
