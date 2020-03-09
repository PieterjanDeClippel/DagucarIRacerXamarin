using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Dagucar.Enums;

namespace Dagucar.Activities
{
    [Activity(Label = "RaceActivity")]
    public class RaceActivity : Activity
    {
        public static BluetoothSocket Socket;
        private Button btnVooruit;
        private Button btnAchteruit;
        private Button btnLinks;
        private Button btnRechts;

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
        #region Update
        private byte sentByte;
        private void Update()
        {
            var newByte = ComputeDirection(Direction, Sense, speed);
            if (sentByte == newByte) return;
            sentByte = newByte;
            Socket.OutputStream.Write(new[] { newByte });
        }
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
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_race);

            btnVooruit = FindViewById<Button>(Resource.Id.btnForward);
            btnAchteruit = FindViewById<Button>(Resource.Id.btnBackward);
            btnLinks = FindViewById<Button>(Resource.Id.btnLeft);
            btnRechts = FindViewById<Button>(Resource.Id.btnRight);

            btnVooruit.Click += BtnVooruit_Click;
            btnAchteruit.Click += BtnAchteruit_Click;
            btnLinks.Click += BtnLinks_Click;
            btnRechts.Click += BtnRechts_Click;
        }

        private void BtnRechts_Click(object sender, EventArgs e)
        {
            Direction = eDirection.Right;
            Update();
        }

        private void BtnLinks_Click(object sender, EventArgs e)
        {
            Direction = eDirection.Left;
            Update();
        }

        private void BtnAchteruit_Click(object sender, EventArgs e)
        {
            Speed = 15;
            Sense = eSense.Backward;
            Update();
        }

        private void BtnVooruit_Click(object sender, EventArgs e)
        {
            Speed = 15;
            Sense = eSense.Forward;
            Update();
        }
    }
}