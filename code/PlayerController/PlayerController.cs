using Sandbox;
using Sandbox.Citizen;
using System.Diagnostics;


//This is just a holder for the inputs
//All of the actual movement should be calculated on the player pawn so it's easier to offset input lag?
public sealed class PlayerController : Component
{

	[Property][Sync] public Angles eyeAngle { get; set; }
	[Property][Sync] public Vector3 inputVelocity { get; set; }







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

			inputVelocity = 0;

			var rot = eyeAngle.ToRotation();
			if ( Input.Down( "Forward" ) ) inputVelocity += rot.Forward;
			if ( Input.Down( "Backward" ) ) inputVelocity += rot.Backward;
			if ( Input.Down( "Left" ) ) inputVelocity += rot.Left;
			if ( Input.Down( "Right" ) ) inputVelocity += rot.Right;

			inputVelocity = inputVelocity.WithZ( 0 );

			if ( !inputVelocity.IsNearZeroLength ) inputVelocity = inputVelocity.Normal;

			//Log.Info( WishVelocity );





		}



	




	}
}
