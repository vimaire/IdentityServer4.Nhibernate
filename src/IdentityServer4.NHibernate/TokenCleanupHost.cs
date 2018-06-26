using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Options;
using Microsoft.Extensions.Hosting;

namespace IdentityServer4.NHibernate
{
    internal class TokenCleanupHost :  IHostedService
    {
        private readonly OperationalStoreOptions _options;
        private readonly TokenCleanup _tokenCleanup;

        public TokenCleanupHost(OperationalStoreOptions options, TokenCleanup tokenCleanup)
        {
            _options = options;
            _tokenCleanup = tokenCleanup;
            if (_options.EnableTokenCleanup)
            {
                _tokenCleanup.SetInterval(options.TokenCleanupInterval);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.EnableTokenCleanup)
            {
                _tokenCleanup.Start();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_options.EnableTokenCleanup)
            {
                _tokenCleanup.Stop();
            }
            return Task.CompletedTask;
        }
    }
}