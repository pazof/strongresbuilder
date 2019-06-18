# strongresbuilder

Generates strongly typed ressources csharp code, from command line.

## Strongly typed resources code generator

Invokes StronglyTypedResourceBuilder.Create from System.Designer framework assembly,
and use the `GetTypeInfo()` method from `System.Reflection`, when needed, at targeting pcl projects. 

## Usage 

```shlog
# mono strongresbuildercli/bin/Debug/strongresbuildercli.exe
:!strotygen
Usage: strongresbuildercli [OPTIONS]* [RESXFILE]+
Generates strongly typed ressource names for given ressource files.
Resource files must be named using the '.resx' extension.
Options:
  -n, --default-namespace=VALUE
                             the name default namespace
  -p, --public               Generate a public class
  -t, --target-pcl           Target PCL
  -l, --gen-parcial          Generate a partial class
  -r, --resource-filename-prefix=VALUE
                             Prefix resource embeded file name using
given value
  -v                         increase debug message verbosity
  -h, --help                 show this message and exit

```

## note

This code mainly comes from the MSDN example regarding `StronglyTypedResourceBuilder.Create`, 
and the Monodevelop source code.

That hacks the result in case of targeting PCL projects, thanks to MonoDevelop people.

That also permit my private usage concerning my Dnx project resource files.

