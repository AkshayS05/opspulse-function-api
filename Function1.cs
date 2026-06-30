using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace OpsPulseFunction;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        //return new OkObjectResult("Welcome to Azure Functions!");
        var results = await GetCasesFromShipCore();
        return new OkObjectResult(results);

    }

    private async Task<List<object>> GetCasesFromShipCore()
    {
        var results = new List<object>();
        //1. get the connection string from config (by its name)
        string connectionString = Environment.GetEnvironmentVariable("ShipCoreConnection");
        //2 open a connection (the "using" auto-closes it when it is done)

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            //3. define the sql command to run
            var query = "SELECT case_id, shipper, status, station FROM cases";
            using (var command = new SqlCommand(query, connection))
            {
                // 4. Read the rows


                    using (var reader = await command.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            results.Add(new
                            {
                                CaseId = reader["case_id"],
                                Shipper = reader["shipper"],
                                Status = reader["status"],
                                Station = reader["station"]
                            });

                        }
                    }
            }
        }
        return results;
    }
}
