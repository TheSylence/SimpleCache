using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleCacheSharp.Tests
{
	[TestClass]
	public class CacheConfigurationTests
	{
		[TestMethod]
		public void BuildCacheCreatesNewCacheObject()
		{
			// Arrange
			var config = new CacheConfiguration().InMemory();

			// Act
			var cache = config.BuildCache();

			// Assert
			Assert.IsNotNull( cache );
		}

		[TestMethod]
		public void EmptyFileNameThrowsException()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => config.UsingFile( "" ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod]
		public void EmptyPasswordThrowsException()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => config.Encrypt( "" ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod]
		public void EncryptReturnsConfiguration()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ret = config.Encrypt( "password" );

			// Assert
			Assert.IsNotNull( ret );
			Assert.AreSame( config, ret );
		}

		[TestMethod]
		public void EncryptUsingPasswordDoesNotThrow()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => config.Encrypt( "password" ) );

			// Assert
			Assert.IsNull( ex );
		}

		[TestMethod]
		public async Task IncompleteConfigurationCannotBuildCache()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ex = await ExceptionAssert.CatchAsync<InvalidOperationException>( () => config.BuildCache() );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod]
		public void InMemoryReturnsConfiguration()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ret = config.InMemory();

			// Assert
			Assert.IsNotNull( ret );
			Assert.AreSame( config, ret );
		}

		[TestMethod]
		public void NullFileNameThrowsException()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => config.UsingFile( null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod]
		public void NullPasswordThrowsException()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => config.Encrypt( null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod]
		public void UsingFileMethodReturnsConfiguration()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ret = config.UsingFile( "C:/temp" );

			// Assert
			Assert.IsNotNull( ret );
			Assert.AreSame( config, ret );
		}

		[TestMethod]
		public void ValidFileNameDoesNotThrowException()
		{
			// Arrange
			var config = new CacheConfiguration();

			// Act
			var ex = ExceptionAssert.Catch<Exception>( () => config.UsingFile( "C:/temp" ) );

			// Assert
			Assert.IsNull( ex );
		}
	}
}