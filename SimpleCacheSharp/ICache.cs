using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleCacheSharp
{
	/// <summary>
	///     Interface for accessing a cache.
	/// </summary>
	public interface ICache : IDisposable
	{
		/// <summary>
		///     Set an expiry time for an already existing entry.
		/// </summary>
		/// <param name="key">Key of the entry to modify.</param>
		/// <param name="expiry">Specifies how long until the entry will expire.</param>
		/// <returns></returns>
		Task Expire( string key, TimeSpan expiry );

		/// <summary>
		///     Set an expiry date for an already existing entry.
		/// </summary>
		/// <param name="key">Key of the entry to modify.</param>
		/// <param name="expiresOn">Date when the entry expires.</param>
		/// <returns></returns>
		Task Expire( string key, DateTime expiresOn );

		/// <summary>
		///     Fetches an entry from the cache.
		/// </summary>
		/// <param name="key">Key of the entry.</param>
		/// <returns>Value of the entry or <c>null</c> if key was not found.</returns>
		Task<string> Get( string key );

		/// <summary>
		///     Lists all existing keys in the cache.
		/// </summary>
		/// <returns></returns>
		Task<List<string>> GetKeys();

		/// <summary>
		///     Removes an entry from the cache.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>Key of the entry.</returns>
		Task Remove( string key );

		/// <summary>
		///     Adds or updates an entry in the cache.
		/// </summary>
		/// <param name="key">Key of the entry.</param>
		/// <param name="value">Value of the entry.</param>
		/// <param name="expiresOn">Date when the entry expires.</param>
		/// <returns></returns>
		Task Set( string key, string value, DateTime? expiresOn );

		/// <summary>
		///     Adds or updates an entry in the cache.
		/// </summary>
		/// <param name="key">Key of the entry.</param>
		/// <param name="value">Value of the entry.</param>
		/// <param name="expiry">Specifies how long until the entry will expire.</param>
		/// <returns></returns>
		Task Set( string key, string value, TimeSpan? expiry );

		/// <summary>
		///     Adds or updates an entry in the cache.
		/// </summary>
		/// <param name="key">Key of the entry.</param>
		/// <param name="value">Value of the entry.</param>
		/// <returns></returns>
		Task Set( string key, string value );
	}
}