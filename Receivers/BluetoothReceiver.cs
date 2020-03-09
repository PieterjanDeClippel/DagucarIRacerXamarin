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
using Dagucar.Events.EventArgs;
using Dagucar.Events.EventHandlers;
using Java.Util;

namespace Dagucar.Receivers
{
    public class BluetoothReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
		{
			switch (intent.Action)
			{
				case BluetoothDevice.ActionFound:
					BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
					OnDeviceFound(new DeviceFoundEventArgs(device));
					break;
				case BluetoothAdapter.ActionDiscoveryStarted:
					OnDiscoveryStarted(EventArgs.Empty);
					break;
				case BluetoothAdapter.ActionDiscoveryFinished:
					OnDiscoveryFinished(EventArgs.Empty);
					break;
				case BluetoothDevice.ActionBondStateChanged:
					BluetoothDevice device2 = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
					Bond oldState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraPreviousBondState);
					Bond newState = (Bond)(int)intent.GetParcelableExtra(BluetoothDevice.ExtraBondState);
					OnBondStateChanged(new BondStateChangedEventArgs(device2, oldState, newState));
					break;
				case BluetoothDevice.ActionUuid:
					UUID uuid = (UUID)intent.GetParcelableExtra(BluetoothDevice.ExtraUuid);
					OnUUIDFetched(new UuidFetchedEventArgs(uuid));
					break;
			}
		}

		#region DeviceFound
		public event DeviceFoundEventHandler DeviceFound;
		protected void OnDeviceFound(DeviceFoundEventArgs e)
		{
			if (DeviceFound != null)
				DeviceFound(this, e);
		}
		#endregion
		#region DiscoveryStarted
		public event EventHandler DiscoveryStarted;
		protected void OnDiscoveryStarted(EventArgs e)
		{
			if (DiscoveryStarted != null)
				DiscoveryStarted(this, e);
		}
		#endregion
		#region DiscoveryFinished
		public event EventHandler DiscoveryFinished;
		protected void OnDiscoveryFinished(EventArgs e)
		{
			if (DiscoveryFinished != null)
				DiscoveryFinished(this, e);
		}
		#endregion
		#region BondStateChanged
		public event BondStateChangedEventHandler BondStateChanged;
		protected void OnBondStateChanged(BondStateChangedEventArgs e)
		{
			if (BondStateChanged != null)
				BondStateChanged(this, e);
		}
		#endregion
		#region UuidFetched
		public event UuidFetchedEventHandler UuidFetched;
		protected void OnUUIDFetched(UuidFetchedEventArgs e)
		{
			if (UuidFetched != null)
				UuidFetched(this, e);
		}
		#endregion
	}
}