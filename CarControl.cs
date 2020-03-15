using Dagucar.Enums;
using System;

namespace Dagucar
{
    public class CarControl
    {
        #region Direction
        public eDirection Direction { get; set; }
        #endregion
        #region Sense
        public eSense Sense { get; set; }
        #endregion
        #region Speed
        private int speed;
        public int Speed
        {
            get { return speed; }
            set
            {
                if (value < 0) throw new InvalidOperationException("Speed cannot be smaller than 0");
                if (value > 15) throw new InvalidOperationException("Speed cannot be bigger than 15");
                speed = value;
            }
        }
        #endregion

        private byte ComputeDirection(eDirection direction, eSense sense, int speed)
        {
            int ms_nibble;
            if (sense == eSense.Forward)
            {
                if (direction == eDirection.Left) ms_nibble = 5;
                else if (direction == eDirection.Right) ms_nibble = 6;
                else ms_nibble = 1;
            }
            else if (sense == eSense.Backward)
            {
                if (direction == eDirection.Left) ms_nibble = 7;
                else if (direction == eDirection.Right) ms_nibble = 8;
                else ms_nibble = 2;
            }
            else
            {
                if (direction == eDirection.Left) ms_nibble = 3;
                else if (direction == eDirection.Right) ms_nibble = 4;
                else ms_nibble = 0;
            }

            if (speed < 0) throw new Exception("Speed cannot be smaller than 0");
            if (speed > 15) throw new Exception("Speed cannot be larger than 15");

            var val = (ms_nibble << 4) + (speed);
            Console.WriteLine($"Byte: {val}, Direction: {Enum.GetName(typeof(eDirection), direction)}, Sense: {Enum.GetName(typeof(eSense), sense)}, Direction: {ms_nibble}, Speed: {speed}");

            return Convert.ToByte(val);
        }

        public byte DataToSend
        {
            get
            {
                return ComputeDirection(Direction, Sense, speed);
            }
        }
    }
}