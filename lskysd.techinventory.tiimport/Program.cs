using lskysd.techinventory.importers;
using lskysd.techinventory.util;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace lskysd.techinventory.tiimport
{
    class Program
    {
        private static void ConsoleWrite(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm K") + ": " + message);
        }

        static void Main(string[] args)
        {
            // Configure with Azure Key Vault
            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile($"appsettings.json", true, true)
               .AddEnvironmentVariables()
               .Build();

            string keyvault_endpoint = configuration["KEYVAULT_ENDPOINT"];
            if (!string.IsNullOrEmpty(keyvault_endpoint))
            {
                ConsoleWrite("Loading configuration from Azure Key Vault: " + keyvault_endpoint);
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                                new KeyVaultClient.AuthenticationCallback(
                                    azureServiceTokenProvider.KeyVaultTokenCallback));

                configuration = new ConfigurationBuilder()
                    .AddConfiguration(configuration)
                    .AddAzureKeyVault(keyvault_endpoint, keyVaultClient, new DefaultKeyVaultSecretManager())
                    .Build();
            }

            string dbConnectionString = configuration.GetConnectionString(ImporterSettings.ConnectionStringName) ?? string.Empty;

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                ConsoleWrite("ConnectionString can't be empty");
            }
            else
            { 
                // Parse options
                // Source (Meraki, Google, Azure, etc)
                // Facility

                // Attempt to import a CSV
                string fileName = "google.csv";
                Facility facility = new Facility()
                {
                    Id = 15,
                    Name = "Macklin"
                };

                GoogleCSVImporter importer = new GoogleCSVImporter(dbConnectionString);

                using (StreamReader streamReader = new StreamReader(fileName))
                {
                    importer.Import(streamReader);
                }
            }
           


        }
    }
}
