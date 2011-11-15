NConfig
=======
NConfig is a .Net library that allows using multi-file, host-based configurations in .Net

This is thing you are really missing during staged deployments and with developer dependent environments.

### License
All Original Software is licensed under the MIT License (see LICENSE.txt) and does not apply to any other 3rd party tools, utilities or code which may be used to develop this application.

Please inform me if anyone is aware of any license violations that this code may be making.

### Building
The easiest way to build NConfig from source is to clone the git repository on GitHub and build the NConfig solution.

`git clone git://github.com/Yegoroff/NConfig.git`

The solution file `NConfig.sln` is located in the root of the repo.

### NuGet support
Currently NConfig is not published on NuGet official package source but I'm working on it.

However you can create NuGet package and put it to your local NuGet hive.
To do this please run NuGet.bat, package will be put to NuGet subfolder.

### Examples
To enable NConfig in your code, you need only one code line in your application startup section:

`NConfigurator.UsingFiles("Config\\Custom.config", "Config\\Connections.config").SetAsSystemDefault();`

As result configurations from files *{HostName}.Custom.config* and *{HostName}.Connections.config* in *Config* subfolder will be merged and used in ConfigurationManager instead of default configuration.

Where **{HostName}** is the current machine name.

In case *{HostName}.Custom.config* do not exist, *Custom.config* file will be used. Moreover if *Custom.config* is missing too, App.config (or Web.config) will be used.

#### Sample code from Samples\ConsoleTest:
```csharp
	// Setup NConfigurator to use Custom.config file from Config subfolder.
	NConfigurator.UsingFile(@"Config\Custom.config").SetAsSystemDefault();


	var testSection = NConfigurator.Default.GetSection<TestConfigSection>();

	var configManagerTestSection = ConfigurationManager.GetSection("TestConfigSection") as TestConfigSection;

	var namedTestSection = NConfigurator.UsingFile(@"Config\Custom.config").GetSection<TestConfigSection>("NamedSection");

	Console.WriteLine("NConfig Default : " + testSection.TestValue);
	Console.WriteLine("ConfigurationManager : " + configManagerTestSection.TestValue);
	Console.WriteLine("NConfig named section : " + namedTestSection.TestValue);

	Console.WriteLine("Merged APP Settings : ");
	foreach(var key in ConfigurationManager.AppSettings.AllKeys)
	{
		Console.WriteLine(key + " : " +  ConfigurationManager.AppSettings[key]);
	}
```

For more complex examples please refer to Samples subfolder.

### Issues
Currently NConfig was tested against ASP.Net, ASP.Net MVC, WinServices, WinForms/WPF, Console apps.
Other environments are supposed to be supported too, but I haven't tested them yet.

If you encounter any issues please do not hesitate to inform me using GitHub direct messages or raising issue on GitHub.

### Thanks
I would like to thank my colleagues for helping me testing and fixing this tool. Also thanks to TLK for his help in this file creation.
Also I would like to thank the guys at 31337 chat for their support.
