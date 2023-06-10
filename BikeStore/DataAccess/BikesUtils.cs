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
        public Task<IEnumerable<Bike>> GetBikes();
        public Task<Bike> GetBikeById(int id);
        public Task CreateTable();
        public Task ClearTable();
        public Task GenerateData(int quant);
    }

    public class BikesUtils : IBikeUtils
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();

        private string ConnectionString => configuration.GetSection("ConnectionString").Value;

        public bool CheckIfTableExists()
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
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
                var tableExists = CheckIfTableExists();

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
	                        Size varchar(3),
                            CreatedIn datetime2(7))
                        ";

                    using (var con = new SqlConnection(ConnectionString))
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
            var tableExists = CheckIfTableExists();

            if (tableExists)
            {
                string insertQuery =
                    @"INSERT INTO Bikes
                        (Id,Name,Description,Brand,Price,Weight,HasInsurance,Size,CreatedIn)
                    VALUES
                        (@Id,@Name,@Description,@Brand,@Price,@Weight,@HasInsurance,@Size,@CreatedIn)";


                using (var con = new SqlConnection(ConnectionString))
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
                        parameters.Add("CreatedIn", DateTime.Now);

                        con.Execute(insertQuery, param: parameters, commandType: System.Data.CommandType.Text);
                    }
                }
            }
        }

        public async Task<IEnumerable<Bike>> GetBikes()
        {
            var tableExists = CheckIfTableExists();

            if (tableExists)
            {
                IEnumerable<Bike> list;
                string query = @"SELECT * FROM Bikes";

                using (var con = new SqlConnection(ConnectionString))
                {
                    list = await con.QueryAsync<Bike>(query, commandType: System.Data.CommandType.Text);
                }
                return list;
            }
            return null;
        }

        public async Task ClearTable()
        {
            var tableExists = CheckIfTableExists();

            if(tableExists)
            {
                string query = @"DELETE FROM Bikes";

                using (var con = new SqlConnection(ConnectionString))
                {
                    con.Execute(query);
                }
            }
            return;
        }

        public async Task<Bike> GetBikeById(int id)
        {
            var tableExists = CheckIfTableExists();
            var bike = new Bike();
            if (tableExists)
            {
                string query = $"SELECT TOP 1 * FROM Bikes WHERE Id = {id}";
                using (var con = new SqlConnection(ConnectionString))
                {
                    var item = await con.QueryAsync<Bike>(query, commandType: System.Data.CommandType.Text);
                    return item.FirstNonDefault();
                }
            }
            return null;
        }
    }
}