using NHibernate;
using System;

namespace IdentityServer4.NHibernate.Stores
{
    public class ConfigurationSessionProvider : IDisposable
    {
        private readonly Lazy<ISession> _sessionProvider;

        public ISession Session => _sessionProvider.Value;

        public ConfigurationSessionProvider(Func<ISession> sessionProvider)
        {
            _sessionProvider = new Lazy<ISession>(() =>
            {
                var session = sessionProvider();
                session.BeginTransaction();
                return session;
            });
        }

        public void Dispose()
        {
            if (_sessionProvider.IsValueCreated
                && Session?.Transaction?.IsActive == true)
            {
                try
                {
                    Session.Transaction.Commit();
                }
                catch
                {
                    Session.Transaction.Rollback();
                    throw;
                }
                finally
                {
                    Session.Transaction.Dispose();
                }

                Session.Dispose();
            }
        }
    }
}