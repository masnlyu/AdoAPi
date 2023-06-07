using System.Data;
using MySqlConnector;

public class SqlFunction
{
    private IConfiguration configuration { get; }
    private string connectionString { get; }
    public SqlFunction(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.connectionString = this.configuration.GetConnectionString("MariaDB");
    }
    /// <summary>執行SQL查詢語法
    /// </summary>
    /// <param name="query">查詢語法</param>
    /// <param name="parameters">語法參數</param>
    /// <returns>查詢結果</returns>
    public DataTable StartQuery(string query, MySqlParameter[] parameters)
    {
        DataTable dataTable = new DataTable();
        MySqlCommand command = new MySqlCommand();
        MySqlConnection connection = new MySqlConnection(this.connectionString);
        try
        {
            connection.Open();
            command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.Clear();
            if ((parameters != null) && (parameters.Length > 0))
            {
                command.Parameters.AddRange(parameters);
            }
            MySqlDataReader reader = command.ExecuteReader();
            dataTable.Load(reader);
        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            command.Dispose();
            connection.Close();
        }
        return dataTable;
    }

    /// <summary>執行SQL非查詢語法
    /// </summary>
    /// <param name="query">SQL語法</param>
    /// <param name="parameters">SQL的參數</param>
    /// <returns>執行結果</returns>
    public int StartNonQuery(string query,MySqlParameter[] parameters)
    {
        int result = 0;
        MySqlCommand command = new MySqlCommand();
        MySqlConnection connection = new MySqlConnection(this.connectionString);
        try
        {
            connection.Open();
            command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.Clear();
            if ((parameters != null) && (parameters.Length > 0))
            {
                command.Parameters.AddRange(parameters);
            }
            result = command.ExecuteNonQuery();
        }
        catch (System.Exception)
        {
            throw;
        }
        finally
        {
            command.Dispose();
            connection.Close();
        }
        return result;
    }
}