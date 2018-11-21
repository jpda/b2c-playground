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

public static async Task<IActionResult> Run(HttpRequest req, CloudTable cloudTable, ILogger log)
{
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    // ONLY LOG THIS INFO DURING DEV WITH FAKE DATA! 
    log.LogInformation($"body: {requestBody}");

    dynamic data = JsonConvert.DeserializeObject(requestBody);
    string user = data.email;
    string password = data.password;

    // query something for that hash - graph, table, etc. using tables here because they're fast, cheap and easy. see other sample for graph
    var previousPassword = await CheckIfHashExistsInHistoryAsync(user, password, cloudTable);
    
    // if hash exists for user, return 409
    if(previousPassword)
    {
        return new ConflictObjectResult(new {
            version = "1.0.0",
            status = 409,
            userMessage = "Use a different password (make sure this message is sufficiently vague in a production scenario)"
        });
    }
    // if not found, let's get out of here - we'll save the hash later in a different step, 
    // after the user object has passed the rest of the validation and is ready to be saved to AAD
    return new OkResult();
}

public static async Task<bool> CheckIfHashExistsInHistoryAsync(string user, string password, CloudTable table)
{
    var q = new TableQuery<UserHashEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, user.ToLowerInvariant()));
    TableContinuationToken token = null;
    var results = new List<UserHashEntity>();
    do
    {
        var querySegment = await table.ExecuteQuerySegmentedAsync(q, token);
        token = querySegment.ContinuationToken;
        results.AddRange(querySegment.Results);
    } while (token != null);

    // this uses the Microsoft.AspNetCore.Identity default password hasher
    var pwhasher = new PasswordHasher<string>();
    foreach (var r in results)
    {
        // user here is largely immaterial, the hash and verify processes don't use the user
        var answer = pwhasher.VerifyHashedPassword(user, r.Hash, password);
        if(answer == PasswordVerificationResult.Success) return true;
    }
    return false;
}

public class UserHashEntity : TableEntity 
{
    public string Hash { get; set; }
    public string Salt { get; set; }
    public DateTime SetTimeUtc { get; set; }
    // ONLY USE Cleartext FOR TEST/DEBUG - NEVER EVER EVER *EVER* STORE AN ACTUAL PASSWORD IN CLEARTEXT
    public string Cleartext { get; set; }
}