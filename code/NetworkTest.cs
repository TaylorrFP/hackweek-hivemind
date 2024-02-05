using Sandbox;

public sealed class NetworkTest : Component
{

	[Sync] public int Clicks { get; set; }
	protected override void OnUpdate()
	{
		if ( Input.Pressed( "attack1") & !IsProxy )
		{
			Clicks++;

		}


		Log.Info( Network.OwnerConnection.DisplayName + "Clicks = " + Clicks );
	}
}
