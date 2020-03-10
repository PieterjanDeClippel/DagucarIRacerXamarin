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
using Java.Util;

namespace Dagucar.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private BluetoothAdapter adapter;
        private List<BluetoothDevice> bondedDevices = new List<BluetoothDevice>();
        private Button btnDiscover;
        private ListView listGekoppeld;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            btnDiscover = FindViewById<Button>(Resource.Id.btnDiscover);
            listGekoppeld = FindViewById<ListView>(Resource.Id.listGekoppeld);

            btnDiscover.Click += BtnDiscover_Click;
            listGekoppeld.ItemClick += ListGekoppeld_ItemClick;
        }

        protected override void OnStart()
        {
            base.OnStart();
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
                Manifest.Permission.BluetoothAdmin,
                Manifest.Permission.AccessCoarseLocation,
                "android.hardware.sensor.accelerometer"
            };

            ActivityCompat.RequestPermissions(this, requiredPermissions, 1);
        }

        #region Request enable bluetooth
        const int REQUEST_ENABLE_BT = 2;
        private void RequestEnableBluetooth()
        {
            var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
            StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode != REQUEST_ENABLE_BT)
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }
            else if (resultCode == Result.Canceled)
            {
                Toast.MakeText(this, "This app requires bluetooth", ToastLength.Long).Show();
                Finish();
            }
        }
        #endregion

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
            bondedDevices.Clear();
            bondedDevices.AddRange(adapter.BondedDevices);
        }


        private void BtnDiscover_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(DiscoverActivity));
        }

        private void ListGekoppeld_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var device = bondedDevices[e.Position];
            adapter.CancelDiscovery();

            var uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
            var sock = device.CreateRfcommSocketToServiceRecord(uuid);
            sock.Connect();

            RaceActivity.Socket = sock;
            StartActivity(typeof(RaceActivity));
        }
    }
}