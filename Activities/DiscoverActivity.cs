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
using Dagucar.Receivers;

namespace Dagucar.Activities
{
    [Activity(Label = "DiscoverActivity")]
    public class DiscoverActivity : Activity
    {
        private Button btnDiscover;
        private ListView listFoundDevices;
        private BluetoothAdapter bluetoothAdapter;
        private BluetoothReceiver bluetoothReceiver;
        private List<BluetoothDevice> foundDevices;
        private ArrayAdapter foundDeviceNamesAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_discovery);
            InitializeBluetoothReceiver();

            btnDiscover = FindViewById<Button>(Resource.Id.btnDiscover);
            listFoundDevices = FindViewById<ListView>(Resource.Id.listFoundDevices);

            foundDevices = new List<BluetoothDevice>();
            listFoundDevices.Adapter = foundDeviceNamesAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);

            btnDiscover.Click += BtnDiscover_Click;
            listFoundDevices.ItemClick += ListFoundDevices_ItemClick;
        }

        private void InitializeBluetoothReceiver()
        {
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            bluetoothReceiver = new BluetoothReceiver();

            bluetoothReceiver.DeviceFound += BluetoothReceiver_DeviceFound;
            RegisterReceiver(bluetoothReceiver, new IntentFilter(BluetoothDevice.ActionFound));
            bluetoothReceiver.DiscoveryStarted += BluetoothReceiver_DiscoveryStarted;
            RegisterReceiver(bluetoothReceiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted));
            bluetoothReceiver.DiscoveryFinished += BluetoothReceiver_DiscoveryFinished;
            RegisterReceiver(bluetoothReceiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
            bluetoothReceiver.BondStateChanged += BluetoothReceiver_BondStateChanged;
            RegisterReceiver(bluetoothReceiver, new IntentFilter(BluetoothDevice.ActionBondStateChanged));
            //rec.UUIDFetched += Rec_UUIDFetched;
            //RegisterReceiver(rec, new IntentFilter(BluetoothDevice.ActionUuid));
        }

        private void BtnDiscover_Click(object sender, EventArgs e)
        {
            if (bluetoothAdapter.IsDiscovering)
            {
                bluetoothAdapter.CancelDiscovery();
            }
            else
            {
                foundDevices.Clear();
                foundDeviceNamesAdapter.Clear();
                bluetoothAdapter.StartDiscovery();
            }
        }

        private void BluetoothReceiver_DiscoveryStarted(object sender, EventArgs e)
        {
            btnDiscover.Text = "Stop Discovery";
        }

        private void BluetoothReceiver_DeviceFound(object sender, Events.EventArgs.DeviceFoundEventArgs e)
        {
            if (e.Device == null) return;
            if (e.Device.Name == null) return;
            if (e.Device.BondState == Bond.Bonded | e.Device.BondState == Bond.Bonding) return;
            if (foundDevices.Any(dev => dev.Address == e.Device.Address)) return;

            foundDeviceNamesAdapter.Add(e.Device.Name);
            foundDevices.Add(e.Device);
        }

        private void BluetoothReceiver_DiscoveryFinished(object sender, EventArgs e)
        {
            btnDiscover.Text = "Restart Discovery";
        }

        private void ListFoundDevices_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            foundDevices[e.Position].CreateBond();
        }

        private void BluetoothReceiver_BondStateChanged(object sender, Events.EventArgs.BondStateChangedEventArgs e)
        {
            if (e.NewState == Bond.Bonded)
            {
                foundDeviceNamesAdapter.Remove(e.Device.Name);
                foundDevices.Remove(e.Device);
                Toast.MakeText(this, "Gekoppeld met " + e.Device.Name, ToastLength.Short);
            }
            else
            {
                Toast.MakeText(this, "Bezig met koppelen met " + e.Device.Name, ToastLength.Long);
            }
        }
    }
}