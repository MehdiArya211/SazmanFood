using Microsoft.Extensions.Configuration;
using System;

namespace BLL
{
    public class CustomSettings 

    {
        private static CustomSettings _instance = null;
        private static readonly object padlock = new object();
        public readonly string _connectionString = string.Empty;
        public readonly string _authurl = string.Empty;
        public readonly string _smsurl = string.Empty;
        public readonly string _PersonelUrl = string.Empty;
        public readonly string _OrganUrl = string.Empty;
        public readonly string _FajrUrl = string.Empty;
        public readonly string _ProvinceCityUrl = string.Empty;


        public readonly string _scope = string.Empty;
        public readonly string _clientsecret = string.Empty;
        public readonly string _clientid = string.Empty;
        public readonly string _username = string.Empty;
        public readonly string _password = string.Empty;
        //*********************************************************
        public readonly string _Cardurl = string.Empty;

        public readonly string _scopeCardIsar = string.Empty;
        public readonly string _clientsecretCardIsar = string.Empty;
        public readonly string _clientidCardIsra = string.Empty;
        public readonly string _usernameCardIsar = string.Empty;
        public readonly string _passwordCardIsar = string.Empty;
        public readonly string _UrlSarbazMaher_Aja = string.Empty;
        private CustomSettings()
        {
            var configuration = new  ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
                .Build();

            _authurl = configuration.GetValue<string>("ApiResourceBaseUrls:AuthServer");
            _PersonelUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Personel");
            _OrganUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Organ");
            _ProvinceCityUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Province");
            _UrlSarbazMaher_Aja = configuration.GetValue<string>("ApiResourceBaseUrls:apiUrlSarbazMaher_Aja");



            _scope = configuration.GetValue<string>("ApiClientInfo:Scope");
            _clientsecret = configuration.GetValue<string>("ApiClientInfo:ClientSecret");
            _clientid = configuration.GetValue<string>("ApiClientInfo:ClientId");
            _username = configuration.GetValue<string>("ApiClientInfo:UserName");
            _password = configuration.GetValue<string>("ApiClientInfo:Password");
            //************************************************************************************

            _Cardurl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Card");
            _scopeCardIsar = configuration.GetValue<string>("ApiClientCardInfo:Scope");
            _clientsecretCardIsar = configuration.GetValue<string>("ApiClientCardInfo:ClientSecret");
            _clientidCardIsra = configuration.GetValue<string>("ApiClientCardInfo:ClientId");
            _usernameCardIsar = configuration.GetValue<string>("ApiClientCardInfo:UserName");
            _passwordCardIsar = configuration.GetValue<string>("ApiClientCardInfo:Password");

        }

        public static CustomSettings Instance
        {

            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new CustomSettings();
                    }
                    return _instance;
                }
            }
        }

        public string ConnectionString
        {
            get => _connectionString;
        }

        public string ApiSmsUrl
        {
            get => _smsurl;

        }
        public string UrlSarbazMaher_Aja
        {
            get => _UrlSarbazMaher_Aja;

        }

        public string AuthenticationSrverUrl
        {
            get => _authurl;

        }

        public string Scope
        {
            get => _scope;

        }

        public string ClientSecret
        {
            get => _clientsecret;

        }

        public string ClientId
        {
            get => _clientid;

        }

        public string ROPC_UserName
        {
            get => _username;

        }

        public string ROPC_Password
        {
            get => _password;

        }

        //********************************************

        public string ApiCardIsarUrl
        {
            get => _Cardurl;

        }

        public string ScopeCardIsar
        {
            get => _scopeCardIsar;

        }

        public string ClientSecretCardIsar
        {
            get => _clientsecretCardIsar;

        }

        public string ClientIdCardIsar
        {
            get => _clientidCardIsra;

        }

        public string ROPC_UserNameCardIsar
        {
            get => _usernameCardIsar;

        }

        public string ROPC_PasswordCardIsar
        {
            get => _passwordCardIsar;

        }

        public string ApiPersonelUrl
        {
            get => _PersonelUrl;

        }

        public string ApiOrganUrl
        {
            get => _OrganUrl;

        }

        public string ApiProvinceCityUrl
        {
            get => _ProvinceCityUrl;

        }

    }
}
