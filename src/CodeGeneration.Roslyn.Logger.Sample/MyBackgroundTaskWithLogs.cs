using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace CodeGeneration.Roslyn.Logger.Sample
{
	public class MyBackgroundTaskWithLogs : IHostedService, IDisposable
	{
		private readonly IBackgroundTaskManagerLogger _logger;
		private Timer _timer;
		private readonly List<string> _tasks = new List<string>() { "task1", "task2", "task3" };

		public MyBackgroundTaskWithLogs(IBackgroundTaskManagerLogger logger)
		{
			_logger = logger;
		}
		
		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(RunAll, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
			return Task.CompletedTask;
		}

		private void RunAll(object state)
		{
			foreach (var task in _tasks)
			{
				Run(task);
			}
		}

		private void Run(string name)
		{
			_logger.ExecutionStarted(name);
			try
			{
				DoWork();
			}
			catch (Exception e)
			{
				_logger.ExecutionFailed(name, e);
			}
			_logger.ExecutionFinished(name);
		}

		private void DoWork()
		{
			//...
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