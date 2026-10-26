using Microsoft.OpenApi;
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
            if (doc.Paths == null)
            {
                return;
            }

            foreach (var path in doc.Paths.Values)
            {
                if (path.Operations == null)
                {
                    continue;
                }

                foreach (var op in path.Operations.Values)
                {
                    var content = op.RequestBody?.Content;
                    if (content == null)
                    {
                        continue;
                    }

                    foreach (var key in content.Keys.ToList())
                        if (key.Contains("json") && key != "application/json"
                            && !content.ContainsKey("application/json"))
                            content["application/json"] = content[key];
                }
            }
        }
    }
}
