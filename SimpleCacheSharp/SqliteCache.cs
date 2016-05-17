using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Fody;
using Resourcer;

namespace SimpleCacheSharp
{
	[ConfigureAwait( false )]
	internal class SqliteCache : ICache
	{
		public SqliteCache( string connectionString )
		{
			Connection = new SQLiteConnection( connectionString );
		}

		internal SqliteCache( SQLiteConnection connection )
		{
			Connection = connection;
		}

		internal async Task Initialize()
		{
			await Connection.OpenAsync();

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = Resource.AsString( "DDL.sql" );
				await cmd.ExecuteNonQueryAsync();
			}
		}

		public void Dispose()
		{
			Connection.Dispose();
		}

		public async Task Expire( string key, TimeSpan expiry )
		{
			await Expire( key, DateTime.Now.Add( expiry ) );
		}

		public async Task Expire( string key, DateTime expiresOn )
		{
			if( string.IsNullOrEmpty( key ) )
			{
				throw new ArgumentNullException( nameof( key ) );
			}

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "UPDATE DataCache SET Expires = @expire WHERE Key = @key;";
				cmd.AddParameter( "key", key );
				cmd.AddParameter( "expire", SqliteHelper.GetDateValue( expiresOn ) );

				await cmd.ExecuteNonQueryAsync();
			}
		}

		public async Task<string> Get( string key )
		{
			if( string.IsNullOrEmpty( key ) )
			{
				throw new ArgumentNullException( nameof( key ) );
			}

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "DELETE FROM DataCache WHERE Expires <= @now;";
				cmd.AddParameter( "now", SqliteHelper.GetDateValue( DateTime.Now ) );
				await cmd.ExecuteNonQueryAsync();
			}

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Value FROM DataCache WHERE Key = @key;";
				cmd.AddParameter( "key", key );

				var res = await cmd.ExecuteScalarAsync();
				return res as string;
			}
		}

		public async Task<List<string>> GetKeys()
		{
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Key FROM DataCache";

				List<string> keyList = new List<string>();
				using( var reader = await cmd.ExecuteReaderAsync() )
				{
					while( await reader.ReadAsync() )
					{
						keyList.Add( await reader.GetFieldValueAsync<string>( 0 ) );
					}
				}

				return keyList;
			}
		}

		public async Task Remove( string key )
		{
			if( string.IsNullOrEmpty( key ) )
			{
				throw new ArgumentNullException( nameof( key ) );
			}

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "DELETE FROM DataCache WHERE Key = @key;";
				cmd.AddParameter( "key", key );

				await cmd.ExecuteNonQueryAsync();
			}
		}

		public async Task Set( string key, string value )
		{
			await Set( key, value, (DateTime?)null );
		}

		public async Task Set( string key, string value, DateTime? expiresOn )
		{
			if( string.IsNullOrEmpty( key ) )
			{
				throw new ArgumentNullException( nameof( key ) );
			}

			if( value == null )
			{
				throw new ArgumentNullException( nameof( value ) );
			}

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.AddParameter( "key", key );
				cmd.AddParameter( "value", value );

				if( expiresOn.HasValue )
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value,Expires) VALUES( @key, @value, @expires );";
					cmd.AddParameter( "expires", SqliteHelper.GetDateValue( expiresOn.Value ) );
				}
				else
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value) VALUES( @key, @value );";
				}

				await cmd.ExecuteNonQueryAsync();
			}
		}

		public async Task Set( string key, string value, TimeSpan? expiry )
		{
			DateTime? expireDate = null;
			if( expiry.HasValue )
			{
				expireDate = DateTime.Now.Add( expiry.Value );
			}

			await Set( key, value, expireDate );
		}

		private readonly SQLiteConnection Connection;
	}
}