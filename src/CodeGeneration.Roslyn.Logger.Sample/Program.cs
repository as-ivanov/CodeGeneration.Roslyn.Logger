using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeGeneration.Roslyn.Logger.Sample
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			using var host = Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<MyBackgroundTaskWithLogs>();
					services.AddSingleton<IBackgroundTaskManagerLogger, BackgroundTaskManagerLogger>();

				})
				.ConfigureLogging(logging => { logging.AddConsole(); })
				.Build();
			await host.StartAsync();

			await host.WaitForShutdownAsync();
		}
	}
}