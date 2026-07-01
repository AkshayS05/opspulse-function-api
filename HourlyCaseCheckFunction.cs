using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace FunctionApp2
{
    public class HourlyCaseCheckFunction
    {

        private readonly ILogger<HourlyCaseCheckFunction> _logger;

        public HourlyCaseCheckFunction(ILogger<HourlyCaseCheckFunction> logger)
        {
            _logger = logger;
        }
        [Function("HourlyCaseCheck")]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo timer)
        {



            _logger.LogInformation("HourlyCaseCheck timer function run at: {Time}", DateTime.UtcNow);
            //get the connection string
            var connectionString = Environment.GetEnvironmentVariable("SHIPCORE_CONNECTION_STRING");
            //make a connection with the sql
            using (var connection = new SqlConnection(connectionString))
            {
                //once we have the conneciton, run your sql command
                await connection.OpenAsync();
                var statusFilter = "Open";
                var query = "SELECT COUNT(*) FROM cases WHERE status =@status";
                //now get the result
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@status", statusFilter)
; var openCasesCount = (int)await command.ExecuteScalarAsync();
                    _logger.LogInformation("Open Cases count:{Count}", openCasesCount);
                }
            }
        }


    }
}


