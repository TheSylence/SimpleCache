using System;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace SimpleCacheSharp
{
	internal class CacheConfiguration : ICacheConfiguration
	{
		public CacheConfiguration()
		{
			ConnectionStringBuilder = new SQLiteConnectionStringBuilder();
		}

		public async Task<ICache> BuildCache()
		{
			if( string.IsNullOrEmpty( ConnectionStringBuilder.DataSource ) )
			{
				throw new InvalidOperationException( "Cache is not completely configured" );
			}

			var cache = new SqliteCache( ConnectionStringBuilder.ToString() );
			await cache.Initialize();
			return cache;
		}

		public ICacheConfiguration Encrypt( string password )
		{
			if( string.IsNullOrWhiteSpace( password ) )
			{
				throw new ArgumentNullException( nameof( password ) );
			}

			ConnectionStringBuilder.Password = password;
			return this;
		}

		public ICacheConfiguration InMemory()
		{
			ConnectionStringBuilder.DataSource = ":memory:";
			return this;
		}

		public ICacheConfiguration UsingFile( string fileName )
		{
			if( string.IsNullOrEmpty( fileName ) )
			{
				throw new ArgumentNullException( nameof( fileName ) );
			}

			ConnectionStringBuilder.DataSource = fileName;
			return this;
		}

		private readonly SQLiteConnectionStringBuilder ConnectionStringBuilder;
	}
}