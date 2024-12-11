using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using CTAI.Models;
using Microsoft.Data.SqlClient;

namespace CTAI.Repositories
{
    public class TransactionRepository


    {
        private readonly string _connectionString;

        public TransactionRepository()
        {
            _connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        }

        public async Task<List<CoffeeTransaction>> GetAllTransactionsAsync() {
            List<CoffeeTransaction> transactions = new List<CoffeeTransaction>();

            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                string query = "SELECT ID, DateTime, CashType, Card, Money, CoffeeName FROM Transactions";

                using (SqlCommand command = new SqlCommand(query, connection)) {
                    try {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                            while (await reader.ReadAsync()) {
                                CoffeeTransaction transaction = new CoffeeTransaction {
                                    ID = reader.GetGuid(reader.GetOrdinal("ID")),
                                    DateTime = reader.GetDateTime(reader.GetOrdinal("DateTime")),
                                    CashType = reader["CashType"] as string,
                                    Card = reader["Card"] as string,
                                    Money = reader.GetDecimal(reader.GetOrdinal("Money")),
                                    CoffeeName = reader["CoffeeName"] as string
                                };
                                transactions.Add(transaction);
                            }
                        }
                    }
                    catch (Exception ex) {
                        throw;
                    }
                }
            }
            return transactions;

        }
        

        public async Task InsertTransactionAsync(CoffeeTransaction transaction)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Transactions (ID, DateTime, CashType, Card, Money, CoffeeName) VALUES (@ID, @DateTime, @CashType, @Card, @Money, @CoffeeName)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", transaction.ID);
                    command.Parameters.AddWithValue("@DateTime", transaction.DateTime);
                    command.Parameters.AddWithValue("@CashType", transaction.CashType);
                    command.Parameters.AddWithValue("@Card", transaction.Card);
                    command.Parameters.AddWithValue("@Money", transaction.Money);
                    command.Parameters.AddWithValue("@CoffeeName", transaction.CoffeeName);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> GetTotalTransactionCountAsync()
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Transactions";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    count = (int)await command.ExecuteScalarAsync();
                }
            }

            return count;
        }


    }

    
}