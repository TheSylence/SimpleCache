# SimpleCacheSharp
Simple cache implementation for .NET

## Installation
### Nuget
> Install-Package SimpleCacheSharp

### From source
**Visual Studio 2015 required.**

Restore nuget packages and copy *SQLITE.Interop.dll* from *x86* and *x64* in *packages/System.Data.SQLite.Core.1.0.xxx.0/build/net46/* to *costura32* and *costura64* in SimpleCacheSharp/ folder. (x86 goes into costura32 and x64 into costura64)

Or to put it in code:
> copy packages/System.Data.SQLite.Core.1.0.xxx.0/build/net46/x86/SQLite.Interop.dll SimpleCacheSharp/costura32

> copy packages/System.Data.SQLite.Core.1.0.xxx.0/build/net46/x64/SQLite.Interop.dll SimpleCacheSharp/costura64

Open solution and hit build. Now reference SimpleCacheSharp.dll in your project.

## Usage
### Constructing a cache
Create an instance of the cache that is stored on the disk:
```csharp
var cache = CacheFactory.Configure().UsingFile("cache.db").BuildCache();
```
using *Encrypt()* the cache file can be protected using a password:
```csharp
var cache = CacheFactory.Configure().UsingFile("cache.db").Encrypt("TopSecret").BuildCache();
```

If you want to store the cache in memory instead use *InMemory()*:
```csharp
var cache = CacheFactory.Configure().InMemory().BuildCache();
```

### Using the ICache interface
All methods in *ICache* are async since reading and writing to the database are potentially expensive operations.

#### Writing values to the cache
```csharp
await cache.Set("dataKey", "my cached value");
```
Will store *my cached value* under *dataKey* with no expiry (the value will be keep cached forever)

If an item with the same was already cached, a call to *Set()* with the same key, will update its value.

If you want your data to expire after some time use one of the *Set()* overloads that either take a DateTime (entry will expire at the given moment) or a TimeSpan (will expire in xyz seconds/minutes/...):
```csharp
await cache.Set("anotherKey", "Hello World", TimeSpan.FromSeconds(60));
```

**Note**: You cannot store *null* in the cache, because this value is used to indicate that a key does not exist in the cache.

#### Retrieving cached values
```csharp
var cachedValue = await cache.Get("dataKey");
```
*cachedValue* will now contain the string *my cached value*.
If nothing was cached using the key *dataKey* the method will return *null*.

#### Removing values from the cache
If you want to remove a value from that cache you simply call *Remove()*
```csharp
await cache.Remove("dataKey");
```

#### Altering the expiry of a cached item
If you want to change the expiry time of a cache item you have to call *Expire()* with the key of the item to update and the new expiry time.
```csharp
await cache.Expire("dataKey", TimeSpan.FromSeconds(120));
```