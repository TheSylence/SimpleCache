namespace SimpleCacheSharp
{
	public static class CacheFactory
	{
		public static ICacheConfiguration Configure()
		{
			return new CacheConfiguration();
		}
	}
}