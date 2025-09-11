using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateSelectionService.WebApi.HeaderParameterSwagger
{
    public class AddRefreshTokenHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Refresh-Token",
                In = ParameterLocation.Header,
                Description = "Refresh token",
                Required = false,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
