using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Squircle
{
    public class RingIndex
    {
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Normalize();
            }
        }
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }

        public void Increment()
        {
            Value = _value + 1;
        }

        public void Decrement()
        {
            Value = _value - 1;
        }

        public static implicit operator int(RingIndex ring)
        {
            return ring.Value;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}:{2})", Value, LowerBound, UpperBound);
        }

        private void Normalize()
        {
            Debug.Assert(LowerBound <= UpperBound);

            while (_value > UpperBound)
            {
                var diff = _value - UpperBound;
                _value = LowerBound - 1 + diff;
            }

            while(_value < LowerBound)
            {
                var diff = LowerBound - _value;
                _value = UpperBound + 1 - diff;
            }
        }
    }
}
