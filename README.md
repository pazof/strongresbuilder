# Strongly typed resources code generator

Invokes StronglyTypedResourceBuilder.Create from System.Designer framework assembly

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
  -v                         increase debug message verbosity
  -h, --help                 show this message and exit
```
