using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AssemblyToDOT.OperandHelper;

namespace AssemblyToDOT
{
    public class DOTBuilder
    {
        private StringBuilder sb;
        private long dataNodeId = 0;
        private long insnNodeId = 0;
        private long branchNodeId = 0;
        private Dictionary<string, string> branchesToNodes;
        public DOTBuilder()
        {
            sb = new StringBuilder();
            branchesToNodes = new Dictionary<string, string>();
        }

        public void AddFunction(Function function)
        {
            CollectBranches(function);
            CollectNodes(function);
            for (int i = 0; i < function.Instructions.Count; i++)
            {
                AddInstruction(function.Instructions[i]);
            }
        }
        public void AddInstruction(Instruction instruction)
        {
            if (instruction.Opcode == "mov")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsRegister(instruction.Operands[1]))
                {
                    //mov from register to register
                    MakeMovRegReg(instruction);
                }
                else if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsAddress(instruction.Operands[1]))
                {
                    //mov from memory to register
                    MakeMovRegMem(instruction);
                }
                else if (OperandHelper.IsAddress(instruction.Operands[0])
                    && OperandHelper.IsRegister(instruction.Operands[1]))
                {
                    //mov from register to memory
                    MakeMovMemReg(instruction);
                }
                else if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsConstant(instruction.Operands[1]))
                {
                    //mov constant to register
                    MakeMovRegConst(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "cmovb")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsRegister(instruction.Operands[1]))
                {
                    //cmovb from register to register
                    MakeCmovbRegReg(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "cmp")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsRegister(instruction.Operands[1]))
                {
                    //cmp register with register
                    MakeCmpRegReg(instruction);
                }
                else if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsConstant(instruction.Operands[1]))
                {
                    //cmp register with constant
                    MakeCmpRegConst(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "ja")
            {
                //ja to address
                MakeJaAddr(instruction);
            }
            else if (instruction.Opcode == "jb")
            {
                //jb to address
                MakeJbAddr(instruction);
            }
            else if (instruction.Opcode == "jbe")
            {
                //jbe to address
                MakeJbeAddr(instruction);
            }
            else if (instruction.Opcode == "jmp")
            {
                //jmp to address
                MakeJmpAddr(instruction);
            }
            else if (instruction.Opcode == "lea")
            {
                //lea
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsAddress(instruction.Operands[1]))
                {
                    //lea from address to register
                    MakeLeaRegAddr(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "call")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0]))
                {
                    //MakeCallReg(instruction);
                    MakeUnknownInstruction(instruction);
                }
                else
                {
                    //call name
                    MakeCallName(instruction);
                }
            }
            else if (instruction.Opcode == "add")
            {
                //add
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsRegister(instruction.Operands[1]))
                {
                    //add register to register
                    //MakeAddRegReg(instruction);
                    MakeUnknownInstruction(instruction);
                }
                else if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsConstant(instruction.Operands[1]))
                {
                    //add constant to register
                    MakeAddRegConst(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "xor")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsRegister(instruction.Operands[1]))
                {
                    //xor register to register
                    MakeXorRegReg(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "or")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsRegister(instruction.Operands[1]))
                {
                    //or register with register
                    //MakeOrRegReg(instruction);
                    MakeUnknownInstruction(instruction);
                }
                else if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsConstant(instruction.Operands[1]))
                {
                    //or register with constant
                    MakeOrRegConst(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "push")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0]))
                {
                    //push register
                    MakePushReg(instruction);
                }
                else if (OperandHelper.IsAddress(instruction.Operands[0]))
                {
                    //push memory value
                    MakePushMem(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "pop")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0]))
                {
                    //pop register
                    MakePopReg(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "retn")
            {
                if (OperandHelper.IsConstant(instruction.Operands[0]))
                {
                    //retn constant
                    MakeRetnConst(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "shr")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsConstant(instruction.Operands[1]))
                {
                    //shr register by constant
                    MakeShrRegConst(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else if (instruction.Opcode == "sub")
            {
                if (OperandHelper.IsRegister(instruction.Operands[0])
                    && OperandHelper.IsRegister(instruction.Operands[1]))
                {
                    //sub register from register
                    MakeSubRegReg(instruction);
                }
                else
                {
                    MakeUnknownInstruction(instruction);
                }
            }
            else
            {
                MakeUnknownInstruction(instruction);
            }
        }
        private void CollectBranches(Function function)
        {
            for (int i = 0; i < function.Instructions.Count; i++)
            {
                Instruction instruction = function.Instructions[i];
                string location;

                if (OpcodeHelper.IsJump(instruction.Opcode))
                {
                    location = instruction.Operands[0].Replace("loc_", "00");
                    if (!branchesToNodes.ContainsKey(location))
                    {
                        branchesToNodes.Add(location, "");
                    }
                    
                }
            }
        }
        private void CollectNodes(Function function)
        {
            foreach (KeyValuePair<string, string> item in branchesToNodes)
            {
                string location = item.Key;
                for (int i = 0; i < function.Instructions.Count; i++)
                {
                    Instruction instruction = function.Instructions[i];
                    if (instruction.Address == location)
                    {
                        branchesToNodes[location] = "br" + branchNodeId;
                        branchNodeId++;
                    }
                }
            }
        }
        
        private string MakeReg(string reg)
        {
            string node = string.Format("dn{0}", dataNodeId++);
            sb.AppendLine(string.Format("  {0} [label=\"{1}\"];", node, reg));
            return node;
        }
        private string MakeMem(string mem, OperandSize size)
        {
            string node = string.Format("dn{0}", dataNodeId++);
            string operandSize = "";
            switch (size)
            {
                case OperandSize.Byte:
                    operandSize = "memory.byte";
                    break;
                case OperandSize.Word:
                    operandSize = "memory.word";
                    break;
                case OperandSize.Dword:
                    operandSize = "memory.dword";
                    break;
                case OperandSize.Qword:
                    operandSize = "memory.qword";
                    break;
                case OperandSize.Dqword:
                    operandSize = "memory.dqword";
                    break;
                case OperandSize.Unknown:
                    operandSize = "memory.unknown";
                    break;
                default:
                    operandSize = "memory.undefined";
                    break;
            }
            sb.AppendLine(string.Format("  {0} [label=\"{1}\", color=orange, shape=box];", node, operandSize));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", mem, node));
            return node;
        }
        private string MakeAddr(string addr)
        {
            string node = string.Format("dn{0}", dataNodeId++);
            sb.AppendLine(string.Format("  {0} [label=\"{1}\"];", node, addr));
            return node;
        }
        private string MakeName(string name)
        {
            string node = string.Format("dn{0}", dataNodeId++);
            sb.AppendLine(string.Format("  {0} [label=\"{1}\"];", node, name));
            return node;
        }
        private string MakeConst(string constant)
        {
            string node = string.Format("dn{0}", dataNodeId++);
            sb.AppendLine(string.Format("  {0} [label=\"{1}\"];", node, constant));
            return node;
        }
        private string MakeFlags(string name)
        {
            string node = string.Format("dn{0}", dataNodeId++);
            sb.AppendLine(string.Format("  {0} [label=\"{1}\"];", node, name));
            return node;
        }
        private string MakeMov(Instruction instruction, string lhsNode, string rhsNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"mov\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, lhsNode));
            return node;
        }
        private string MakeMov(Instruction instruction, string lhsNode, string rhsNode, string nodeSuffix)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            node += nodeSuffix;
            sb.AppendLine(string.Format("  {0} [label=\"mov\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, lhsNode));
            return node;
        }
        private string MakeCmovb(Instruction instruction, string lhsNode, string rhsNode, string flags, string resultNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"cmovb\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", flags, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeCmp(Instruction instruction, string lhsNode, string rhsNode, string flagsNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"cmp\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan, xlabel=\"1\"]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan, xlabel=\"2\"]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, flagsNode));
            return node;
        }
        private string MakeJa(Instruction instruction, string flagsNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"ja\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", flagsNode, node));
            return node;
        }
        private string MakeJb(Instruction instruction, string flagsNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"jb\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", flagsNode, node));
            return node;
        }
        private string MakeJbe(Instruction instruction, string flagsNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"jbe\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", flagsNode, node));
            return node;
        }
        private string MakeJmp(Instruction instruction)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"jmp\", color=purple, shape=box];", node));
            return node;
        }
        private string MakeLea(Instruction instruction, string lhsNode, string rhsNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"lea\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, lhsNode));
            return node;
        }
        private string MakeCall(Instruction instruction, string calleeNode, string resultNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"call\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", calleeNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data (if any!)", node + "args", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeAdd(Instruction instruction, string lhsNode, string rhsNode, string resultNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"add\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeAdd(Instruction instruction, string lhsNode, string rhsNode, string resultNode, string nodeSuffix)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            node += nodeSuffix;
            sb.AppendLine(string.Format("  {0} [label=\"add\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeSub(Instruction instruction, string lhsNode, string rhsNode, string resultNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"sub\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeSub(Instruction instruction, string lhsNode, string rhsNode, string resultNode, string nodeSuffix)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            node += nodeSuffix;
            sb.AppendLine(string.Format("  {0} [label=\"sub\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeXor(Instruction instruction, string lhsNode, string rhsNode, string resultNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"xor\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeOr(Instruction instruction, string lhsNode, string rhsNode, string resultNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"or\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeShr(Instruction instruction, string lhsNode, string rhsNode, string resultNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"shr\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan, xlabel=\"1\"]; //data", lhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan, xlabel=\"2\"]; //data", rhsNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", node, resultNode));
            return node;
        }
        private string MakeRetn(Instruction instruction, string constNode, string retValNode)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"retn\", color=purple, shape=box];", node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", constNode, node));
            sb.AppendLine(string.Format("  {0} -> {1} [color=cyan]; //data", retValNode, node));
            return node;
        }
        private string MakeUnknown(Instruction instruction)
        {
            string node;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                node = branchesToNodes[instruction.Address];
            }
            else
            {
                node = string.Format("n{0}", insnNodeId++);
            }
            sb.AppendLine(string.Format("  {0} [label=\"Unknown({1} @ {2})\", color=red, shape=box];", 
                node, instruction.Opcode, instruction.Address));
            return node;
        }
        private void MakeFlow(string currentNode)
        {
            string futureNode = string.Format("n{0}", insnNodeId);
            sb.AppendLine(string.Format("  {0} -> {1}; //flow", currentNode, futureNode));
        }
        private void MakeVirtualFlow(string currentNode, string firstNodeSuffix, string secondNodeSuffix)
        {
            sb.AppendLine(string.Format("  {0} -> {1}; //flow",
                currentNode + "" + firstNodeSuffix,
                currentNode + "" + secondNodeSuffix));
        }
        private void MakeFlowFalse(string currentNode, string falseNode)
        {
            sb.AppendLine(string.Format("  {0} -> {1} [color=red]; //flow", currentNode, falseNode));
        }
        private void MakeFlowTrue(string currentNode, string trueNode)
        {
            sb.AppendLine(string.Format("  {0} -> {1} [color=green]; //flow", currentNode, trueNode));
        }
        private void MakeFlowUnconditional(string currentNode, string targetNode)
        {
            sb.AppendLine(string.Format("  {0} -> {1} [color=blue]; //flow", currentNode, targetNode));
        }
        private void MakeMovRegReg(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeReg(instruction.Operands[1]);
            string insnNode = MakeMov(instruction, lhsNode, rhsNode);
            MakeFlow(insnNode);
        }
        private void MakeMovRegMem(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeAddr(instruction.Operands[1]);
            rhsNode = MakeMem(rhsNode, OperandHelper.GetOperandSize(instruction.Operands[0]));
            string insnNode = MakeMov(instruction, lhsNode, rhsNode);
            MakeFlow(insnNode);
        }
        private void MakeMovMemReg(Instruction instruction)
        {
            string lhsNode = MakeAddr(instruction.Operands[0]);
            string rhsNode = MakeReg(instruction.Operands[1]);
            lhsNode = MakeMem(lhsNode, OperandHelper.GetOperandSize(instruction.Operands[1]));
            string insnNode = MakeMov(instruction, lhsNode, rhsNode);
            MakeFlow(insnNode);

        }
        private void MakeMovRegConst(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeConst(instruction.Operands[1]);
            string insnNode = MakeMov(instruction, lhsNode, rhsNode);
            MakeFlow(insnNode);
        }
        private void MakeCmovbRegReg(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeReg(instruction.Operands[1]);
            string flagsNode = MakeFlags("eflags"); //depending on architecture
            string resultNode = MakeReg(instruction.Operands[0]);
            string insnNode = MakeCmovb(instruction, lhsNode, rhsNode, flagsNode, resultNode);
            MakeFlow(insnNode);
        }
        private void MakeCmpRegReg(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeReg(instruction.Operands[1]);
            string flagsNode = MakeFlags("eflags"); //depending on architecture
            string insnNode = MakeCmp(instruction, lhsNode, rhsNode, flagsNode);
            MakeFlow(insnNode);
        }
        private void MakeCmpRegConst(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeConst(instruction.Operands[1]);
            string flagsNode = MakeFlags("eflags"); //depending on architecture
            string insnNode = MakeCmp(instruction, lhsNode, rhsNode, flagsNode);
            MakeFlow(insnNode);
        }
        private void MakeJaAddr(Instruction instruction)
        {
            string flagsNode = MakeFlags("eflags");
            string insnNode = MakeJa(instruction, flagsNode);
            string falseNode = string.Format("n{0}", insnNodeId);
            string trueNode = branchesToNodes[instruction.Operands[0].Replace("loc_", "00")];
            MakeFlowFalse(insnNode, falseNode);
            MakeFlowTrue(insnNode, trueNode);
        }
        private void MakeJbAddr(Instruction instruction)
        {
            string flagsNode = MakeFlags("eflags");
            string insnNode = MakeJb(instruction, flagsNode);
            string falseNode = string.Format("n{0}", insnNodeId);
            string trueNode = branchesToNodes[instruction.Operands[0].Replace("loc_", "00")];
            MakeFlowFalse(insnNode, falseNode);
            MakeFlowTrue(insnNode, trueNode);
        }
        private void MakeJbeAddr(Instruction instruction)
        {
            string flagsNode = MakeFlags("eflags");
            string insnNode = MakeJbe(instruction, flagsNode);
            string falseNode = string.Format("n{0}", insnNodeId);
            string trueNode = branchesToNodes[instruction.Operands[0].Replace("loc_", "00")];
            MakeFlowFalse(insnNode, falseNode);
            MakeFlowTrue(insnNode, trueNode);
        }
        private void MakeJmpAddr(Instruction instruction)
        {
            string insnNode = MakeJmp(instruction);
            string targetNode = branchesToNodes[instruction.Operands[0].Replace("loc_", "00")];
            MakeFlowUnconditional(insnNode, targetNode);
        }
        private void MakeLeaRegAddr(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeAddr(instruction.Operands[1]);
            string insnNode = MakeLea(instruction, lhsNode, rhsNode);
            MakeFlow(insnNode);
        }
        private void MakeXorRegReg(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeReg(instruction.Operands[1]);
            string resultNode = MakeReg(instruction.Operands[0]);
            string insnNode = MakeXor(instruction, lhsNode, rhsNode, resultNode);
            MakeFlow(insnNode);
        }
        private void MakeOrRegConst(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeConst(instruction.Operands[1]);
            string resultNode = MakeReg(instruction.Operands[0]);
            string insnNode = MakeOr(instruction, lhsNode, rhsNode, resultNode);
            MakeFlow(insnNode);
        }
        private void MakeShrRegConst(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeConst(instruction.Operands[1]);
            string resultNode = MakeReg(instruction.Operands[0]);
            string insnNode = MakeShr(instruction, lhsNode, rhsNode, resultNode);
            MakeFlow(insnNode);
        }
        private void MakePushReg(Instruction instruction)
        {
            string oldEspNode = MakeReg("esp");
            string ptrSizeNode = MakeConst("4"); //Depending on pointer size
            string newEspNode = MakeReg("esp");
            string insnNode;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                insnNode = MakeSub(instruction, oldEspNode, ptrSizeNode, newEspNode, "");
            }
            else
            {
                insnNode = MakeSub(instruction, oldEspNode, ptrSizeNode, newEspNode);
            }
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                MakeVirtualFlow(insnNode, "", "_2"); //virtual
            }
            else
            {
                MakeFlow(insnNode); //real
            }
                
            string sourceNode = MakeReg(instruction.Operands[0]);
            string destinationNode = MakeMem(newEspNode, OperandHelper.GetOperandSize(instruction.Operands[0]));
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                insnNode = MakeMov(instruction, destinationNode, sourceNode, "_2");
            }
            else
            {
                insnNode = MakeMov(instruction, destinationNode, sourceNode);
            }
            MakeFlow(insnNode);
        }
        private void MakePushMem(Instruction instruction)
        {
            string oldEspNode = MakeReg("esp");
            string ptrSizeNode = MakeConst("4"); //Depending on pointer size
            string newEspNode = MakeReg("esp");
            string insnNode;
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                insnNode = MakeSub(instruction, oldEspNode, ptrSizeNode, newEspNode, "");
            }
            else
            {
                insnNode = MakeSub(instruction, oldEspNode, ptrSizeNode, newEspNode);
            }
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                MakeVirtualFlow(insnNode, "", "_2"); //virtual
            }
            else
            {
                MakeFlow(insnNode); //real
            }

            string sourceNode = MakeAddr(instruction.Operands[0]);
            sourceNode = MakeMem(sourceNode, OperandHelper.OperandSize.Dword); //depending on architecture
            string destinationNode = MakeMem(newEspNode, OperandHelper.OperandSize.Dword); //depending on architecture
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                insnNode = MakeMov(instruction, destinationNode, sourceNode, "_2");
            }
            else
            {
                insnNode = MakeMov(instruction, destinationNode, sourceNode);
            }
            MakeFlow(insnNode);
        }
        private void MakePopReg(Instruction instruction)
        {
            string oldEspNode = MakeReg("esp");
            string ptrSizeNode = MakeConst("4"); //Depending on pointer size
            string newEspNode = MakeReg("esp");

            string insnNode;
            string sourceNode = MakeMem(oldEspNode, OperandHelper.GetOperandSize(instruction.Operands[0]));
            string destinationNode = MakeReg(instruction.Operands[0]);
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                insnNode = MakeMov(instruction, destinationNode, sourceNode, "");
            }
            else
            {
                insnNode = MakeMov(instruction, destinationNode, sourceNode);
            }

            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                MakeVirtualFlow(insnNode, "", "_2"); //virtual
            }
            else
            {
                MakeFlow(insnNode); //real
            }
            
            if (branchesToNodes.ContainsKey(instruction.Address))
            {
                insnNode = MakeAdd(instruction, oldEspNode, ptrSizeNode, newEspNode, "_2");
            }
            else
            {
                insnNode = MakeAdd(instruction, oldEspNode, ptrSizeNode, newEspNode);
            }
            MakeFlow(insnNode);
        }
        private void MakeRetnConst(Instruction instruction)
        {
            string constNode = MakeConst(instruction.Operands[0]);
            string retValNode = MakeReg("eax"); //depending on architecture
            string insnNode = MakeRetn(instruction, constNode, retValNode);
        }
        private void MakeAddRegConst(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeConst(instruction.Operands[1]);
            string resultNode = MakeReg(instruction.Operands[0]);
            string insnNode = MakeAdd(instruction, lhsNode, rhsNode, resultNode);
            MakeFlow(insnNode);
        }
        private void MakeSubRegReg(Instruction instruction)
        {
            string lhsNode = MakeReg(instruction.Operands[0]);
            string rhsNode = MakeReg(instruction.Operands[1]);
            string resultNode = MakeReg(instruction.Operands[0]);
            string insnNode = MakeSub(instruction, lhsNode, rhsNode, resultNode);
            MakeFlow(insnNode);
        }
        private void MakeUnknownInstruction(Instruction instruction)
        {
            string insnNode = MakeUnknown(instruction);
            MakeFlow(insnNode);
        }
        private void MakeCallName(Instruction instruction)
        {
            string calleeNode = MakeName(instruction.Operands[0]);
            string resultNode = MakeReg("eax"); //depending on the architecture
            string insnNode = MakeCall(instruction, calleeNode, resultNode);
            MakeFlow(insnNode);
        }
        public string Build()
        {
            return sb.ToString();
        }
    }
}
