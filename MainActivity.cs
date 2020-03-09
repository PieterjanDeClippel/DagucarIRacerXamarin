using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android;
using Android.Support.V4.App;
using Android.Content;
using Android.Bluetooth;
using System.Collections.Generic;

namespace Dagucar
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private BluetoothAdapter adapter;
        private List<BluetoothDevice> bonded = new List<BluetoothDevice>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            RequestBluetoothPermissions();
            InitializeBluetooth();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void RequestBluetoothPermissions()
        {
            var requiredPermissions = new[]
            {
                Manifest.Permission.Bluetooth,
                Manifest.Permission.BluetoothAdmin
            };

            ActivityCompat.RequestPermissions(this, requiredPermissions, 1);
        }

        private void RequestEnableBluetooth()
        {
            const int REQUEST_ENABLE_BT = 2;
            var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
            StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
        }

        void InitializeBluetooth()
        {
            adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
            {
                Toast.MakeText(this, "Bluetooth is not available", ToastLength.Long).Show();
                Finish();
                return;
            }

            if (!adapter.IsEnabled)
                RequestEnableBluetooth();

            ListView list = FindViewById<ListView>(Resource.Id.listGekoppeld);
            ArrayAdapter ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            list.Adapter = ListAdapter;
            ListAdapter.Clear();
            foreach (BluetoothDevice dev in adapter.BondedDevices)
                ListAdapter.Add(dev.Name);
            bonded.Clear();
            bonded.AddRange(adapter.BondedDevices);
        }
    }
}