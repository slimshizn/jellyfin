using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Tasks;

namespace Emby.Server.Implementations.LiveTv
{
    /// <summary>
    /// The "Refresh Guide" scheduled task.
    /// </summary>
    public class RefreshGuideScheduledTask : IScheduledTask, IConfigurableScheduledTask
    {
        private readonly ILiveTvManager _liveTvManager;
        private readonly IConfigurationManager _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshGuideScheduledTask"/> class.
        /// </summary>
        /// <param name="liveTvManager">The live tv manager.</param>
        /// <param name="config">The configuration manager.</param>
        public RefreshGuideScheduledTask(ILiveTvManager liveTvManager, IConfigurationManager config)
        {
            _liveTvManager = liveTvManager;
            _config = config;
        }

        /// <inheritdoc />
        public string Name => "Refresh Guide";

        /// <inheritdoc />
        public string Description => "Downloads channel information from live tv services.";

        /// <inheritdoc />
        public string Category => "Live TV";

        /// <inheritdoc />
        public bool IsHidden => _liveTvManager.Services.Count == 1 && GetConfiguration().TunerHosts.Length == 0;

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public bool IsLogged => true;

        /// <inheritdoc />
        public string Key => "RefreshGuide";

        /// <inheritdoc />
        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var manager = (LiveTvManager)_liveTvManager;

            return manager.RefreshChannels(progress, cancellationToken);
        }

        /// <inheritdoc />
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                // Every so often
                new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerInterval, IntervalTicks = TimeSpan.FromHours(24).Ticks }
            };
        }

        private LiveTvOptions GetConfiguration()
        {
            return _config.GetConfiguration<LiveTvOptions>("livetv");
        }
    }
}
