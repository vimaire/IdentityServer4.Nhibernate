using System;
using NHibernate;

namespace IdentityServer4.NHibernate.Stores
{
    public class OperationalSessionProvider : IDisposable
    {
        private readonly Lazy<ISession> _sessionProvider;

        public ISession Session => _sessionProvider.Value;

        public OperationalSessionProvider(Func<ISession> sessionProvider)
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

                Session?.Dispose();
            }
        }
    }
}