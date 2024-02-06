using Sandbox;
using Sandbox.Citizen;
using System.Diagnostics;


//This is just a holder for the inputs
//All of the actual movement should be calculated on the player pawn so it's easier to offset input lag?
public sealed class PlayerController : Component
{

	[Property][Sync] public Angles eyeAngle { get; set; }

	
	[Property] [Sync] public int forwardInput { get; set; }
	[Property] [Sync] public int strafeInput { get; set; }







	protected override void OnStart()
	{
		base.OnStart();



		//Log.Info( "Spawning PlayerController for: " + this.GameObject.Network.OwnerConnection.DisplayName );

		//gonna have to manually add these to the pawn and hope they sync?
	}

	protected override void OnUpdate()
	{

		if ( !IsProxy ) //if you own this
		{

			eyeAngle = new Angles( (eyeAngle.pitch + Input.MouseDelta.y * 0.1f).Clamp( -89.9f, 89.9f ), eyeAngle.yaw - Input.MouseDelta.x * 0.1f, 0f );

		

			var rot = eyeAngle.ToRotation();

			forwardInput = 0;
			strafeInput = 0;


			if ( Input.Down( "Forward" ) ) forwardInput = 1;
			if ( Input.Down( "Backward" ) ) forwardInput = -1;
			if ( Input.Down( "Backward" ) & Input.Down( "Forward" ) ) forwardInput = 0;

			if ( Input.Down( "Left" ) ) strafeInput = 1;
			if ( Input.Down( "Right" ) ) strafeInput = -1;
			if ( Input.Down( "Left" ) & Input.Down( "Right" ) ) strafeInput = 0;









		}



	




	}
}
