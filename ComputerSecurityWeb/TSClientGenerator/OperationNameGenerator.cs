using NSwag;
using NSwag.CodeGeneration.OperationNameGenerators;
using System.Linq;

namespace TSClientGenerator
{
    public class OperationNameGenerator : IOperationNameGenerator
    {
        public bool SupportsMultipleClients { get; } = true;

        public string GetClientName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            return path
                .Split('/')
                .Where(p => !p.Contains("{") && !string.IsNullOrWhiteSpace(p))
                .FirstOrDefault() ?? string.Empty;
        }

        public string GetOperationName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            var action = path
                .Split('/')
                .Where(p => !p.Contains("{") && !string.IsNullOrWhiteSpace(p))
                .LastOrDefault() ?? string.Empty;

            var actionName = char.ToUpper(action[0]) + (action.Length > 1 ? action.Substring(1) : "");

            var method = httpMethod.ToString().ToLower();

            var lastSegment = path
                .Split('/')
                .LastOrDefault() ?? string.Empty;

            string ById = "";
            if (lastSegment.Contains("{") && !string.IsNullOrWhiteSpace(lastSegment))
            {
                ById = "ById";
            }

            return (method + actionName + ById);
        }
    }
}
