using Android.OS;
using Android.Widget;
using Android.Hardware;
using Android.App;
using static Android.OS.PowerManager;

namespace Dagucar.Activities
{
    public partial class RaceActivity
    {
        private SeekBar sbrDirection;
        private SeekBar sbrSpeed;
        private CheckBox chkAccellero;
        private TextView lblSpeed;
        private TextView lblDirection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_race);

            sbrDirection = FindViewById<SeekBar>(Resource.Id.sbrDirection);
            sbrSpeed = FindViewById<SeekBar>(Resource.Id.sbrSpeed);
            chkAccellero = FindViewById<CheckBox>(Resource.Id.chkAccellero);
            lblSpeed = FindViewById<TextView>(Resource.Id.lblSpeed);
            lblDirection = FindViewById<TextView>(Resource.Id.lblDirection);
            sensorManager = (SensorManager)GetSystemService(SensorService);
            carControl = new CarControl();

            sbrDirection.ProgressChanged += SbrDirection_ProgressChanged;
            sbrSpeed.ProgressChanged += SbrSpeed_ProgressChanged;
            chkAccellero.CheckedChange += ChkAccellero_CheckedChange;

            powerManager = (PowerManager)GetSystemService(PowerService);
            wakeLock = powerManager?.NewWakeLock(WakeLockFlags.ScreenBright, "Dagucar::KeepAwakeTag");

            wakeLock?.Acquire();
        }

        PowerManager powerManager;
        WakeLock wakeLock;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            wakeLock?.Release();
        }
    }
}