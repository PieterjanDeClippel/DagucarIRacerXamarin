using Android.Bluetooth;

namespace Dagucar.Events.EventArgs
{
	public class DeviceFoundEventArgs : System.EventArgs
	{
		public DeviceFoundEventArgs(BluetoothDevice Device)
		{
			this.Device = Device;
		}

        public BluetoothDevice Device { get; }
    }
}