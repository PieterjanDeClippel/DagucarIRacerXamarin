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
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Dagucar.Enums;

namespace Dagucar.Activities
{
    [Activity(Label = "Race", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public partial class RaceActivity : AppCompatActivity, ISensorEventListener
    {
        public static BluetoothSocket Socket;
        private SensorManager sensorManager;
        private CarControl carControl;
        private bool useAccellero;

        #region WriteToStream
        private byte sentByte;
        /// <summary>Writes the byte to the stream.</summary>
        /// <returns></returns>
        private async Task WriteToStreamAsync()
        {
            var newByte = carControl.DataToSend;
            if (sentByte == newByte) return;
            sentByte = newByte;
            await Socket.OutputStream.WriteAsync(new[] { newByte });
        }
        #endregion
        #region Lifecycle hooks
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
        #endregion

        #region Manual control

        private async void SbrSpeed_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            lblSpeed.Text = e.Progress.ToString();
            if (!useAccellero)
            {
                carControl.Speed = Math.Abs(e.Progress);
                carControl.Sense = e.Progress == 0 ? eSense.Stop
                    : e.Progress > 0 ? eSense.Forward
                    : eSense.Backward;
                await WriteToStreamAsync();
            }
        }

        private async void SbrDirection_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            lblDirection.Text = e.Progress switch
            {
                -1 => "Left",
                1 => "Right",
                _ => "Straight"
            };

            if (!useAccellero)
            {
                carControl.Direction = e.Progress switch
                {
                    -1 => eDirection.Left,
                    1 => eDirection.Right,
                    _ => eDirection.Straight
                };
                await WriteToStreamAsync();
            }
        }
        #endregion
        #region Accellero control
        static readonly object syncLock = new object();
        void ISensorEventListener.OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
        }

        async void ISensorEventListener.OnSensorChanged(SensorEvent e)
        {
            if (useAccellero)
            {
                lock (syncLock)
                {
                    UpdateCarControlForAccellero(e.Values);
                    UpdateSliders();
                }
                await WriteToStreamAsync();
            }
        }

        private void UpdateCarControlForAccellero(IList<float> accel)
        {
            // http://cdn.sparkfun.com/datasheets/Robotics/DaguCarCommands.pdf
            //
            // Y	->		-8:+8		=		vooruit:achteruit
            // X	->		>3:<-3		=		links:rechts

            float x = accel[0];
            float y = accel[1];

            // speed =   0 -> 15
            carControl.Speed = Math.Min(Convert.ToInt32(Math.Abs(y) * 2), 15);
            // sense =	true:vooruit	false:achteruit
            carControl.Sense =
                y < 0 ? eSense.Forward :
                y > 0 ? eSense.Backward :
                eSense.Stop;
            // direction =	0:links	-1:rechtdoor	1:rechts
            carControl.Direction =
                x >= 3 ? eDirection.Left :
                x <= -3 ? eDirection.Right :
                eDirection.Straight;
        }
        private void UpdateSliders()
        {
            RunOnUiThread(() =>
            {
                sbrSpeed.Progress = carControl.Sense switch
                {
                    eSense.Forward => carControl.Speed,
                    eSense.Backward => -carControl.Speed,
                    _ => 0
                };
                sbrDirection.Progress = carControl.Direction switch
                {
                    eDirection.Left => -1,
                    eDirection.Right => 1,
                    _ => 0
                };
            });
        }

        private void ChkAccellero_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            useAccellero = e.IsChecked;
        }

        #endregion
    }
}