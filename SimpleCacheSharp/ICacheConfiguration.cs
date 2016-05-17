using System.Threading.Tasks;

namespace SimpleCacheSharp
{
	/// <summary>
	///     Used to configure and build <see cref="ICache" /> instances.
	/// </summary>
	public interface ICacheConfiguration
	{
		/// <summary>
		///     Constructs an <see cref="ICache" /> instance using this configuration.
		/// </summary>
		/// <returns>The constructed <see cref="ICache" /></returns>
		Task<ICache> BuildCache();

		/// <summary>
		///     Encrypt the cache with a password.
		/// </summary>
		/// <param name="password">The password to encrypt the cache with.</param>
		/// <returns></returns>
		ICacheConfiguration Encrypt( string password );

		/// <summary>
		///     Construct an in memory cache.
		/// </summary>
		/// <returns></returns>
		ICacheConfiguration InMemory();

		/// <summary>
		///     Construct a persistent cache that is stored on disk.
		/// </summary>
		/// <param name="fileName">Name of the file to store the cache in.</param>
		/// <returns></returns>
		ICacheConfiguration UsingFile( string fileName );
	}
}