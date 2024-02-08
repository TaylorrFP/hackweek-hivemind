using Sandbox;
using Sandbox.Citizen;
using System;
using System.Diagnostics;


//This is just a holder for the inputs
//All of the actual movement should be calculated on the player pawn so it's easier to offset input lag?
public sealed class PlayerController : Component
{

	[Sync ] public Angles eyeAngle { get; set; }

	[Property] [Sync] public int forwardInput { get; set; }
	[Property] [Sync] public int strafeInput { get; set; }
	[Property] public Angles altEyeAngle { get; set; }

	[Property] [Sync] public float jumpTimer { get; set; } = 1f;


	protected override void OnUpdate()
	{

		if ( !IsProxy ) //if you own this
		{
			//zer strage values
			forwardInput = 0;
			strafeInput = 0;

			if ( Input.Down("Walk" ))
			{
				altEyeAngle = new Angles( (altEyeAngle.pitch + Input.MouseDelta.y * 0.1f).Clamp( -89.9f, 89.9f ), altEyeAngle.yaw - Input.MouseDelta.x * 0.1f, 0f );

			}
			else
			{
				//zero alt-look
				altEyeAngle = Angles.Zero;

				//set eye angles with inputs
				eyeAngle = new Angles( (eyeAngle.pitch + Input.MouseDelta.y * 0.1f).Clamp( -89.9f, 89.9f ), eyeAngle.yaw - Input.MouseDelta.x * 0.1f, 0f );

				//set strafe values with inputs
				//only do this when alt look is disabled to prevent exploiting it



				if ( Input.Down( "Forward" ) ) forwardInput = 1;
				if ( Input.Down( "Backward" ) ) forwardInput = -1;
				if ( Input.Down( "Backward" ) & Input.Down( "Forward" ) ) forwardInput = 0;

				if ( Input.Down( "Left" ) ) strafeInput = 1;
				if ( Input.Down( "Right" ) ) strafeInput = -1;
				if ( Input.Down( "Left" ) & Input.Down( "Right" ) ) strafeInput = 0;
			}

			jumpTimer = MathX.Clamp( jumpTimer - Time.Delta, 0, 10 );

		}


		//Debug
		//Log.Info( this.Network.OwnerConnection.DisplayName + ": " + eyeAngle.ToString() + " | Is Owner = " + this.Network.IsOwner + " | IsNetworked? = " + this.Network.Active);

		//Log.Info( this.Network.OwnerConnection.DisplayName + ": " + forwardInput + " | " + strafeInput + " | " + "Is Owner = " + this.Network.IsOwner + " | IsNetworked? = " + this.Network.Active);








	}
}
