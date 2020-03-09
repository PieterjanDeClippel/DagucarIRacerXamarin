using Java.Util;

namespace Dagucar.Events.EventArgs
{
	public class UuidFetchedEventArgs : System.EventArgs
	{
		public UuidFetchedEventArgs(UUID uuid)
		{
			this.UUID = uuid;
		}

        public UUID UUID { get; }
    }
}