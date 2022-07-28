using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyToDOT
{
    public class OpcodeHelper
    {
        private static string[] jumpOpcodes = { "ja", "jb", "jbe", "jmp" };

        public static bool IsJump(string opcode)
        {
            return jumpOpcodes.Contains(opcode);
        }
    }
}
