using System;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleCacheSharp.Tests
{
	[TestClass]
	public class SqliteCacheTests
	{
		[TestMethod, TestCategory( "TestCategory" )]
		public void DisposingCacheDisposesConnection()
		{
			// Arrange
			bool disposed = false;
			var connection = OpenConnection();
			connection.Disposed += ( s, e ) => disposed = true;

			var cache = new SqliteCache( connection );

			// Act
			cache.Dispose();

			// Assert
			Assert.IsTrue( disposed );
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task ExpiredValueIsNotRead()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value,Expires) VALUES ('test', '123', 1);";
					await cmd.ExecuteNonQueryAsync();
				}

				// Act
				var get = await cache.Get( "test" );

				// Assert
				Assert.IsNull( get );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task ExpiresWithNullKeyThrowsException()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				// Act
				var ex =
					await ExceptionAssert.CatchAsync<ArgumentNullException>( () => cache.Expire( null, TimeSpan.FromSeconds( 1 ) ) );

				// Assert
				Assert.IsNotNull( ex );
				Assert.AreEqual( "key", ex.ParamName );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task ExpiryCanBeUpdated()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value) VALUES ('test', '123');";
					await cmd.ExecuteNonQueryAsync();
				}

				var now = SqliteHelper.GetDateValue( DateTime.Now );

				// Act
				await cache.Expire( "test", TimeSpan.FromSeconds( 5 ) );

				// Assert
				var nowPlus10 = SqliteHelper.GetDateValue( DateTime.Now.AddSeconds( 10 ) );

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT Expires FROM DataCache WHERE Key = 'test';";
					var res = await cmd.ExecuteScalarAsync();
					var fromDb = Convert.ToUInt64( res );

					Assert.AreNotEqual( 0, fromDb );
					Assert.IsTrue( fromDb > now );
					Assert.IsTrue( fromDb < nowPlus10 );
				}
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task GetKeysReturnsCorrectList()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value) VALUES ('test', '123');";
					await cmd.ExecuteNonQueryAsync();
				}
				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value) VALUES ('test2', '123');";
					await cmd.ExecuteNonQueryAsync();
				}
				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value) VALUES ('abc', '123');";
					await cmd.ExecuteNonQueryAsync();
				}

				// Act
				var keys = await cache.GetKeys();

				// Assert
				CollectionAssert.AreEquivalent( new[] {"test", "test2", "abc"}, keys );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task GetNullKeyThrowsException()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				// Act
				var ex = await ExceptionAssert.CatchAsync<ArgumentNullException>( () => cache.Get( null ) );

				// Assert
				Assert.IsNotNull( ex );
				Assert.AreEqual( "key", ex.ParamName );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task GettingExistingValueReturnsCorrectData()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value) VALUES ('test', '123');";
					await cmd.ExecuteNonQueryAsync();
				}

				// Act
				var get = await cache.Get( "test" );

				// Assert
				Assert.AreEqual( "123", get );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task GettingNonExistingValueReturnsNull()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				// Act
				var get = await cache.Get( "unknown" );

				// Assert
				Assert.IsNull( get );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task NullCannotBeSetAsValue()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				// Act
				var ex = await ExceptionAssert.CatchAsync<ArgumentNullException>( () => cache.Set( "key", null ) );

				// Assert
				Assert.IsNotNull( ex );
				Assert.AreEqual( "value", ex.ParamName );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task RemovingKeyRemovesEntryFromDatabase()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO DataCache (Key,Value) VALUES ('test', '123');";
					await cmd.ExecuteNonQueryAsync();
				}

				// Act
				await cache.Remove( "test" );

				// Assert
				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT Value FROM DataCache WHERE Key = 'test';";
					var fromDb = await cmd.ExecuteScalarAsync();

					Assert.IsNull( fromDb );
				}
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task RemovingNonExistingKeyDoesNothing()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				// Act
				var ex = await ExceptionAssert.CatchAsync<Exception>( () => cache.Remove( "unknown" ) );

				// Assert
				Assert.IsNull( ex );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task RemovingNullKeyThrowsException()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				// Act
				var ex = await ExceptionAssert.CatchAsync<ArgumentNullException>( () => cache.Remove( null ) );

				// Assert
				Assert.IsNotNull( ex );
				Assert.AreEqual( "key", ex.ParamName );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task SettingNullKeyThrowsException()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				// Act
				var ex = await ExceptionAssert.CatchAsync<ArgumentNullException>( () => cache.Set( null, "123" ) );

				// Assert
				Assert.IsNotNull( ex );
				Assert.AreEqual( "key", ex.ParamName );
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task ValueCanBeInsertedIntoCache()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				// Act
				await cache.Set( "test", "123" );

				// Assert
				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT Value FROM DataCache WHERE Key = 'test';";
					var fromDb = await cmd.ExecuteScalarAsync();

					Assert.AreEqual( "123", fromDb );
				}
			}
		}

		[TestMethod, TestCategory( "TestCategory" )]
		public async Task ValueWithExpiryCanBeInsertedIntoCache()
		{
			// Arrange
			using( var connection = OpenConnection() )
			{
				var cache = new SqliteCache( connection );
				await cache.Initialize();

				var now = SqliteHelper.GetDateValue( DateTime.Now );

				// Act
				await cache.Set( "test", "123", TimeSpan.FromSeconds( 5 ) );

				// Assert
				var nowPlus10 = SqliteHelper.GetDateValue( DateTime.Now.AddSeconds( 10 ) );

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT Expires FROM DataCache WHERE Key = 'test';";
					var res = await cmd.ExecuteScalarAsync();
					var fromDb = Convert.ToUInt64( res );

					Assert.AreNotEqual( 0, fromDb );
					Assert.IsTrue( fromDb > now );
					Assert.IsTrue( fromDb < nowPlus10 );
				}
			}
		}

		private static SQLiteConnection OpenConnection()
		{
			var sb = new SQLiteConnectionStringBuilder
			{
				DataSource = ":memory:"
			};

			return new SQLiteConnection( sb.ToString() );
		}
	}
}