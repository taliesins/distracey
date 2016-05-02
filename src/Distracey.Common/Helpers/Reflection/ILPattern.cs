using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Distracey.Common.Helpers.Reflection
{
    public abstract class ILPattern
    {
        public static ILPattern Optional(OpCode opcode)
        {
            return Optional(OpCode(opcode));
        }

        public static ILPattern Optional(params OpCode[] opcodes)
        {
            return Optional(Sequence(opcodes.Select(opcode => OpCode(opcode)).ToArray()));
        }

        public static ILPattern Optional(ILPattern pattern)
        {
            return new OptionalPattern(pattern);
        }

        class OptionalPattern : ILPattern
        {
            readonly ILPattern _pattern;

            public OptionalPattern(ILPattern optional)
            {
                _pattern = optional;
            }

            public override void Match(MatchContext context)
            {
                _pattern.TryMatch(context);
            }
        }

        public static ILPattern Sequence(params ILPattern[] patterns)
        {
            return new SequencePattern(patterns);
        }

        class SequencePattern : ILPattern
        {
            readonly ILPattern[] _patterns;

            public SequencePattern(ILPattern[] patterns)
            {
                _patterns = patterns;
            }

            public override void Match(MatchContext context)
            {
                foreach (var pattern in _patterns)
                {
                    pattern.Match(context);

                    if (!context.success)
                        break;
                }
            }
        }

        public static ILPattern OpCode(OpCode opcode)
        {
            return new OpCodePattern(opcode);
        }

        class OpCodePattern : ILPattern
        {
            readonly OpCode _opcode;

            public OpCodePattern(OpCode opcode)
            {
                this._opcode = opcode;
            }

            public override void Match(MatchContext context)
            {
                if (context.instruction == null)
                {
                    context.success = false;
                    return;
                }

                context.success = context.instruction.OpCode == _opcode;
                context.Advance();
            }
        }

        public static ILPattern Either(ILPattern a, ILPattern b)
        {
            return new EitherPattern(a, b);
        }

        class EitherPattern : ILPattern
        {
            readonly ILPattern _a;
            readonly ILPattern _b;

            public EitherPattern(ILPattern a, ILPattern b)
            {
                _a = a;
                _b = b;
            }

            public override void Match(MatchContext context)
            {
                if (!_a.TryMatch(context))
                    _b.Match(context);
            }
        }

        public abstract void Match(MatchContext context);

        protected static Instruction GetLastMatchingInstruction(MatchContext context)
        {
            if (context.instruction == null)
                return null;

            return context.instruction.Previous;
        }

        public bool TryMatch(MatchContext context)
        {
            var instruction = context.instruction;
            Match(context);

            if (context.success)
                return true;

            context.Reset(instruction);
            return false;
        }

        public static MatchContext Match(MethodBase method, ILPattern pattern)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            var instructions = method.GetInstructions();
            if (instructions.Count == 0)
                throw new ArgumentException();

            var context = new MatchContext(instructions[0]);
            pattern.Match(context);
            return context;
        }
    }

    public sealed class MatchContext
    {

        internal Instruction instruction;
        internal bool success;

        Dictionary<object, object> data = new Dictionary<object, object>();

        public bool IsMatch
        {
            get { return success; }
            set { success = true; }
        }

        internal MatchContext(Instruction instruction)
        {
            Reset(instruction);
        }

        public bool TryGetData(object key, out object value)
        {
            return data.TryGetValue(key, out value);
        }

        public void AddData(object key, object value)
        {
            data.Add(key, value);
        }

        internal void Reset(Instruction instruction)
        {
            this.instruction = instruction;
            this.success = true;
        }

        internal void Advance()
        {
            this.instruction = this.instruction.Next;
        }
    }
}