using System.Data.SqlClient;

namespace ContactesPOO
{
    public class Database
    {
        string connectionString = "Server=ANTHONY\\SQLEXPRESS; Database=ContactesDB; Integrated Security=True;";

        public SqlConnection GetConnection()
        {
            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }

        public void InsertContact(Contact c)
        {
            SqlConnection con = GetConnection();
            con.Open();

            string query = "INSERT INTO Contactos (Name, Phone, Email, Address) VALUES (@Name, @Phone, @Email, @Address)";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@Name", c.Name);
            cmd.Parameters.AddWithValue("@Phone", c.Phone);
            cmd.Parameters.AddWithValue("@Email", c.Email);
            cmd.Parameters.AddWithValue("@Address", c.Address);

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public SqlDataReader GetContacts()
        {
            SqlConnection con = GetConnection();
            con.Open();

            string query = "SELECT * FROM Contactos";
            SqlCommand cmd = new SqlCommand(query, con);

            return cmd.ExecuteReader();
        }

        public void UpdateContact(Contact c)
        {
            SqlConnection con = GetConnection();
            con.Open();

            string query = "UPDATE Contactos SET Name=@Name, Phone=@Phone, Email=@Email, Address=@Address WHERE Id=@Id";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@Name", c.Name);
            cmd.Parameters.AddWithValue("@Phone", c.Phone);
            cmd.Parameters.AddWithValue("@Email", c.Email);
            cmd.Parameters.AddWithValue("@Address", c.Address);
            cmd.Parameters.AddWithValue("@Id", c.Id);

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void DeleteContact(int id)
        {
            SqlConnection con = GetConnection();
            con.Open();

            string query = "DELETE FROM Contactos WHERE Id=@Id";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();

            con.Close();
        }
    }
}
