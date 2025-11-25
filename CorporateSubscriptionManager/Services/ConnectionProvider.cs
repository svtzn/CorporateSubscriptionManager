using System;
using System.Configuration;

namespace CorporateSubscriptionManager.Services
{
    public static class ConnectionProvider
    {
        public static string DefaultConnection =>
            ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in App.config.");
    }
}