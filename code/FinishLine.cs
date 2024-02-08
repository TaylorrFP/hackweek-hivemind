using Sandbox;
using static Sandbox.Internal.ICSharpCompiler;

public sealed class FinishLine : Component, Component.ITriggerListener
{
	protected override void OnUpdate()
	{

	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		

		Sound.Play( "assets/sounds/finish.sound",this.Transform.Position);
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
	
	}
}
