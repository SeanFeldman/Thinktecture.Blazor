using Microsoft.JSInterop;

namespace Thinktecture.Blazor.ViewTransition
{
    public interface IViewTransitionService
    {
        Task<bool> IsSupportedAsync(CancellationToken cancellationToken);
        Task StartViewTransitionAsync(Task completionTask, CancellationToken cancellationToken);
    }

    public class ViewTransitionService : IViewTransitionService
    {
        private Lazy<ValueTask<IJSInProcessObjectReference>> _moduleTask;
        private TaskCompletionSource _oldViewStateCompleted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private Task? _beforeTransition;

        public ViewTransitionService(IJSRuntime jSRuntime)
        {
            _moduleTask = new(() => jSRuntime.InvokeAsync<IJSInProcessObjectReference>(
                "import", "./_content/Thinktecture.Blazor.ViewTransition/Thinktecture.Blazor.ViewTransition.js"));
        }

        public async Task<bool> IsSupportedAsync(CancellationToken cancellationToken = default)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<bool>("isSupported", cancellationToken);
        }

        /// <summary>
        /// Start view transition with the help of the View Transition API (https://drafts.csswg.org/css-view-transitions-1/) 
        /// </summary>
        /// <param name="beforeTransition">Optinal: Optinal: Task that is executed before the view transition is executed.</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public async Task StartViewTransitionAsync(Task? beforeTransition = null, CancellationToken cancellationToken = default)
        {
            var module = await _moduleTask.Value;
            _beforeTransition = beforeTransition;
            Console.WriteLine("Before call JS start transition");
            await module.InvokeVoidAsync("startViewTransition", cancellationToken, DotNetObjectReference.Create(this), nameof(TransitionStarted));
            Console.WriteLine("After called JS start transition");
            await _oldViewStateCompleted.Task;
            _oldViewStateCompleted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        [JSInvokable]
        public async Task TransitionStarted()
        {
            Console.WriteLine("After old screenshot is finished");
            _oldViewStateCompleted.SetResult();
            if (_beforeTransition is not null)
            {
                Console.WriteLine("wait for task");
                await _beforeTransition;
            }
            Console.WriteLine("finish method to run transition");
        }
    }
}
