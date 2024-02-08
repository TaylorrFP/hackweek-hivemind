using Sandbox;
using System;

public sealed class Mine : Component, Component.ITriggerListener
{
	public void OnTriggerEnter( Collider other )
	{




			var playerPawn = other.GameObject.Components.Get<PlayerPawn>();

			if ( playerPawn != null )
			{

				playerPawn.Death();
			}

		

		
	}

	public void OnTriggerExit( Collider other )
	{
		
	}

	protected override void OnUpdate()
	{

	}
}
