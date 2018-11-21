using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

public static IActionResult Run(HttpRequest req, ILogger log)
{
    foreach(var a in req.Query) {
        // delete this before actually using - getting user passwords in logs isn't a thing we want to do
        log.LogInformation($"{a}");
    }

    if(!req.Query.ContainsKey("newPassword") || !req.Query.ContainsKey("email")) {
        return new BadRequestObjectResult("please send me good data");
    }

    string password = req.Query["newPassword"];
    string username = req.Query["email"];
    var valid = !password.Contains(username, StringComparison.OrdinalIgnoreCase);
    if(valid) // we don't actually need to send any data back to B2C - by sending 200 OK, we're signalling this is ok. 
    {
        return new OkResult();
    }
    // if the username is in the password, return a 409 Conflict. The userMessage property is what is shown to the user.
    return new ConflictObjectResult(new {
        version = "1.0.0",
        status = 409,
        userMessage = "Your password cannot contain your username."
    });
}