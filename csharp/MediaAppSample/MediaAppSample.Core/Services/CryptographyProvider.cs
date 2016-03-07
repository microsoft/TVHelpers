using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the cryptography provider of the platform currently executing.
        /// </summary>
        public CryptographyProvider Cryptography
        {
            get { return this.GetService<CryptographyProvider>(); }
            protected set { this.SetService<CryptographyProvider>(value); }
        }
    }

    /// <summary>
    /// Interface used to access the cryptography functions of the executing platform. Additional details of encryption implementation method: http://msdn.microsoft.com/en-us/library/windows/apps/windows.security.cryptography.dataprotection.dataprotectionprovider.aspx
    /// </summary>
    public sealed class CryptographyProvider : ServiceBase
    {
        #region Constructors

        internal CryptographyProvider()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="message">String to encrypt</param>
        /// <returns>Encrypted string</returns>
        public async Task<string> EncryptAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            string base64message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));
            IBuffer buffer = CryptographicBuffer.DecodeFromBase64String(base64message);
            DataProtectionProvider protectedData = new DataProtectionProvider("LOCAL=user");
            IBuffer encryptedBuffer = await protectedData.ProtectAsync(buffer);
            return CryptographicBuffer.EncodeToBase64String(encryptedBuffer);
        }

        /// <summary>
        /// Decrypts an encrypted string
        /// </summary>
        /// <param name="message">String to decrypt</param>
        /// <returns>Decrypted string inside the encrypted param string</returns>
        public async Task<string> DecryptAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            IBuffer buffer = CryptographicBuffer.DecodeFromBase64String(message);
            var protectedData = new DataProtectionProvider("LOCAL=user");
            var decryptedBuffer = await protectedData.UnprotectAsync(buffer);
            string base64message = CryptographicBuffer.EncodeToBase64String(decryptedBuffer);
            byte[] msgContents = Convert.FromBase64String(base64message);
            return System.Text.Encoding.UTF8.GetString(msgContents, 0, msgContents.Length);
        }

        #endregion
    }
}
