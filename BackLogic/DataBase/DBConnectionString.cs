using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackLogic.Model;

namespace BackLogic.DataBase
{
    public  class DbConnectionString
    {
       
        public void Db_Init(ServerDataViewModel data)
        {
            string connString;
            const string providerName = "MySql.Data.MySqlClient";
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.Password = data.Password;
            builder.UserID = data.UserName;
            builder.InitialCatalog = data.NameDatabase;
            builder.DataSource = data.ServerAdress;

            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = providerName;
            entityBuilder.Metadata = "res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl";
            entityBuilder.ProviderConnectionString = builder.ToString();
            connString = entityBuilder.ConnectionString;
            SqlConnection conn = new SqlConnection(builder.ConnectionString);

            conn.Open();
        }
    }
}
