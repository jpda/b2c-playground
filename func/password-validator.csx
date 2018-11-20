using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

public static IActionResult Run(HttpRequest req, ILogger log)
{
    foreach(var a in req.Query) {
        log.LogInformation($"{a}");
    }

    if(!req.Query.ContainsKey("newPassword") || !req.Query.ContainsKey("email")) {
        return new BadRequestObjectResult("please send me good data");
    }

    string password = req.Query["newPassword"];
    string username = req.Query["email"];
    var valid = !password.Contains(username, StringComparison.OrdinalIgnoreCase);
    if(valid){
        return new OkResult();
    }
    return new ConflictObjectResult(new {
        version = "1.0.0",
        status = 409,
        userMessage = "Your password cannot contain your username"
    });
}