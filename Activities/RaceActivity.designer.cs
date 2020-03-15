using Android.OS;
using Android.Widget;
using Android.Hardware;

namespace Dagucar.Activities
{
    public partial class RaceActivity
    {
        private SeekBar sbrDirection;
        private SeekBar sbrSpeed;
        private CheckBox chkAccellero;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_race);

            sbrDirection = FindViewById<SeekBar>(Resource.Id.sbrDirection);
            sbrSpeed = FindViewById<SeekBar>(Resource.Id.sbrSpeed);
            chkAccellero = FindViewById<CheckBox>(Resource.Id.chkAccellero);
            sensorManager = (SensorManager)GetSystemService(SensorService);
            carControl = new CarControl();

            sbrDirection.ProgressChanged += SbrDirection_ProgressChanged;
            sbrSpeed.ProgressChanged += SbrSpeed_ProgressChanged;
            chkAccellero.CheckedChange += ChkAccellero_CheckedChange;
        }
    }
}