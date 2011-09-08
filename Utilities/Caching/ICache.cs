namespace Utilities.Caching
{
	public interface ICache<TKeyObject, TStorageObject>
	{
		void Clear();
		void Add(TKeyObject keyObj, TStorageObject obj);
		TStorageObject Get(TKeyObject keyObj);
		bool Contains(TKeyObject keyObj);
	}
}
