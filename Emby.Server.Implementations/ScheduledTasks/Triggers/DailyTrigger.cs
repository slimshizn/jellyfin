using System;
using System.Threading;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Emby.Server.Implementations.ScheduledTasks
{
    /// <summary>
    /// Represents a task trigger that fires everyday.
    /// </summary>
    public sealed class DailyTrigger : ITaskTrigger
    {
        private readonly TimeSpan _timeOfDay;
        private Timer? _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyTrigger"/> class.
        /// </summary>
        /// <param name="timeofDay">The time of day to trigger the task to run.</param>
        /// <param name="taskOptions">The options of this task.</param>
        public DailyTrigger(TimeSpan timeofDay, TaskOptions taskOptions)
        {
            _timeOfDay = timeofDay;
            TaskOptions = taskOptions;
        }

        /// <summary>
        /// Occurs when [triggered].
        /// </summary>
        public event EventHandler<EventArgs>? Triggered;

        /// <summary>
        /// Gets the options of this task.
        /// </summary>
        public TaskOptions TaskOptions { get; }

        /// <summary>
        /// Stars waiting for the trigger action.
        /// </summary>
        /// <param name="lastResult">The last result.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="taskName">The name of the task.</param>
        /// <param name="isApplicationStartup">if set to <c>true</c> [is application startup].</param>
        public void Start(TaskResult lastResult, ILogger logger, string taskName, bool isApplicationStartup)
        {
            DisposeTimer();

            var now = DateTime.Now;

            var triggerDate = now.TimeOfDay > _timeOfDay ? now.Date.AddDays(1) : now.Date;
            triggerDate = triggerDate.Add(_timeOfDay);

            var dueTime = triggerDate - now;

            logger.LogInformation("Daily trigger for {Task} set to fire at {TriggerDate:yyyy-MM-dd HH:mm:ss.fff zzz}, which is {DueTime:c} from now.", taskName, triggerDate, dueTime);

            _timer = new Timer(state => OnTriggered(), null, dueTime, TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Stops waiting for the trigger action.
        /// </summary>
        public void Stop()
        {
            DisposeTimer();
        }

        /// <summary>
        /// Disposes the timer.
        /// </summary>
        private void DisposeTimer()
        {
            _timer?.Dispose();
        }

        /// <summary>
        /// Called when [triggered].
        /// </summary>
        private void OnTriggered()
        {
            Triggered?.Invoke(this, EventArgs.Empty);
        }
    }
}
