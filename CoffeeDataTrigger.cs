using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CTAI.Repositories;
using CTAI.Models;
using Newtonsoft.Json;

namespace CTAI.Functions
{
    public class CoffeeDataTrigger
    {
        private readonly ILogger<CoffeeDataTrigger> _logger;

        public CoffeeDataTrigger(ILogger<CoffeeDataTrigger> logger)
        {
            _logger = logger;
        }

        [Function("CoffeeDataTrigger")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        [Function("GetTransactions")]
        public async Task<IActionResult> GetTransactions([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route="transactions")] HttpRequest req)
        {
            try
            {
                TransactionRepository repository = new TransactionRepository();
                List<CoffeeTransaction> transactions = await repository.GetAllTransactionsAsync();
                return new OkObjectResult(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching transactions: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Function("GetCoffeeNames")]
        public static async Task<IActionResult> GetCoffeeNames([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "coffeenames")] HttpRequest req)
        {
            List<string> coffeeNames = new List<string>
            {
                "Americano",
                "Americano with Milk",
                "Cocoa",
                "Cortado",
                "Hot Chocolate",
                "Latte"
            };

            return new OkObjectResult(coffeeNames);
        }

        [Function("InsertTransaction")]
        public async Task<IActionResult> InsertTransaction([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "transactions")] HttpRequest req)
        {
            try
            {
                var transaction = await System.Text.Json.JsonSerializer.DeserializeAsync<CoffeeTransaction>(req.Body);

                TransactionRepository repository = new TransactionRepository();
                await repository.InsertTransactionAsync(transaction);

                return new StatusCodeResult(StatusCodes.Status201Created); // 201 Created
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while inserting the transaction: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError); // 500 Internal Server Error
            }
        }


        [Function("GetTotalTransactions")]
        public async Task<IActionResult> GetTotalTransactions([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "transactions/count")] HttpRequest req)
        {
            var repository = new TransactionRepository();

            try
            {
                int count = await repository.GetTotalTransactionCountAsync();
                return new OkObjectResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching transaction count: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
