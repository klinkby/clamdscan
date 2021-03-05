# Klinkby.Clam
Non-blocking .NET Standard client for ClamAV.
Nuget available at https://www.nuget.org/packages/Klinkby.Clam

## ClamAV?
[ClamAV](https://www.clamav.net/) is an open source antivirus engine for 
detecting trojans, viruses, malware & other malicious threats, 
available on many platforms including Linux, Windows and OSX. 

## Client library
This library use use 
[Task Parallel](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl) 
to provide an asynchronous, non-blocking, yet easily consumed interface. The library is effectively an adapter to 
abstract away the low level TCP protocol used by the Clam daemon (clamd). 

The target framework is .NET Standard 2.0 (compatible with Linux, Windows and OSX), thus runtime compatible with .NET 
Core 2.0 and later versions (and if you lingered at .NET Framework 4.6.1 and later versions).

## Getting started
If you do not already have Clam running on your box, it's easy to pull a docker container e.g. 
[mkodockx/docker-clamav](https://hub.docker.com/r/mkodockx/docker-clamav), which will expose the daemon on the default 
port to the host.

```sh
docker run -d -p 3310:3310 mkodockx/docker-clamav:alpine
```

In your .NET code add a reference to my nuget available at [Klinkby.Clam](https://www.nuget.org/packages/Klinkby.Clam).
Then to pull the Clam vesion from the daemon, simply write:

```csharp
using (var client = new Klinkby.Clam.ClamClient())
{
    string ver = await client.VersionAsync();
    Console.WriteLine(ver);
}
```

Clearly the `ClamClient` constructor has sane defaults but it does take optional parameters for hostname, ipadresses, 
port number and an ILogger for diagnostic tracing.

## Tests
The `Klinkby.ClamTests` project is a [xUnit](https://xunit.net/)-based integration test for verifying command handling. 
For practical reasons the `Shutdown` method is inhibited, as it terminates the clamd service, leaving the rest of the 
tests without a daemon. 

## Known issues
Unsupported commands are `RAWSCAN` as it is deprecated by Clam, and pending are `FILDES` (e.g. STDIN) and `IDSESSION`, `END`  (for batching). 

If you need any of the latter, I would be quite happy to take pull requests. :-)
