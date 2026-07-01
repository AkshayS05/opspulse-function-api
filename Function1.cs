using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FunctionApp2;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("cases")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var statusFilter = req.Query["status"].ToString();
        var results = await GetShipCoreData(statusFilter);
        return new OkObjectResult(results);

    }
    private async Task<List<object>> GetShipCoreData(string statusFiler)
    {
        var results = new List<object>();



        string connectionString = Environment.GetEnvironmentVariable("SHIPCORE_CONNECTION_STRING");
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            //3. run a command, sql over that connection
            var query = "SELECT case_id, shipper, status, station FROM cases";

            if (!string.IsNullOrWhiteSpace(statusFiler))
            {
                query += " WHERE status =@status";
            }

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@status", statusFiler);


                using (var reader = await command.ExecuteReaderAsync())
                {
                    //4. read the rowss that comeback and do something
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

    [Function("GetCaseById")]
    public async Task<IActionResult> GetCaseById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "cases/{caseId:int}")] HttpRequest req, int caseId)
    {
        var results = new List<object>();
        string connectionString = Environment.GetEnvironmentVariable("SHIPCORE_CONNECTION_STRING");

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            //write the query
            var query = "SELECT case_id, status, shipper, station FROM cases WHERE case_id=@caseId";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@caseId", caseId);

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
        return new OkObjectResult(results);


    }
}
