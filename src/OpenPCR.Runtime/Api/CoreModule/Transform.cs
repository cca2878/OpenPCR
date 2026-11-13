using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	public class Transform : global::OpenPCR.Runtime.Api.Component, global::System.Collections.IEnumerable
	{
		[global::OpenPCR.Migration("96d23f5f-844d-4221-b685-4277d54902f9")]
		protected Transform()
		{

		}
		[global::OpenPCR.Migration("2645442e-64dc-4b9d-999c-2eac7156f3dd")]
		public void SetParent(global::OpenPCR.Runtime.Api.Transform p)
		{
			global::OpenPCR.Runtime.Impl.PreparedTransformInputs.SetParent(this, p, true);
		}
		[global::OpenPCR.Migration("f4117cdf-59fb-428d-a3dc-3206a5a8d8ff")]
		public void SetParent(global::OpenPCR.Runtime.Api.Transform parent, bool worldPositionStays)
		{
			global::OpenPCR.Runtime.Impl.PreparedTransformInputs.SetParent(this, parent, worldPositionStays);
		}
		[global::OpenPCR.Migration("c6dafdc3-9b19-415e-a49d-b17ad861a611")]
		public global::OpenPCR.Runtime.Api.Vector3 TransformPoint(global::OpenPCR.Runtime.Api.Vector3 position)
		{
			var result = global::OpenPCR.Runtime.Impl.PreparedTransformInputs.TransformPoint(this, new global::OpenPCR.Runtime.Impl.EngineVector(position.x, position.y, position.z));
			return new global::OpenPCR.Runtime.Api.Vector3(result.X, result.Y, result.Z);
		}
		[global::OpenPCR.Migration("72a024f7-7a77-43c6-b674-eb04ac49d6b3")]
		public global::OpenPCR.Runtime.Api.Vector3 InverseTransformPoint(global::OpenPCR.Runtime.Api.Vector3 position)
		{
			var result = global::OpenPCR.Runtime.Impl.PreparedTransformInputs.InverseTransformPoint(this, new global::OpenPCR.Runtime.Impl.EngineVector(position.x, position.y, position.z));
			return new global::OpenPCR.Runtime.Api.Vector3(result.X, result.Y, result.Z);
		}
		[global::OpenPCR.Migration("fc9ccc00-7c81-459d-8d28-3b8b5ffe1774")]
		public global::System.Collections.IEnumerator GetEnumerator()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Transform::GetEnumerator():System.Collections.IEnumerator", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("b7a6edcc-8f06-4d56-abe1-458418572680")]
		public global::OpenPCR.Runtime.Api.Transform GetChild(int index)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Transform::GetChild(System.Int32):OpenPCR.Runtime.Api.Transform", "Unity infrastructure not implemented");
		}
		public global::OpenPCR.Runtime.Api.Vector3 position
		{
			[global::OpenPCR.Migration("5f7977d4-e742-4e30-8512-daf294260ff2")]
			get
			{
				var value = global::OpenPCR.Runtime.Impl.PreparedTransformInputs.Position(this);
				return new global::OpenPCR.Runtime.Api.Vector3(value.X, value.Y, value.Z);
			}
			[global::OpenPCR.Migration("494820f0-9c4b-4fad-9332-c81d2bff055c")]
			set
			{
				global::OpenPCR.Runtime.Impl.PreparedTransformInputs.Position(this, new global::OpenPCR.Runtime.Impl.EngineVector(value.x, value.y, value.z));
			}
		}
		public global::OpenPCR.Runtime.Api.Vector3 localPosition
		{
			[global::OpenPCR.Migration("f05aa372-aaef-4ab8-8c1e-0c72e545aff1")]
			get
			{
				var value = global::OpenPCR.Runtime.Impl.PreparedTransformInputs.LocalPosition(this);
				return new global::OpenPCR.Runtime.Api.Vector3(value.X, value.Y, value.Z);
			}
			[global::OpenPCR.Migration("1f77fb7e-b697-47d0-938b-8ccf959b041b")]
			set
			{
				global::OpenPCR.Runtime.Impl.PreparedTransformInputs.LocalPosition(this, new global::OpenPCR.Runtime.Impl.EngineVector(value.x, value.y, value.z));
			}
		}
		public global::OpenPCR.Runtime.Api.Vector3 eulerAngles
		{
			[global::OpenPCR.Migration("81acde19-d142-4bf3-9d38-d6012f3ab0ef")]
			get
			{
				return rotation.eulerAngles;
			}
			[global::OpenPCR.Migration("298495dd-e86b-4097-bd13-868cb031e6f1")]
			set
			{
				if (value.x != 0f || value.y != 0f || !float.IsFinite(value.z)) throw new global::System.NotSupportedException("Prepared Euler writes require finite world Z rotation.");
				var q = Quaternion.Euler(value);
				global::OpenPCR.Runtime.Impl.PreparedTransformInputs.Rotation(this, new global::OpenPCR.Runtime.Impl.EngineQuaternion(q.x, q.y, q.z, q.w));
			}
		}
		public global::OpenPCR.Runtime.Api.Vector3 localEulerAngles
		{
			[global::OpenPCR.Migration("cd2b364e-81b9-49b9-bd83-b505585f8615")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Transform::get_localEulerAngles():OpenPCR.Runtime.Api.Vector3", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("6c0ea361-b226-44dc-b720-4ffe487959dd")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Transform::set_localEulerAngles(OpenPCR.Runtime.Api.Vector3):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Quaternion rotation
		{
			[global::OpenPCR.Migration("f65402a6-af8c-449a-bb42-a734e99a20ba")]
			get
			{
				var q = global::OpenPCR.Runtime.Impl.PreparedTransformInputs.Rotation(this);
				return new Quaternion(q.X, q.Y, q.Z, q.W);
			}
			[global::OpenPCR.Migration("430ef7e6-c3d0-4108-afa7-c697cbebd817")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}
		public global::OpenPCR.Runtime.Api.Quaternion localRotation
		{
			[global::OpenPCR.Migration("35d7e7a2-8921-4a62-935e-09f4f140b1e1")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Transform::get_localRotation():OpenPCR.Runtime.Api.Quaternion", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("b606ef2b-3539-4a28-a08f-9979a1dad7b0")]
			set
			{
				global::OpenPCR.Runtime.Impl.PreparedTransformInputs.LocalRotation(this, new global::OpenPCR.Runtime.Impl.EngineQuaternion(value.x, value.y, value.z, value.w));
			}
		}
		public global::OpenPCR.Runtime.Api.Vector3 localScale
		{
			[global::OpenPCR.Migration("3d2d1883-142a-445f-96ab-e4378ffe8751")]
			get
			{
				var value = global::OpenPCR.Runtime.Impl.PreparedTransformInputs.LocalScale(this);
				return new global::OpenPCR.Runtime.Api.Vector3(value.X, value.Y, value.Z);
			}
			[global::OpenPCR.Migration("d1aa5d5e-4b78-4ddf-8b98-6e5468339ee5")]
			set
			{
				global::OpenPCR.Runtime.Impl.PreparedTransformInputs.LocalScale(this, new global::OpenPCR.Runtime.Impl.EngineVector(value.x, value.y, value.z));
			}
		}
		public global::OpenPCR.Runtime.Api.Transform parent
		{
			[global::OpenPCR.Migration("3d26b704-cb04-4da5-9dba-5c5057223ea0")]
			get
			{
				return (global::OpenPCR.Runtime.Api.Transform)global::OpenPCR.Runtime.Impl.SceneScripting.Parent(this);
			}
			[global::OpenPCR.Migration("22299e5f-6033-4378-b9a7-d75de62491e5")]
			set
			{
				global::OpenPCR.Runtime.Impl.PreparedTransformInputs.SetParent(this, value, true);
			}
		}
		public int childCount
		{
			[global::OpenPCR.Migration("36208ac1-d360-4572-93c9-e1ddc7cd4940")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Transform::get_childCount():System.Int32", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Vector3 lossyScale
		{
			[global::OpenPCR.Migration("5b9d7eef-1b5f-4d1b-bf61-24102c587226")]
			get
			{
				var value = global::OpenPCR.Runtime.Impl.PreparedTransformInputs.Scale(this);
				return new global::OpenPCR.Runtime.Api.Vector3(value.X, value.Y, value.Z);
			}
		}
	}
}
