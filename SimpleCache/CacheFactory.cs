namespace SimpleCache
{
	public static class CacheFactory
	{
		public static ICacheConfiguration Configure()
		{
			return new CacheConfiguration();
		}
	}
}