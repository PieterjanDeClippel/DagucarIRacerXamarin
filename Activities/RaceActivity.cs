using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Dagucar.Enums;

namespace Dagucar.Activities
{
    [Activity(Label = "RaceActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class RaceActivity : Activity, ISensorEventListener
    {
        public static BluetoothSocket Socket;
        private SensorManager sensorManager;
        private bool useAccellero = true;

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
        private async Task UpdateAsync()
        {
            var newByte = ComputeDirection(Direction, Sense, speed);
            if (sentByte == newByte) return;
            sentByte = newByte;
            await Socket.OutputStream.WriteAsync(new[] { newByte });
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
            sensorManager = (SensorManager)GetSystemService(SensorService);

            btnVooruit.Click += BtnVooruit_Click;
            btnAchteruit.Click += BtnAchteruit_Click;
            btnLinks.Click += BtnLinks_Click;
            btnRechts.Click += BtnRechts_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();
			sensorManager.RegisterListener(this, sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
        }

        protected override void OnPause()
        {
            base.OnPause();
			sensorManager.UnregisterListener(this);
        }

        public override void OnBackPressed()
        {
            sensorManager.UnregisterListener(this);
            Socket.Close();
            Socket.Dispose();
            base.OnBackPressed();
        }

        #region Manual control
        private async void BtnRechts_Click(object sender, EventArgs e)
        {
            if (!useAccellero)
            {
                Direction = eDirection.Right;
                await UpdateAsync();
            }
        }

        private async void BtnLinks_Click(object sender, EventArgs e)
        {
            if (!useAccellero)
            {
                Direction = eDirection.Left;
                await UpdateAsync();
            }
        }

        private async void BtnAchteruit_Click(object sender, EventArgs e)
        {
            if (!useAccellero)
            {
                Speed = 15;
                Sense = eSense.Backward;
                await UpdateAsync();
            }
        }

        private async void BtnVooruit_Click(object sender, EventArgs e)
        {
            if (!useAccellero)
            {
                Speed = 15;
                Sense = eSense.Forward;
                await UpdateAsync();
            }
        }
        #endregion
        #region Accellero control
        static readonly object syncLock = new object();
        void ISensorEventListener.OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
        }

        void ISensorEventListener.OnSensorChanged(SensorEvent e)
        {
            if (useAccellero)
            {
                lock (syncLock)
                {
                    byte send = ConvertDirection(e.Values);
                    if (send != sentByte)
                        Socket.OutputStream.Write(new byte[] { sentByte = send }, 0, 1);
                }
            }
        }

        public byte ConvertDirection(IList<float> accel)
        {
            // http://cdn.sparkfun.com/datasheets/Robotics/DaguCarCommands.pdf
            //
            // Y	->		-8:+8		=		vooruit:achteruit
            // X	->		>3:<-3		=		links:rechts

            float x = accel[0];
            float y = accel[1];

            // speed =   0 -> 15
            int speed = System.Math.Min(Convert.ToInt32(System.Math.Abs(y) * 2), 15);
            // forw =	true:vooruit	false:achteruit
            bool forw = y <= 0;
            // dir =	0:links	-1:rechtdoor	1:rechts
            int dir = x >= 3 ? 0 : x <= -3 ? 1 : -1;

            int MSB_nibble = 0;
            if (dir == -1)
            {
                MSB_nibble = forw ? 1 : 2;
            }
            else
            {
                MSB_nibble = forw ? 5 : 7;
                MSB_nibble += dir;
            }

            return Convert.ToByte((MSB_nibble << 4) + speed);
        }
        #endregion
    }
}