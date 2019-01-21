// // Program.cs
// /*
// Paul Schneider paul@pschneider.fr 18/01/2019 20:04 20192019 1 18
// */
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Resources.Tools;

namespace strongresbuildercli
{
    class MainClass
    {
        static int verbosity = 0;
        const string _designer_cs_ext = ".Designer.cs";
        enum LogLevel : int
        {
            None = 0,
            Error,
            Warning,
            Info,
            Verbose,
            Trace
        };

        public static void Main(string[] args)
        {
            bool show_help = false;
            string defaultNameSpace = null;
            bool declare_internal = true;
            bool targetsPcl = false;

            var p = new OptionSet() {
            { "n|default-namespace=", "the name default namespace ",
              v => defaultNameSpace = v },
            { "p|public", "Generate a public class", v=> declare_internal = false },
                {"t|target-pcl","Target PCL" , v=> targetsPcl =true },
            { "v", "increase debug message verbosity",
              v => { if (v != null) ++verbosity; } },
            { "h|help",  "show this message and exit",
              v => show_help = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("strongresbuildercli: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `strongresbuildercli --help' for more information.");
                return;
            }


            if (extra.Count == 0) show_help = true;
            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            foreach (var fileName in extra)
            {
                FileInfo fi = new FileInfo(fileName);
                if (fi.Exists)
                {
                    if (fi.Extension==".resx")
                    {
                        // try an get namespace from the file name.
                        var names = fi.Name.Split('.');
                        var len = names.Length;
                        if (len >= 2) {

                            var ns = string.Join(".", names.Take(len - 2));
                            if (string.IsNullOrEmpty(ns))
                                ns = defaultNameSpace;
                            var className = names[len - 2];
                            var outputFileName = className + _designer_cs_ext;
                            var fo = Path.Combine(fi.DirectoryName, outputFileName);
    
                            generateResxCode(fi.FullName, ns, className, fo, declare_internal, targetsPcl) ;
                        }
                        else
                        {
                            Trace("Bad resource file name : " + fileName);
                        }
                    }
                    else
                    {
                        Trace($"Exentions is not '.rex' ({fileName})");
                    }

                }
                else
                {
                    Error($"File not found: {fileName}");
                }
            }
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine
            ("Usage: strongresbuildercli [OPTIONS]* [RESXFILE]+ ");
            Console.WriteLine
            ("Generates strongly typed ressource " +
                "names for given ressource files.");
            Console.WriteLine(
            "Resource files must be named using the '.resx' extension.");
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
        static void Error(string format, params object[] args)
        {
            Log((int) LogLevel.Error, format, args);
        }
        static void Trace(string format, params object[] args)
        {
            Log((int) LogLevel.Trace, format, args);
        }

        static void Warn(string format, params object[] args)
        {
            Log((int)LogLevel.Warning, format, args);
        }

        static void Log(int level, string format, params object[] args)
        {
            if (verbosity >= level)
            {
                Console.Error.Write("# ");
                Console.Error.WriteLine(format, args);
            }
        }

        /// <summary>
        /// Generates the RESX code.
        /// </summary>
        /// <param name="resXFileName">Res XF ile name.</param>
        /// <param name="nameSpace">Name space.</param>
        /// <param name="className">Class name.</param>
        /// <param name="generatedFileName">Generated file name.</param>
        /// <param name="intern">If set to <c>true</c> intern.</param>
        public static void generateResxCode(string resXFileName,
         string nameSpace, string className, string generatedFileName, bool intern = true, bool targetsPcl = false)
        {
            StreamWriter sw = new StreamWriter(generatedFileName);
            string[] errors = null;
            Dictionary<string, string> provOptions =
            new Dictionary<string, string>();

            provOptions.Add("CompilerVersion", "v12.0");
            // Get the provider for Microsoft.CSharp
            CSharpCodeProvider csProvider = new CSharpCodeProvider(provOptions);


            CodeCompileUnit code = StronglyTypedResourceBuilder.Create(resXFileName, className,
                                                                       nameSpace, csProvider,
                                                                       intern, out errors);
            if (targetsPcl)
                FixupPclTypeInfo(code);

            if (errors.Length > 0)
                foreach (var error in errors)
                    Console.Error.WriteLine(error);
            var options = new CodeGeneratorOptions();
           
            csProvider.GenerateCodeFromCompileUnit(code, sw, options);
            sw.Close();
            Trace($"Generated: {generatedFileName}");
        }

        static CodeObjectCreateExpression GetInitExpr(CodeCompileUnit ccu)
        {
            ccu.Namespaces[0].Imports.Add(new CodeNamespaceImport("System.Reflection"));
            var assignment = ccu.Namespaces[0].Types[0]
                                .Members.OfType<CodeMemberProperty>().Single(t => t.Name == "ResourceManager")
                                .GetStatements.OfType<CodeConditionStatement>().Single()
                                .TrueStatements.OfType<CodeVariableDeclarationStatement>().Single();
            var initExpr = (CodeObjectCreateExpression)assignment.InitExpression;
            return initExpr;
        }

        static void FixupPclTypeInfo(CodeCompileUnit ccu)
        {
            try
            {
                CodeObjectCreateExpression initExpr = GetInitExpr(ccu);
                var typeofExpr = (CodePropertyReferenceExpression)initExpr.Parameters[1];
                typeofExpr.TargetObject = new CodeMethodInvokeExpression(typeofExpr.TargetObject, "GetTypeInfo");
            }
            catch (Exception ex)
            {
                Warn("Failed to fixup StronglyTypedResourceBuilder output for PCL\n{0}", ex);
            }
        }

    }
}
