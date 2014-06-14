NConfig
=======
NConfig is a .Net library that allows using multi-file, host-based configurations in .Net

This is thing you are really missing during staged deployments and with developer dependent environments.

* [License](#license)
* [Building](#building)
* [NuGet support](#nuget-support)
* [Examples](#examples)
* [log4net integration](#log4net)
* [Debug](#debug)
* [Issues](#issues)


### License
All Original Software is licensed under the MIT License (see LICENSE.txt) and does not apply to any other 3rd party tools, utilities or code which may be used to develop this application.

Please inform me if anyone is aware of any license violations that this code may be making.

### Building
The easiest way to build NConfig from source is to clone the git repository on GitHub and build the NConfig solution.

`git clone git://github.com/Yegoroff/NConfig.git`

The solution file `NConfig.sln` is located in the root of the repo.

### NuGet support
You can find **NConfig** in NuGet Gallery or just install it using VS *NuGet Packages Manager*. <br/>
Or just type `Install-Package NConfig` in Package Manager Console.

Also you can create NuGet package and put it to your local NuGet hive.
To do this please run NuGet.bat, newly created package will be put to NuGet subfolder.

### Examples
To enable NConfig in your code, you need only one code line in your application startup section:

`NConfigurator.UsingFiles("Config\\Custom.config", "Config\\Connections.config").SetAsSystemDefault();`

As result configurations from files *{HostName}.Custom.config* and *{HostName}.Connections.config* in *Config* subfolder will be merged and used in ConfigurationManager instead of default configuration.

Where **{HostName}** is the current machine name.

In case *{HostName}.Custom.config* do not exist, *Custom.config* file will be used. Moreover if *Custom.config* is missing too, App.config (or Web.config) will be used.

**Host Alias and Environment Variable**<br/>
You can create a custmom alias for a host by setting the HostName and Alias in a HostMap.config. It will then use the *{AliasName}.Custom.Config* instead of the HostName. See the examples for details. <br/>
It's also possible to supply an alias name by setting the environment variable *NCONFIG_ALIAS* for the running process. This is useful for cloud hosting scenario's where you don't know the hostname in advance. <br/>
For Microsoft Azure Websites you can set the *NCONFIG_ALIAS* as an App Setting through the Azure portal and it will be available as environment variable for NConfig.

**NOTE:**<br/> You should set **Copy To Output Directory = Copy Always** for all your custom configuration files, otherwise they will not be copied to *Bin* folder and NConfig will not find them.

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

### Integrations

#### Log4Net
 In most cases this very popular logging library uses direct access to App.config(Web.config) XML, 
 ignoring *ConfigurationManager* and therefore ignoring *NConfig*. <br/>
 
 *So, how can I configure it?* <br/>
 There are several ways:
 
 **First:** *If you just need to split log4net configuration from App.config.*
 
Use `log4net.Config` setting in appSettings:
	
```
        <appSettings>
            <add key="log4net.Config" value="Config\separate-log4net.config"/>
        </appSettings>
```

Or use `[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"Config\separate-log4net.config")]` attribute. <br/>
Or specify configuration file programmatically
	
```csahrp	    
        var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"\Config\separate-log4net.config");
        log4net.Config.XmlConfigurator.Configure(new FileInfo(fullPath));
```
More details [here] (http://logging.apache.org/log4net/release/manual/configuration.html)

 **Second:** *If you need machine/host dependent log4net configurations.*

One of the issue there is that usually log4net initialized earlier than NConfig, because of static field initialization.<br/>
`private static log4net.ILog log = log4net.LogManager.GetLogger(...);`
<br/>

One way is to use `log4net.Config` setting with *current config file path* in your custom configuration file, <br/>
and delay logger setup till NConfig initialized.

File *Config\Custom.config*

```
      <?xml version="1.0"?>
      <configuration>
        <configSections>
            <section name="log4net" type="System.Configuration.IgnoreSectionHandler" />
        </configSections>

        <appSettings>
            <!-- This will override any previous log4net.Config settings in app.config and 
                 forces log4net to use this file as configuration. -->
            <add key="log4net.Config" value="Config\Custom.config"/>
        </appSettings>
        
        <log4net>
            <!-- log4net configuration here -->
            ...
        </log4net>
        
     </configuration>
```

Program code:

```csharp
   private static log4net.ILog log;

   static void Main() {
   
	// Setup NConfigurator to use Custom.config file from Config subfolder.
	NConfigurator.UsingFile(@"Config\Custom.config").SetAsSystemDefault();
	
	//Delayed log setup, this will configure log4net using current appSettings from Custom.config.
	log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	...
```

Another way is to use explicit `log4net.Config.XmlConfigurator.Configure();` call after *NConfig* initialization and
declare `log4net` section as `Log4NetConfigurationSectionHandler`.
This will allow us not to specify any additional configuration file paths in appSettings and will provide smooth NConfig experience.

File *Config\Custom.config*

```
      <?xml version="1.0"?>
      <configuration>
        <configSections>
            <!-- NOTE that we use Log4NetConfigurationSectionHandler to make log4net use ConfigurationManager -->
            <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
        </configSections>
        
        <log4net>
            <!-- log4net configuration here -->
            ...
        </log4net>
        
     </configuration>
```
Program code:

```csharp
   private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

   static void Main() {
   
	// Setup NConfigurator to use Custom.config file from Config subfolder.
	NConfigurator.UsingFile(@"Config\Custom.config").SetAsSystemDefault();
	
	// Configure log4net using host-based Custom.config configuration file.
	log4net.Config.XmlConfigurator.Configure();
	...
```


Feel free to contact me if you have any other questions related to NConfig integrations.

### Debug

You can review how NConfig setup by calling `NConfigurator.Default.DumpDiagnostics();`

The output will be in a form of string that allows you to review 
what configuration files NConfig is looking for and what files it used to construct the final configuration.

```
NConfig Diagnostics 
Host name: Workstation
Host alias: Tests
Web Environment: False
Configuration files: 
missing file: 'Tests.NotExisting.config' location: 'C:\NConfig\NConfig.Tests\bin\Debug\Tests.NotExisting.config' 
missing file: 'NotExisting.config' location: 'C:\NConfig\NConfig.Tests\bin\Debug\NotExisting.config' 
exists file: 'Configs\Tests.Aliased.config' location: 'C:\NConfig\NConfig.Tests\bin\Debug\Configs\Tests.Aliased.config' 
exists file: 'Configs\Aliased.config' location: 'C:\NConfig\NConfig.Tests\bin\Debug\Configs\Aliased.config' 
```

### Issues
Currently NConfig was tested against ASP.Net, ASP.Net MVC, WinServices, WinForms/WPF, Console apps.
Other environments are supposed to be supported too, but I haven't tested them yet.

If you do everything as described before, but no custom configuration applied, 
please check that settings <br/> 
`Copy To Output Directory = Copy Always` <br/>
for all your configuration files.

If you encounter any issues please do not hesitate to contact me directly or by raising issue on GitHub.

### Thanks
I would like to thank my colleagues for helping me testing and fixing this tool. Also thanks to TLK for his help in this file creation.
Also I would like to thank the guys at 31337 chat for their support.
