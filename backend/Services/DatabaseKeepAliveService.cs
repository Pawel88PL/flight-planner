using Microsoft.Data.SqlClient;
using Serilog;

namespace backend.Services
{
    public class DatabaseKeepAliveService : IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseKeepAliveService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ScheduleNextRun();
            return Task.CompletedTask;
        }

        private void ScheduleNextRun()
        {
            var timeToNextRun = TimeSpan.FromHours(1);
            _timer = new Timer(async state => await SendKeepAliveQuery(state), null, timeToNextRun, Timeout.InfiniteTimeSpan);
            Log.Information("Zadanie 'keep alive database' zaplanowane za {TimeToNextRun}", timeToNextRun);
        }

        private async Task SendKeepAliveQuery(object? state)
        {
            try
            {
                Log.Information("Wykonywanie zapytania keep-alive do bazy danych...");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var connectionString = _configuration.GetConnectionString("azure");
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new SqlCommand("SELECT 1", connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                Log.Information("Zapytanie 'keep alive database' wykonane pomyślnie.");
            }
            catch (Exception ex)
            {
                Log.Error($"Wystąpił błąd podczas wykonywania zapytania 'keep alive database': {ex.Message}");
            }
            finally
            {
                ScheduleNextRun();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}