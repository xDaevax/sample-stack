using System.Reactive.Subjects;

namespace SampleStack.Api.Providers
{
    /// <summary>
    /// Type that provides the ability to write to sockets.
    /// </summary>
    public class InMemorySocketDataProvider : ISocketDataProvider
    {
        private readonly BehaviorSubject<string> _socketData;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySocketDataProvider"/> class.
        /// </summary>
        public InMemorySocketDataProvider()
        {
            this._socketData = new BehaviorSubject<string>(string.Empty);
        }

        public BehaviorSubject<string> SocketData => this._socketData; // end property SocketData

        /// <inheritdoc/>
        public event WriteHandler? OnWrite;

        /// <inheritdoc />
        public void Write(object content)
        {
            this.OnWrite?.Invoke(this, content);
        }
    } // end class InMemorySocketDataProvider
} // end namespace
