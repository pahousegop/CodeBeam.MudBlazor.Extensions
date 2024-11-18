using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;

namespace MudExtensions.UnitTests.Mocks
{
#pragma warning disable CS1998
#pragma warning disable CS0067
    public class MockKeyInterceptorServiceFactory : IKeyInterceptorFactory
    {
        private readonly MockKeyInterceptorService? _interceptorService;

        public MockKeyInterceptorServiceFactory(MockKeyInterceptorService interceptorService)
        {
            _interceptorService = interceptorService;
        }

        public MockKeyInterceptorServiceFactory()
        {

        }

        public IKeyInterceptor Create() => _interceptorService ?? new MockKeyInterceptorService();
    }

    public class MockKeyInterceptorService : IKeyInterceptor, IKeyInterceptorService
    {
        public void Dispose()
        {
            
        }

        public Task Connect(string element, KeyInterceptorOptions options)
        {
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            return Task.CompletedTask;
        }

        public Task UpdateKey(KeyOptions option)
        {
            return Task.CompletedTask;
        }

        public event KeyboardEvent? KeyDown;
        public event KeyboardEvent? KeyUp;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public Task SubscribeAsync(IKeyInterceptorObserver observer, KeyInterceptorOptions options) => Task.CompletedTask;

        public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, IKeyDownObserver? keyDown = null, IKeyUpObserver? keyUp = null) => Task.CompletedTask;

        public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Action<KeyboardEventArgs>? keyDown = null, Action<KeyboardEventArgs>? keyUp = null) => Task.CompletedTask;

        public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Func<KeyboardEventArgs, Task>? keyDown = null, Func<KeyboardEventArgs, Task>? keyUp = null) => Task.CompletedTask;

        public Task UpdateKeyAsync(IKeyInterceptorObserver observer, KeyOptions option) => Task.CompletedTask;

        public Task UpdateKeyAsync(string elementId, KeyOptions option) => Task.CompletedTask;

        public Task UnsubscribeAsync(IKeyInterceptorObserver observer) => Task.CompletedTask;

        public Task UnsubscribeAsync(string elementId) => Task.CompletedTask;
    }
}
