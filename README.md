# strongresbuilder

Generates strongly typed ressources csharp code, from command line.

## Strongly typed resources code generator

Invokes StronglyTypedResourceBuilder.Create from System.Designer framework assembly,
and use the `GetTypeInfo()` method from `System.Reflection`, when needed, at targeting pcl projects. 

## Usage 

```
# mono strongresbuildercli/bin/Debug/strongresbuildercli.exe
Usage: strongresbuildercli [OPTIONS]* [RESXFILE]+ 
Generates strongly typed ressource names for given ressource files.
Resource files must be named using the '.resx' extension.
Options:
  -n, --default-namespace=VALUE
                             the name default namespace
  -p, --public               Generate a public class
  -t, --target-pcl           Target PCL
  -v                         increase debug message verbosity
  -h, --help                 show this message and exit
```

## note

This code mainly comes from the MSDN example regarding `StronglyTypedResourceBuilder.Create`, 
and the Monodevelop source code,
that hacks the result in case of targeting PCL projects.

