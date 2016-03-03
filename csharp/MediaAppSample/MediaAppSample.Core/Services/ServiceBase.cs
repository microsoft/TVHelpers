using MediaAppSample.Core.Models;
using System;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Services
{
    public interface IServiceSignout
    {
        Task SignoutAsync();
    }

    public abstract class ServiceBase : ModelBase, IDisposable
    {
        #region Properties

        /// <summary>
        /// Indicates whether or not this adapter has been initialized by the PlatformAdaptereCore Initialization method.
        /// </summary>
        internal bool Initialized { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialization logic which is called on launch of this application.
        /// </summary>
        protected internal virtual void Initialize()
        {
            this.Initialized = true;
        }

        public virtual void Dispose()
        {
        }

        #endregion
    }
}