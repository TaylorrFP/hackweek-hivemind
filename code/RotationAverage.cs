using Sandbox;
using System.Collections.Generic;
using System.Numerics;

public sealed class RotationAverage : Component
{
	[Property] public List<GameObject> objects { get; set; }
	protected override void OnUpdate()
	{
		Quaternion[] quaternions = new Quaternion[objects.Count];
		//Vector3[] vectors = new Vector3[objects.Count];


		var averageVector = Vector3.Zero;

		for (int i = 0; i < objects.Count; i++)
		{
			quaternions[i] = objects[i].Transform.Rotation;
			averageVector += objects[i].Transform.Rotation.Forward;
		}

		//Averaging Quats isn't really what we want?
		//this.Transform.Rotation = AverageQuaternion(quaternions);


		averageVector = averageVector.Normal;
		Log.Info( averageVector );

		this.Transform.Rotation = Rotation.LookAt( averageVector );

		//this.Transform.Rotation = Quaternion.CreateFromAxisAngle( averageVector, 0f );


		//this.Transform.Rotation = Quaternion.CreateFromAxisAngle( averageVector, 0f );




	}


	public Quaternion AverageQuaternion( Quaternion[] quaternions )
	{
		if ( quaternions == null || quaternions.Length < 1 )
			return Quaternion.Identity;

		if ( quaternions.Length < 2 )
			return quaternions[0];

		int count = quaternions.Length;
		float weight = 1.0f / (float)count;
		Quaternion avg = Quaternion.Identity;

		for ( int i = 0; i < count; i++ )
			avg *= Quaternion.Slerp( Quaternion.Identity, quaternions[i], weight );

		return avg;

	}
}
