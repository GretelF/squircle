using Microsoft.Xna.Framework;

namespace Squircle
{
    public struct RingIndex
    {
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                if      (value > UpperBound) _value = LowerBound;
                else if (value < LowerBound) _value = UpperBound;
                else                         _value = value;
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
    }
}
