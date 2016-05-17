using System;
using System.Threading.Tasks;

namespace SimpleCacheSharp.Tests
{
	internal static class ExceptionAssert
	{
		public static TException Catch<TException>( Action action ) where TException : Exception
		{
			TException catched = null;

			try
			{
				action();
			}
			catch( TException ex )
			{
				catched = ex;
			}

			return catched;
		}

		public static async Task<TException> CatchAsync<TException>( Func<Task> action )
			where TException : Exception
		{
			TException catched = null;

			try
			{
				await action();
			}
			catch( TException ex )
			{
				catched = ex;
			}

			return catched;
		}
	}
}