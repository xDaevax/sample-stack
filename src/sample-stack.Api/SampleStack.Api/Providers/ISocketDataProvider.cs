using System.Reactive.Subjects;

namespace SampleStack.Api.Providers
{
    /// <summary>
    /// Type that exposes behavior for implementations that provide access to write to sockets, abstracting the low level detail away.
    /// </summary>
    public interface ISocketDataProvider
    {
        #region --Methods--

        /// <summary>
        /// Attempts to write the given <paramref name="content"/> to a socket connection.
        /// </summary>
        /// <param name="content">The content to write.</param>
        void Write(object content);

        #endregion

        public BehaviorSubject<string> SocketData { get; }

        #region --Events--

        /// <summary>
        /// Event for listeners to respond to write events.
        /// </summary>
        public event WriteHandler? OnWrite;

        #endregion
    } // end interface ISocketDataProvider

    /// <summary>
    /// Delegate signature for socket writes.
    /// </summary>
    /// <param name="sender">The instance that raised the event.</param>
    /// <param name="content">The content to write to the socket.</param>
    public delegate void WriteHandler(object sender, object content);
} // end namespace
