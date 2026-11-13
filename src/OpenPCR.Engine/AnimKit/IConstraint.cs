using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public interface IConstraint : IUpdatable
	{
		int Order { get; }
	}
}
