using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Api.Versioning.Swagger
{
    public class PlainJsonBodyFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument doc, DocumentFilterContext ctx)
        {
            foreach (var path in doc.Paths.Values)
                foreach (var op in path.Operations.Values)
                    if (op.RequestBody != null)
                        foreach (var key in op.RequestBody.Content.Keys.ToList())
                            if (key.Contains("json") && key != "application/json"
                                && !op.RequestBody.Content.ContainsKey("application/json"))
                                op.RequestBody.Content["application/json"] = op.RequestBody.Content[key];
        }
    }
}
