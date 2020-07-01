using Composite.Entities;

namespace Composite.Interfaces
{
	public interface IGiftOperations
	{
		void Add(GiftBase gift);
		void Remove(GiftBase gift);
	}
}
