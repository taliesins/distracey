﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Distracey.Common.Helpers.Reflection
{
    public static class Disassembler
    {
        public static IList<Instruction> GetInstructions(this MethodBase self)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            return MethodBodyReader.GetInstructions(self).AsReadOnly();
        }
    }
}