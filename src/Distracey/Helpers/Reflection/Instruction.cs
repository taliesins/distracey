using System.Reflection.Emit;
using System.Text;

namespace Distracey.Helpers.Reflection
{
    public sealed class Instruction
    {
        readonly int _offset;
        OpCode _opcode;
        object _operand;

        Instruction _previous;
        Instruction _next;

        public int Offset
        {
            get { return _offset; }
        }

        public OpCode OpCode
        {
            get { return _opcode; }
        }

        public object Operand
        {
            get { return _operand; }
            internal set { _operand = value; }
        }

        public Instruction Previous
        {
            get { return _previous; }
            internal set { _previous = value; }
        }

        public Instruction Next
        {
            get { return _next; }
            internal set { _next = value; }
        }

        public int Size
        {
            get
            {
                int size = _opcode.Size;

                switch (_opcode.OperandType)
                {
                    case OperandType.InlineSwitch:
                        size += (1 + ((int[])_operand).Length) * 4;
                        break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        size += 8;
                        break;
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:
                        size += 4;
                        break;
                    case OperandType.InlineVar:
                        size += 2;
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                        size += 1;
                        break;
                }

                return size;
            }
        }

        internal Instruction(int offset, OpCode opcode)
        {
            _offset = offset;
            _opcode = opcode;
        }

        public override string ToString()
        {
            var instruction = new StringBuilder();

            AppendLabel(instruction, this);
            instruction.Append(':');
            instruction.Append(' ');
            instruction.Append(_opcode.Name);

            if (_operand == null)
                return instruction.ToString();

            instruction.Append(' ');

            switch (_opcode.OperandType)
            {
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                    AppendLabel(instruction, (Instruction)_operand);
                    break;
                case OperandType.InlineSwitch:
                    var labels = (Instruction[])_operand;
                    for (var i = 0; i < labels.Length; i++)
                    {
                        if (i > 0)
                            instruction.Append(',');

                        AppendLabel(instruction, labels[i]);
                    }
                    break;
                case OperandType.InlineString:
                    instruction.Append('\"');
                    instruction.Append(_operand);
                    instruction.Append('\"');
                    break;
                default:
                    instruction.Append(_operand);
                    break;
            }

            return instruction.ToString();
        }

        static void AppendLabel(StringBuilder builder, Instruction instruction)
        {
            builder.Append("IL_");
            builder.Append(instruction._offset.ToString("x4"));
        }
    }
}