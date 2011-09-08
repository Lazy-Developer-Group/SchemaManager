using System.Collections.Generic;

namespace Utilities.Caching
{
	public class SimpleDictionaryCache<TKeyObject, TStorageObject> : ICache<TKeyObject, TStorageObject>
	{
		private readonly Dictionary<string, TStorageObject> _cache = new Dictionary<string, TStorageObject>();

		public virtual string BuildKeyString(TKeyObject keyObj)
		{
			return keyObj.ToString();
		}

		public void Clear()
		{
			_cache.Clear();
		}

		public void Add(TKeyObject keyObj, TStorageObject obj)
		{
			_cache.Add(BuildKeyString(keyObj), obj);
		}

		public TStorageObject Get(TKeyObject keyObj)
		{
			return _cache[BuildKeyString(keyObj)];
		}

		public bool Contains(TKeyObject keyObj)
		{
			return _cache.ContainsKey(BuildKeyString(keyObj));
		}

	}
}
