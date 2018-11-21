#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"
#r "Microsoft.Extensions.Identity.Core"
#r "Microsoft.Extensions.Options"
#r "Microsoft.AspNetCore.Identity"

using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log, CloudTable cloudTable)
{
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    string user = data.email;
    string password = data.password;
    await SaveHash(user, password, cloudTable);
    return new OkResult();
}

public static async Task SaveHash(string user, string password, CloudTable table)
{
    var hasher = new PasswordHasher<string>();
    var u = new UserHashEntity()
    {
        PartitionKey = user.ToLowerInvariant(),
        RowKey = DateTime.UtcNow.ToString("O"),
        Hash = hasher.HashPassword(user, password),
        Cleartext = password,
        SetTimeUtc = DateTime.UtcNow
    };
    var op = TableOperation.Insert(u);
    var result = await table.ExecuteAsync(op);
    if (result.HttpStatusCode < 299) return;
    Console.WriteLine(result.HttpStatusCode);
}

// we'd also ideally share this across via shared CSX, or, more realistically, by doing this in a precompiled function
public class UserHashEntity : TableEntity 
{
    public string Hash { get; set; }
    public string Salt { get; set; }
    public DateTime SetTimeUtc { get; set; }
    // ONLY USE Cleartext FOR TEST/DEBUG - NEVER EVER EVER *EVER* STORE AN ACTUAL PASSWORD IN CLEARTEXT
    public string Cleartext { get; set; }
}