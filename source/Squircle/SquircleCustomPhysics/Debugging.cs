using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    public enum DebugLevelType
    {
        None,
        Normal,
        Verbose
    }

    public class DebugLevel
    {
        public DebugLevelType Value { get; set; }

        public bool IsNone { get { return Value == DebugLevelType.None; } }
        public bool IsNormal { get { return Value == DebugLevelType.Normal; } }
        public bool IsVerbose { get { return Value == DebugLevelType.Verbose; } }

        public void SetNone() { Value = DebugLevelType.None; }
        public void SetNormal() { Value = DebugLevelType.Normal; }
        public void SetVerbose() { Value = DebugLevelType.Verbose; }

        public void CycleForward()
        {
            var max = Enum.GetNames(typeof(DebugLevelType)).Length;

            var val = (int)Value + 1;
            if (val >= max) { val = 0; }
            Value = (DebugLevelType)val;
        }

        public void CycleBackward()
        {
            var max = Enum.GetNames(typeof(DebugLevelType)).Length;

            var val = (int)Value - 1;
            if (val < 0) { val = max - 1; }
            Value = (DebugLevelType)val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
