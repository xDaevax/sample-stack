using SampleStack.Api.Providers;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;

namespace SampleStack.Api.HostedServices
{
    /// <summary>
    /// HostedService that keeps a persistent socket open to the target listener.
    /// </summary>
    public class SocketHostedService : BackgroundService
    {
        #region --Fields--

        private readonly IConfiguration _configuration;
        private readonly ILogger<SocketHostedService> _logger;
        private readonly ISocketDataProvider _socketDataProvider;
        private bool _isDisposed;
        private Socket? _socket;
        private System.Timers.Timer _retryTimer;
        private System.Timers.Timer _socketTimer;
        private int _retryCount;
        private readonly IHostApplicationLifetime _lifetime;

        #endregion

        #region --Constructors--

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketHostedService"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger{TCategory}"/> instance used to write log messages.</param>
        /// <param name="socketDataProvider">The <see cref="ISocketDataProvider" /> instance used to write socket data.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used to load configuration data.</param>
        /// <param name="lifetime">The <see cref="IHostApplicationLifetime"/> instance used to interrupt operations when necessary.</param>
        public SocketHostedService(ILogger<SocketHostedService> logger, ISocketDataProvider socketDataProvider, IConfiguration configuration, IHostApplicationLifetime lifetime) : base()
        {
            this._logger = logger;
            this._socketDataProvider = socketDataProvider;
            this._configuration = configuration;
            this._socket = null;
            this._retryTimer = new System.Timers.Timer(2000);
            this._socketTimer = new System.Timers.Timer(2000);
            this._retryTimer.Elapsed += this.OnRetrySocketConnection;
            this._socketTimer.Elapsed += this.Write;
            this._retryCount = 0;
            this._lifetime = lifetime;
        } // end constructor

        private void OnRetrySocketConnection(object? sender, System.Timers.ElapsedEventArgs e)
        {
            this.AttemptSocketConnection().GetAwaiter().GetResult();
        } // end method OnRetrySocketConnection

        private void Write(object? sender, System.Timers.ElapsedEventArgs e)
        {
            this._socketDataProvider?.Write("/temp");
        }

        #endregion

        #region --Methods--

        /// <summary>
        /// This method performs disposal and cleanup of resources used by the hosted service.  This method is provided so derived services can have their resources
        /// cleaned up in a way consistent with the correct use of the <see cref="IDisposable"/> pattern.
        /// </summary>
        /// <param name="disposing">true if invoked by user code; false if invoked by the finalizer.</param>
        /// <remarks>https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this.SocketDataProvider.OnWrite -= this.OnWrite;
                    this._socketTimer.Elapsed -= this.Write;
                    this._retryTimer.Elapsed -= this.OnRetrySocketConnection;
                    this._socketTimer?.Stop();
                    this._socketTimer?.Dispose();
                    this._socket?.Close();
                    this._socket?.Dispose();
                    this._retryTimer?.Stop();
                    this._retryTimer?.Dispose();
                    base.Dispose();
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this._isDisposed = true;
            }
        } // end method Dispose

        /// <inheritdoc />
        public new void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
        } // end method Dispose

        /// <inheritdoc />
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation($"Stopping {this.GetType().Name} Hosted Service");
            this._socketTimer.Stop();
            this._socket?.Close();

            await base.StopAsync(cancellationToken).ConfigureAwait(false);
        } // end function StopAsync

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.SocketDataProvider.OnWrite += this.OnWrite;
            this._socketTimer.Start();

            await Task.CompletedTask.ConfigureAwait(false);
        } // end method ExecuteAsync

        /// <summary>
        /// Attempts to perform a socket connection to the host.  Used for re-try and initial connection logic.
        /// </summary>
        private async Task<bool> AttemptSocketConnection()
        {
            try
            {
                if (this._retryCount > 5)
                {
                    this._retryTimer?.Stop();
                    this._lifetime.StopApplication();
                    return false;
                }

                var hostAddress = this.Configuration.GetValue<string>(SettingKeys.SocketHostName);
                var portNumber = this.Configuration.GetValue<int>(SettingKeys.SocketPortNumber);

                this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await this._socket.ConnectAsync(hostAddress!, portNumber).ConfigureAwait(false);

                Interlocked.Exchange(ref this._retryCount, 0);
                this._retryTimer?.Stop();

                return true;
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref this._retryCount);
                this.Logger.LogError(ex, $"A connection attempt to the socket failed.  It has done this {this._retryCount} times.");
            }

            return false;
        } // end method AttemptSocketConnection

        /// <summary>
        /// Handles socket provider write events.
        /// </summary>
        /// <param name="sender">The instance that raised the event.</param>
        /// <param name="content">The content to send on the socket.</param>
        protected void OnWrite(object sender, object content)
        {
            this.AttemptSocketConnection().GetAwaiter().GetResult();
            var length = this._socket?.Send(Encoding.UTF8.GetBytes($"{content}"));
            var bytes = new byte[1024];
            var receivedLength = this._socket?.Receive(bytes);
            this._socketDataProvider.SocketData.OnNext(Encoding.UTF8.GetString(bytes,0, receivedLength ?? 0));
            this._socket.Close();
        } // end method OnWrite

        #endregion

        #region --Properties--

        /// <summary>
        /// Gets the injected <see cref="IConfiguration"/> instance used to load configuration data.
        /// </summary>
        protected IConfiguration Configuration => this._configuration; // end property Configuration

        /// <summary>
        /// Gets the injected <see cref="ISocketDataProvider"/> instance used to write socket data.
        /// </summary>
        protected ISocketDataProvider SocketDataProvider => this._socketDataProvider; // end property SocketDataProvider

        /// <summary>
        /// Gets the injected <see cref="ILogger{TCategoryName}"/> instance used to write log messages.
        /// </summary>
        protected ILogger<SocketHostedService> Logger => this._logger; // end property Logger

        #endregion
    } // end class SocketHostedService
} // end namespace
