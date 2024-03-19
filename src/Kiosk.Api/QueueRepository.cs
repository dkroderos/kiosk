using Kiosk.Shared;
using Microsoft.Data.Sqlite;

namespace Kiosk.Api;

public class QueueRepository(string connectionString)
{
    public async Task<IEnumerable<QueueDto>> GetAllAsync()
    {
        var queues = new List<QueueDto>();
        const string tableName = "Queues";

        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        const string query =
            $"SELECT Id, FullName, UserName, Email, GradeAndSection, Purpose, Card, Forms, Others, DateRegistered FROM {tableName}";
        await using var command = new SqliteCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var queue = new QueueDto(
                reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3),
                reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7),
                reader.GetString(8), reader.GetString(9)
            );

            queues.Add(queue);
        }

        return queues;
    }

    public async Task<QueueDto?> GetByIdAsync(int id)
    {
        const string tableName = "Queues";
        const string query =
            $"SELECT Id, FullName, UserName, Email, GradeAndSection, Purpose, Card, Forms, Others, DateRegistered " +
            $"FROM {tableName} WHERE Id = @Id";

        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new QueueDto(
                reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3),
                reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7),
                reader.GetString(8), reader.GetString(9)
            );
        }

        return null;
    }

    public async Task<QueueDto?> CreateAsync(CreateQueueDto createQueueDto)
    {
        const string tableName = "Queues";
        const string query = $"INSERT INTO {tableName} (FullName, UserName, Email, GradeAndSection, Purpose, Card, Forms, Others, DateRegistered) " +
                             "VALUES (@FullName, @UserName, @Email, @GradeAndSection, @Purpose, @Card, @Forms, @Others, @DateRegistered);" +
                             "SELECT last_insert_rowid();";

        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        
        await using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@FullName", createQueueDto.FullName);
        command.Parameters.AddWithValue("@UserName", createQueueDto.UserName);
        command.Parameters.AddWithValue("@Email", createQueueDto.Email);
        command.Parameters.AddWithValue("@GradeAndSection", createQueueDto.GradeAndSection);
        command.Parameters.AddWithValue("@Purpose", createQueueDto.Purpose);
        command.Parameters.AddWithValue("@Card", createQueueDto.Card);
        command.Parameters.AddWithValue("@Forms", createQueueDto.Forms);
        command.Parameters.AddWithValue("@Others", createQueueDto.Others);
        command.Parameters.AddWithValue("@DateRegistered", DateTimeOffset.Now.ToString("o"));

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        
        var queue = await GetByIdAsync(id);

        return queue;
    }
    
    public async Task<bool> DeleteByIdAsync(int id)
    {
        const string tableName = "Queues";
        const string query = $"DELETE FROM {tableName} WHERE Id = @Id";

        try
        {
            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting record with ID {id}: {ex.Message}");
            return false;
        }
    }
}