using Android.Bluetooth;

namespace Dagucar.Events.EventArgs
{
	public class BondStateChangedEventArgs : System.EventArgs
	{
		public BondStateChangedEventArgs(BluetoothDevice Device, Bond OldState, Bond NewState)
		{
			this.Device = Device;
			this.OldState = OldState;
			this.NewState = NewState;
		}

        public BluetoothDevice Device { get; }
        public Bond OldState { get; }
        public Bond NewState { get; }
    }
}