using IdentityServer4.NHibernate.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.NHibernate
{
    public class TokenCleanup
    {
        private readonly IServiceProvider _serviceProvider;

        private CancellationTokenSource _source;
        
        private TimeSpan CleanupInterval { get; set; } = TimeSpan.FromSeconds(30);

        public TokenCleanup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Start()
        {
            Start(CancellationToken.None);
        }

        private void Start(CancellationToken cancellationToken)
        {
            if (_source != null) throw new InvalidOperationException("Already started. Call Stop first.");

            _source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Factory.StartNew(() => StartInternal(_source.Token));
        }

        public void Stop()
        {
            if (_source == null) throw new InvalidOperationException("Not started. Call Start first.");

            _source.Cancel();
            _source = null;
        }

        private async Task StartInternal(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    await Task.Delay(CleanupInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch
                {
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await ClearTokens(DateTime.UtcNow);
            }
        }

        public async Task ClearTokens(DateTime expirationDate)
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var provider = serviceScope.ServiceProvider.GetService<OperationalSessionProvider>())
                {
                    const string sql = "delete from PersistedGrant p where p.Expiration < :dt";
                    await provider.Session.CreateQuery(sql)
                        .SetParameter("dt", expirationDate)
                        .ExecuteUpdateAsync();
                }
            }
        }

        public void SetInterval(TimeSpan cleanupInterval)
        {
            CleanupInterval = cleanupInterval;
        }
    }
}