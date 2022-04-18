using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Wpf__Test
{
    class DataBase
    {
        SqlConnection SqlConnection = new SqlConnection(@"Data Source=DESKTOP-I33SI4U\SQLEXP;Initial Catalog=Muchkaeva_PaymentDB;Integrated Security=true");
    
        public void OpenConn()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
        }

        public void CloseConn()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Open)
            {
                SqlConnection.Close();
            }
        }

        public SqlConnection GetConn()
        {
            return SqlConnection;
        }

    }
}
