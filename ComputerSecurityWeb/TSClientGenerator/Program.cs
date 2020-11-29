using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NJsonSchema;
using NSwag;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.TypeScript;
using NSwag.SwaggerGeneration.AspNetCore;
using NSwag.SwaggerGeneration.WebApi;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TSClientGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                GenerateTypeScriptClients(GenerateSwaggerDoc(args[1], args[2]), args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught: {e.Message}");
                Console.WriteLine($"Exception stacktrace: {e.StackTrace}");

            }
            finally
            {
                Console.ReadLine();
            }
        }

        private static SwaggerDocument GenerateSwaggerDoc(string dllPath, string baseController)
        {
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                IsAspNetCore = true,
                DefaultPropertyNameHandling = PropertyNameHandling.CamelCase
            });

            Console.WriteLine($"Getting types from path: {dllPath}; basecontroller: {baseController}");
            var assembly = Assembly.LoadFrom(dllPath);
            var baseClass = assembly.GetType(baseController);
            var types = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(baseClass))
                .ToArray();

            Console.WriteLine($"generating swagger from types: {string.Join(",", types.Select(t => t.Name))}");

            var doc = generator.GenerateForControllersAsync(types).Result;

            Console.WriteLine("Done generating swagger");
            return doc;
        }

        private static void GenerateTypeScriptClients(SwaggerDocument swaggerSpecification, string path)
        {
            Console.WriteLine("Start generating ts client");
            var settings = new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateOptionalParameters = false,
                HttpClass = HttpClass.HttpClient,
                InjectionTokenType = InjectionTokenType.InjectionToken,
                OperationNameGenerator = new MultipleClientsFromOperationIdWithResourceNameGenerator(),
                RxJsVersion = 6,
                UseSingletonProvider = true,
                UseTransformOptionsMethod = false,
                UseTransformResultMethod = false,
                WrapResponses = false
            };


            var generator = new SwaggerToTypeScriptClientGenerator(swaggerSpecification, settings);

            var code = generator.GenerateFile();
            Console.WriteLine("Done generating ts client");
            path = Path.Combine(Directory.GetParent(path).Parent.FullName, "frontend/computer-security-client/src/app/shared/clients/index.ts");
            if (File.Exists(path))
            {
                Console.WriteLine("Deleted old file");
                File.Delete(path);
            }
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            File.WriteAllText(path, code);
            Console.WriteLine($"Done writing new file. path: {path}");
        }
    }
}
