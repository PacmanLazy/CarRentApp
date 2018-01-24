using System;
using System.Data;
using System.Data.SqlClient;

namespace CarRentDBApp
{
    public class CarRentalDbWorker
    {
        public int RowsPerPage { get; set; }

        public SqlConnection Connection
        {
            get; set;
        }

        public DataTable Cars { get; set; }
        public DataTable Customers { get; set; }
        public DataTable Rent { get; set; }

        public DataTable TimeRequestTable { get; set; }
        public DateTime StartTimePoint { get; set; }
        public DateTime EndTimePoint { get; set; }

        public DataTable GetCarInfoTable { get; set; }
        public DataTable GetCarModelInfoTable { get; set; }
        public DataTable CarsManufTable { get; set; }
        public DataTable RentDetailsTable { get; set; }

        public CarRentalDbWorker(SqlConnection connection)
        {
            Connection = connection;
            Connection.Open();
            RowsPerPage = 20;

            Cars = GetSchema(Connection, "Cars");
            Cars.TableName = "Автомобили";

            Customers = GetSchema(Connection, "Customers");
            Customers.TableName = "Клиенты";

            Rent = GetSchema(Connection, "Rent");
            Rent.TableName = "Прокат";

            TimeRequestTable = new DataTable();
            GetCarInfoTable = new DataTable();
            GetCarModelInfoTable = new DataTable();
            CarsManufTable = new DataTable();
            RentDetailsTable = new DataTable();
        }

        private DataTable GetSchema(SqlConnection connection, string tableName)
        {
            SqlCommand cmd = new SqlCommand("SelectFrom"+tableName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageNumber", 1);
            cmd.Parameters.AddWithValue("@RowspPage", 1);
            SqlDataReader reader = cmd.ExecuteReader();

            DataTable schema = reader.GetSchemaTable();
            DataTable table = new DataTable();

            foreach (DataRow schemaRow in schema.Rows)
            {
                DataColumn column = new DataColumn((string)schemaRow["ColumnName"]);
                column.AllowDBNull = (bool)schemaRow["AllowDbNull"];
                column.DataType = (Type)schemaRow["DataType"];
                column.Unique = (bool)schemaRow["IsUnique"];
                column.ReadOnly = (bool)schemaRow["IsReadOnly"];
                column.AutoIncrement = (bool)schemaRow["IsIdentity"];

                if (column.DataType == typeof(string))
                    column.MaxLength = (int)schemaRow["ColumnSize"];

                if (column.AutoIncrement == true)
                { column.AutoIncrementStep = -1; column.AutoIncrementSeed = 0; }

                table.Columns.Add(column);
            }

            reader.Close();

            return table;
        }

        private static DataTable LoadWithSchema(string procName, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand(procName, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            DataTable schema = reader.GetSchemaTable();
            DataTable table = new DataTable();

            foreach (DataRow schemaRow in schema.Rows)
            {
                DataColumn column = new DataColumn((string)schemaRow["ColumnName"]);
                column.AllowDBNull = (bool)schemaRow["AllowDbNull"];
                column.DataType = (Type)schemaRow["DataType"];
                column.Unique = (bool)schemaRow["IsUnique"];
                column.ReadOnly = (bool)schemaRow["IsReadOnly"];
                column.AutoIncrement = (bool)schemaRow["IsIdentity"];

                if (column.DataType == typeof(string))
                    column.MaxLength = (int)schemaRow["ColumnSize"];

                if (column.AutoIncrement == true)
                { column.AutoIncrementStep = -1; column.AutoIncrementSeed = 0; }

                table.Columns.Add(column);
            }
            table.Load(reader);
            reader.Close();

            return table;
        }

        public void GetData(SqlDataReader reader, DataTable table)
        {
            table.Load(reader);
            reader.Close();
        }

        public SqlDataReader ExecuteTableCommand(string procedure, int pageNum, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand(procedure, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PageNumber", pageNum);
            cmd.Parameters.AddWithValue("@RowspPage", RowsPerPage);

            SqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }

        public static SqlDataReader ExecuteFormDataCommand(string procedure, int pageNum, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand(procedure, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PageNumber", pageNum);
            cmd.Parameters.AddWithValue("@RowspPage", 1);

            SqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }

        public static SqlDataReader ExecuteFormRentDataCommand(string customerPassData, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand("CustomerRentRequest", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CustPass", customerPassData);
            
            SqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }

        public SqlDataReader ExecuteTimeRequest(int pageNum, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand("RequestTimePeriod", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StartTimePoint", StartTimePoint);
            cmd.Parameters.AddWithValue("@EndTimePoint", EndTimePoint);
            cmd.Parameters.AddWithValue("@PageNumber", pageNum);
            cmd.Parameters.AddWithValue("@RowspPage", RowsPerPage);

            SqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }

        public SqlDataReader ExecuteGetCarRequest(string govNum, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand("GetOneCarInfoRequest", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@GovNum", govNum);

            SqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }

        public static SqlDataReader GetCarInfo(string govNum, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand("GetOneCarInfoRequest", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@GovNum", govNum);

            SqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }

        public static DataTable GetCarsReport(SqlConnection connection)
        {
            string query = "GetCarsReport";
            
            DataTable table = LoadWithSchema(query, connection);

            return table;
        }

        public static DataTable RentCustomerToPrintData(SqlConnection connection, string currentUserKey)
        {
            string query = string.Format("Select * From Customers Where SeriesAndPassNum = '{0}'", currentUserKey);
            
            DataTable table = LoadWithSchema(query, connection);

            return table;
        }

        public static DataTable RentToPrintData(SqlConnection connection)
        {
            string query = "Select * From Rent Where OrderId = IDENT_CURRENT('Rent')";
            
            DataTable table = LoadWithSchema(query, connection);

            return table;
        }

        public static DataTable RentCarToPrintData(SqlConnection connection, string currentCar)
        {
            string query = string.Format("Select * From Cars Where GovNumber = '{0}'", currentCar);
            
            DataTable table = LoadWithSchema(query, connection);

            return table;
        }

        public int TotalTimeRequestRowCount(SqlConnection connection)
        {
            SqlCommand cmdForCount = new SqlCommand("TimeRequestRowCount", connection);
            cmdForCount.CommandType = CommandType.StoredProcedure;

            cmdForCount.Parameters.AddWithValue("@Count", 1);
            cmdForCount.Parameters["@Count"].Direction = ParameterDirection.Output;
            cmdForCount.Parameters.AddWithValue("@StartTimePoint", StartTimePoint);
            cmdForCount.Parameters.AddWithValue("@EndTimePoint", EndTimePoint);

            cmdForCount.ExecuteNonQuery();

            int totalRowCount = (int)cmdForCount.Parameters[0].Value;

            return totalRowCount;
        }

        public int TotalSimpleRequestRowCount(SqlConnection connection, string requestName)
        {
            SqlCommand cmdForCount = new SqlCommand(requestName, connection);
            cmdForCount.CommandType = CommandType.StoredProcedure;

            cmdForCount.Parameters.AddWithValue("@Count", 1);
            cmdForCount.Parameters["@Count"].Direction = ParameterDirection.Output;
            
            cmdForCount.ExecuteNonQuery();

            int totalRowCount = (int)cmdForCount.Parameters[0].Value;

            return totalRowCount;
        }

        public int TotalTableRowCount(SqlConnection connection, string tableName)
        {
            SqlCommand cmdForCount = new SqlCommand("Select @Count = COUNT(*) From " + tableName, connection);
            SqlParameter parameter = new SqlParameter("@Count", SqlDbType.Int);
            parameter.Direction = ParameterDirection.Output;
            cmdForCount.Parameters.Add(parameter);
            cmdForCount.ExecuteNonQuery();

            int totalRowCount = (int)parameter.Value;

            return totalRowCount;
        }

        public static int TotalFormDataRowCount(SqlConnection connection, string tableName)
        {
            SqlCommand cmdForCount = new SqlCommand("Select @Count = COUNT(*) From " + tableName, connection);
            SqlParameter parameter = new SqlParameter("@Count", SqlDbType.Int);
            parameter.Direction = ParameterDirection.Output;
            cmdForCount.Parameters.Add(parameter);
            cmdForCount.ExecuteNonQuery();

            int totalRowCount = (int)parameter.Value;

            return totalRowCount;
        }

        public static void AddNewCar(SqlConnection connection, string govNum, string model, string color, short year, decimal rentPrice, decimal worth)
        {
            SqlCommand cmd = new SqlCommand("AddNewCar", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@GovNumber", govNum);
            cmd.Parameters.AddWithValue("@Model", model);
            cmd.Parameters.AddWithValue("@Color", color);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@RentPrice", rentPrice);
            cmd.Parameters.AddWithValue("@Worth", worth);

            cmd.ExecuteNonQuery();
        }

        public static void AddNewCustomer(SqlConnection connection, string pass, string lName, string fName, string mName)
        {
            SqlCommand cmd = new SqlCommand("AddNewCustomer", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Pass", pass);
            cmd.Parameters.AddWithValue("@LName", lName);
            cmd.Parameters.AddWithValue("@FName", fName);
            cmd.Parameters.AddWithValue("@MName", mName);
            
            cmd.ExecuteNonQuery();
        }

        public static void AddNewRent(SqlConnection connection, byte rentDays, string passData, string govNum)
        {
            SqlCommand cmd = new SqlCommand("AddNewRent", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RentDays", rentDays);
            cmd.Parameters.AddWithValue("@PassData", passData);
            cmd.Parameters.AddWithValue("@GovNum", govNum);
            
            cmd.ExecuteNonQuery();
        }

        public static void EditCar(SqlConnection connection, string oldNum, string newNum, string color, decimal rentPrice, decimal worth)
        {
            SqlCommand cmd = new SqlCommand("EditCar", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@OldGovNum", oldNum);
            cmd.Parameters.AddWithValue("@NewGovNum", newNum);
            cmd.Parameters.AddWithValue("@Color", color);
            cmd.Parameters.AddWithValue("@RentPrice", rentPrice);
            cmd.Parameters.AddWithValue("@Worth", worth);

            cmd.ExecuteNonQuery();
        }

        public static void EditCustomer(SqlConnection connection, string oldPass, string newPass, string lName, string fName, string mName)
        {
            SqlCommand cmd = new SqlCommand("EditCustomer", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@OldPass", oldPass);
            cmd.Parameters.AddWithValue("@NewPass", newPass);
            cmd.Parameters.AddWithValue("@LName", lName);
            cmd.Parameters.AddWithValue("@FName", fName);
            cmd.Parameters.AddWithValue("@MName", mName);

            cmd.ExecuteNonQuery();
        }

        public static void EditRent(SqlConnection connection, int orderId, byte rentDays)
        {
            SqlCommand cmd = new SqlCommand("EditRent", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@RentDays", rentDays);
            
            cmd.ExecuteNonQuery();
        }

        public static void DeleteCar(SqlConnection connection, string govNum)
        {
            SqlCommand cmd = new SqlCommand("DeleteCar", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@GovNum", govNum);
            
            cmd.ExecuteNonQuery();
        }

        public static void DeleteCustomer(SqlConnection connection, string passData)
        {
            SqlCommand cmd = new SqlCommand("DeleteCustomer", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PassData", passData);

            cmd.ExecuteNonQuery();
        }

        public static void DeleteRent(SqlConnection connection, int orderId)
        {
            SqlCommand cmd = new SqlCommand("DeleteRent", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@OrderId", orderId);

            cmd.ExecuteNonQuery();
        }
    }
}
