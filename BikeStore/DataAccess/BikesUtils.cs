using BikeStore.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ServiceStack;
using System.Collections.Generic;

namespace BikeStore.Dal
{
    public interface IBikeUtils
    {
        public Task CreateTable();
        public Task GenerateData(int quant);
        public Task<IEnumerable<Bike>> GetBikes();
    }

    public class BikesUtils : IBikeUtils
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();

        public bool CheckIfTableExists(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    int existsTable = conn.ExecuteScalar<int>($"SELECT COUNT(*) FROM Bikes");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }

        public async Task CreateTable()
        {
            try
            {
                string connectionString = configuration.GetSection("ConnectionString").Value;
                var tableExists = CheckIfTableExists(connectionString);

                if (!tableExists)
                {
                    var createQuery =
                        @"CREATE TABLE Bikes(
	                        Id int,
	                        Name varchar(255),
	                        Description varchar(1000),
	                        Brand varchar(100),
	                        Price numeric(10,2),
	                        Weight numeric(10,2),
	                        HasInsurance bit,
	                        Size varchar(3))
                        ";

                    using (var con = new SqlConnection(connectionString))
                    {
                        con.Execute(createQuery);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return;
        }

        public async Task GenerateData(int quant)
        {
            string connectionString = configuration.GetSection("ConnectionString").Value;
            var tableExists = CheckIfTableExists(connectionString);

            if (tableExists)
            {
                string insertQuery =
                    @"INSERT INTO Bikes
                        (Id,Name,Description,Brand,Price,Weight,HasInsurance,Size)
                    VALUES
                        (@Id,@Name,@Description,@Brand,@Price,@Weight,@HasInsurance,@Size)";


                using (var con = new SqlConnection(connectionString))
                {
                    DynamicParameters parameters;

                    for (int i = 1; i <= quant; i++)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("Id", i);

                        Random random = new Random();
                        parameters.Add("Description", $"Bike {i}");
                        parameters.Add("Name", Enum.GetName(typeof(Names), random.Next(19)));
                        parameters.Add("Brand", Enum.GetName(typeof(Brands), random.Next(11)));
                        parameters.Add("Price", Math.Round(random.NextDouble() * 10000, 2));
                        parameters.Add("Weight", Math.Round(random.Next(10, 20) + random.NextDouble(), 2));
                        parameters.Add("HasInsurance", random.NextDouble() >= 0.5);
                        parameters.Add("Size", Enum.GetName(typeof(Sizes), random.Next(4)));

                        con.Execute(insertQuery, param: parameters, commandType: System.Data.CommandType.Text);
                    }
                }
            }
        }

        public async Task<IEnumerable<Bike>> GetBikes()
        {
            string connectionString = configuration.GetSection("ConnectionString").Value;
            var tableExists = CheckIfTableExists(connectionString);

            if (tableExists)
            {
                IEnumerable<Bike> list;
                string query = @"SELECT * FROM Bikes";

                using (var con = new SqlConnection(connectionString))
                {
                    list = await con.QueryAsync<Bike>(query, commandType: System.Data.CommandType.Text);
                }
                return list;
            }
            return null;
        }
    }
}