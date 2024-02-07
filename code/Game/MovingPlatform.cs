using Sandbox;
using System;
using System.Linq;

public sealed class MovingPlatform : Component, Component.ICollisionListener
{
	private Vector3 startPosition;
	[Property] public BoxCollider boxCollider { get; set; }

	[Property] public float frequency { get; set; }
	[Property] public float magnitude { get; set; }
	[Property] public float offset { get; set; }

	[Property] public GameObject playerPawn { get; set; }

	[Property] public SoundPointComponent soundPoint { get; set; }
	[Property] public float soundAttenuation { get; set; }

	[Property] public float playerDist { get; set; }

	[Property] public Curve curveTest { get; set; }

	public void OnCollisionStart( Collision other )
	{
		Log.Info( other );

		var characterController = other.Other.GameObject.Components.Get<CharacterController>();

		if ( characterController != null )
		{

			//characterController.Punch( Transform.Rotation.Forward * MathF.Sin( Time.Now * frequency + offset ) * magnitude * 50f );

			characterController.Velocity += Transform.Rotation.Forward * MathF.Sin( Time.Now * frequency + offset ) * magnitude * 10f;
		}
		
	}

	public void OnCollisionStop( CollisionStop other )
	{
		//Log.Info( other );
	}

	public void OnCollisionUpdate( Collision other )
	{
		//Log.Info( other );

		//var characterController = other.Other.GameObject.Components.Get<CharacterController>();

		//if ( characterController != null )
		//{

		//	characterController.Velocity = Transform.Rotation.Forward * MathF.Sin( Time.Now * frequency + offset ) * magnitude * 50f;
		//}
	}

	protected override void OnStart()
	{
		base.OnStart();


		startPosition = Transform.Position;

		
	}
	protected override void OnUpdate()
	{
		Transform.Position = startPosition + Transform.Rotation.Forward * MathF.Sin( Time.Now * frequency + offset ) * magnitude;

		
		playerDist = 1- ((this.Transform.Position - playerPawn.Transform.Position).Length*0.002f);

		
		soundPoint.Volume = curveTest.Evaluate( playerDist );

	}

}
