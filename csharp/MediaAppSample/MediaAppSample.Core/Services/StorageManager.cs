// The MIT License (MIT)
//
// Copyright (c) 2016 Microsoft. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the storage system of the platform currently executing.
        /// </summary>
        public StorageManager Storage
        {
            get { return this.GetService<StorageManager>(); }
            protected set { this.SetService<StorageManager>(value); }
        }
    }

    /// <summary>
    /// Base class for accessing storage on the platform currently being executed.
    /// </summary>
    public sealed class StorageManager : ServiceBase, IServiceSignout
    {
        #region Constructors

        internal StorageManager()
        {
        }

        #endregion

        #region Methods

        #region Credentials

        /// <summary>
        /// Loads a user credential from secure storage.
        /// </summary>
        /// <param name="credentialName">Name of the credential set.</param>
        /// <param name="username">Username to retrieve.</param>
        /// <param name="password">Reference parameter contains the stored password.</param>
        /// <returns>True if the credential was found else false.</returns>
        public bool LoadCredential(string credentialName, string username, ref string password)
        {
            if (string.IsNullOrWhiteSpace(credentialName))
                throw new ArgumentNullException(nameof(credentialName));
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            System.Collections.Generic.IReadOnlyList<PasswordCredential> credsList = null;

            try
            {
                PasswordVault vault = new PasswordVault();
                credsList = vault.FindAllByResource(credentialName);
            }
            catch
            {
            }

            if (credsList != null && credsList.Count > 0)
            {
                PasswordCredential cred = credsList.FirstOrDefault(s => s.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase));

                if (cred != null)
                {
                    cred.RetrievePassword();
                    password = cred.Password;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves a user's credentials to secure storage.
        /// </summary>
        /// <param name="credentialName">Name of the credential set.</param>
        /// <param name="username">Username to store.</param>
        /// <param name="password">Password to store.</param>
        public void SaveCredential(string credentialName, string username, string password)
        {
            if (string.IsNullOrEmpty(credentialName))
                throw new ArgumentNullException(nameof(credentialName));

            PasswordVault vault = new PasswordVault();

            try
            {
                foreach (var cred in vault.FindAllByResource(credentialName))
                    if (string.IsNullOrEmpty(username) || cred.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase))
                        vault.Remove(cred);
            }
            catch { }

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                vault.Add(new PasswordCredential(credentialName, username, password));
        }

        #endregion Credentials

        #region Settings

        /// <summary>
        /// Loads a setting from storage.
        /// </summary>
        /// <typeparam name="T">Type of the object stored in settings.</typeparam>
        /// <param name="key">Unique key for the setting.</param>
        /// <param name="serializerType">How the setting should be serialized to storage.</param>
        /// <returns>Object instance of the type specified if found else null.</returns>
        public T LoadSetting<T>(string key, SerializerTypes serializerType = SerializerTypes.Default)
        {
            return this.LoadSetting<T>(key, ApplicationData.Current.LocalSettings, serializerType);
        }

        /// <summary>
        /// Retrieves a setting from storage.
        /// </summary>
        /// <typeparam name="T">Type of the stored value.</typeparam>
        /// <param name="key">Unique key for the setting.</param>
        /// <param name="location">Location the setting should be retrieved from.</param>
        /// <param name="serializerType">How the setting should be serialized to storage.</param>
        /// <returns>Awaitable task is returned.</returns>
        public T LoadSetting<T>(string key, ApplicationDataContainer container, SerializerTypes serializerType = SerializerTypes.Default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                if (container.Values.ContainsKey(key))
                {
                    // Don't deserialize the value if its a primitive type
                    if (TypeUtility.IsPrimitive(typeof(T)))
                        return (T)container.Values[key];
                    else
                        return Serializer.Deserialize<T>(container.Values[key].ToString(), serializerType);
                }
                else
                    return default(T); // Not found in the container return the default value.
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error retrieving key '{0}' from container '{1}' using '{2}' serializer.", key, container.Locality, serializerType);
                return default(T);
            }
        }

        /// <summary>
        /// Checks to see if a setting exists in the LocalSettings container or not.
        /// </summary>
        /// <param name="key">Unique key for the setting.</param>
        /// <returns>True if found else false.</returns>
        public bool ContainsSetting(string key)
        {
            return this.ContainsSetting(key, ApplicationData.Current.LocalSettings);
        }

        /// <summary>
        /// Checks to see if a setting exists or not.
        /// </summary>
        /// <param name="key">Unique key for the setting.</param>
        /// <param name="container">Container to check.</param>
        /// <returns>True if found else false.</returns>
        public bool ContainsSetting(string key, ApplicationDataContainer container)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            return container.Values.ContainsKey(key);
        }

        /// <summary>
        /// Saves a setting to the LocalSettings container.
        /// </summary>
        /// <param name="key">Unique key for the setting.</param>
        /// <param name="obj">Object to store in the LocalSettings container.</param>
        /// <param name="serializerType">How the setting should be serialized to storage.</param>
        public void SaveSetting(string key, object obj, SerializerTypes serializerType = SerializerTypes.Default)
        {
            this.SaveSetting(key, obj, ApplicationData.Current.LocalSettings, serializerType);
        }

        /// <summary>
        /// Saves a setting to storage.
        /// </summary>
        /// <param name="key">Unique key for the setting.</param>
        /// <param name="obj">Value to store.</param>
        /// <param name="location">Location the setting should be stored to.</param>
        /// <param name="serializerType">How the setting should be deserialized from storage.</param>
        public void SaveSetting(string key, object obj, ApplicationDataContainer container, SerializerTypes serializerType = SerializerTypes.Default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            // Remove the value if null
            if (obj == null)
            {
                if (container.Values.ContainsKey(key))
                    container.Values.Remove(key);
                return;
            }

            // Don't serialize the object if its a primitive type.
            object value = null;
            if (TypeUtility.IsPrimitive(obj.GetType()))
                value = obj;
            else
                value = Serializer.Serialize(obj, serializerType);

            // Update or add the setting.
            if (container.Values.ContainsKey(key))
                container.Values[key] = value;
            else
                container.Values.Add(key, value);
        }

        #endregion Settings

        #region Files

        /// <summary>
        /// Loads a file from storage.
        /// </summary>
        /// <param name="path">Path and name of the file.</param>
        /// <param name="folder">Which storage location to retrieve the file from.</param>
        /// <returns>String contents from the file or null if it doesn't exists.</returns>
        public async Task<string> ReadFileAsStringAsync(string path, StorageFolder folder)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            try
            {
                // Get to the folder from the string path
                StorageFolder destinationFolder = await this.GetToFolderFromPathAsync(path, folder);

                // Retrieve the file from that folder
                StorageFile textFile = await destinationFolder.GetFileAsync(Path.GetFileName(path));

                // Read the whole file as a string.
                string contents = null;
                using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
                {
                    using (DataReader textReader = new DataReader(textStream))
                    {
                        uint textLength = (uint)textStream.Size;
                        await textReader.LoadAsync(textLength);
                        contents = textReader.ReadString(textLength);
                    }
                }

                // Return the entire contents of the file as a string.
                return contents;
            }
            catch (FileNotFoundException)
            {
                Platform.Current.Logger.Log(LogLevels.Debug, "File not found! '{0}' in '{1}'", path, folder);
                return null;
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error while attempting to read file '{0}' in '{1}'", path, folder);
                return null;
            }
        }

        /// <summary>
        /// Loads a file from storage.
        /// </summary>
        /// <typeparam name="T">Type representing the data stored.</typeparam>
        /// <param name="path">Path and name of the file.</param>
        /// <param name="folder">Which storage location to retrieve the file from.</param>
        /// <returns>String contents from the file or null if it doesn't exists.</returns>
        /// <returns>Awaitable task of type specified.</returns>
        public async Task<T> LoadFileAsync<T>(string path, StorageFolder folder, SerializerTypes fileType = SerializerTypes.Default)
        {
            string data = await this.ReadFileAsStringAsync(path, folder);
            return Serializer.Deserialize<T>(data, fileType);
        }

        /// <summary>
        /// Gets a file by path.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <param name="folder">StorageFolder to check for a file.</param>
        /// <returns>IStorageFile instance if found else null.</returns>
        public async Task<IStorageFile> GetFileAsync(string path, StorageFolder folder)
        {
            StorageFolder destinationFolder = await this.GetToFolderFromPathAsync(path, folder);
            return await destinationFolder.GetFileAsync(Path.GetFileName(path));
        }

        /// <summary>
        /// Saves a string to file in storage.
        /// </summary>
        /// <param name="path">Path and name of the file.</param>
        /// <param name="data">Data to write to file.</param>
        /// <param name="location">Location to store the file.</param>
        /// <returns>Awaitable task returning the final file path in storage.</returns>
        public async Task<StorageFile> SaveFileAsync(string path, object data, StorageFolder folder, SerializerTypes serializerType = SerializerTypes.Default)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            // Serialize the object to string
            string dataAsString = null;
            if (data is string)
                dataAsString = data.ToString();
            else if(data != null)
                dataAsString = Serializer.Serialize(data, serializerType);

            if (string.IsNullOrEmpty(dataAsString))
            {
                // Delete the file if null or empty string.
                await this.DeleteFileAsync(path, folder);
                return null;
            }
            else
            {
                // Get to the requested folder destination in the path.
                StorageFolder destinationFolder = await this.GetToFolderFromPathAsync(path, folder, true);

                // Create the file
                StorageFile textFile = await destinationFolder.CreateFileAsync(Path.GetFileName(path), CreationCollisionOption.ReplaceExisting);

                // Write string data to file.
                using (IRandomAccessStream textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (DataWriter textWriter = new DataWriter(textStream))
                    {
                        textWriter.WriteString(dataAsString);
                        await textWriter.StoreAsync();
                    }
                }

                // Return handle to file
                return textFile;
            }
        }

        /// <summary>
        /// Deletes a file from storage.
        /// </summary>
        /// <param name="path">Path and name of file.</param>
        /// <param name="folder">Location where the file is stored.</param>
        /// <returns>Awaitable task.</returns>
        public async Task DeleteFileAsync(string path, StorageFolder folder)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            try
            {
                // Get to the requested folder destination in the path.
                StorageFolder destinationForlder = await this.GetToFolderFromPathAsync(path, folder);

                if (destinationForlder != null)
                {
                    // Get to the file and delete if found.
                    var file = await destinationForlder.GetFileAsync(Path.GetFileName(path));
                    if (file != null)
                        await file.DeleteAsync();
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        /// <summary>
        /// Checks wether or not a file exists at a specified location.
        /// </summary>
        /// <param name="path">Path and name of the file.</param>
        /// <param name="folder">Location of where the file is stored.</param>
        /// <returns>True if file exists else False.</returns>
        public async Task<bool> DoesFileExistsAsync(string path, StorageFolder folder)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            try
            {
                // Get to the requested folder destination in the path.
                StorageFolder destinationFolder = await this.GetToFolderFromPathAsync(path, folder);

                // Check if file exists or not and return true if found else false
                string fileName = Path.GetFileName(path);
                var files = await destinationFolder.GetFilesAsync();
                var file = files.FirstOrDefault(x => x.Name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));
                return file != null;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        #endregion Files

        #region Folders

        /// <summary>
        /// Gets to a folder object found in a string path.
        /// </summary>
        /// <param name="path">Path to evaluate.</param>
        /// <param name="startingFolder">Starting folder location.</param>
        /// <param name="createIfNotFound">Create the folder if it doesn't exist.</param>
        /// <returns>Return a handle to the folder.</returns>
        private async Task<StorageFolder> GetToFolderFromPathAsync(string path, StorageFolder startingFolder, bool createIfNotFound = false)
        {
            string[] folderNames = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int folderDepth = folderNames.Length - 1;
            for (int i = 0; i < folderDepth; i++)
            {
                if (createIfNotFound)
                    startingFolder = await startingFolder.CreateFolderAsync(folderNames[i], CreationCollisionOption.OpenIfExists);
                else
                    startingFolder = await startingFolder.GetFolderAsync(folderNames[i]);
            }
            return startingFolder;
        }

        /// <summary>
        /// Deletes a specified folder.
        /// </summary>
        /// <param name="folderName">Folder name to delete.</param>
        /// <param name="startingFolder">Starting folder location.</param>
        /// <returns></returns>
        public async Task DeleteFolderAsync(string folderName, StorageFolder startingFolder)
        {
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));

            try
            {
                // Get to the requested folder destination in the path.
                var folder = await startingFolder.GetFolderAsync(folderName);
                if (folder != null)
                    await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch(FileNotFoundException)
            {
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not delete folder '{0}' in '{1}'", folderName, startingFolder.DisplayName);
                throw ex;
            }
        }

        /// <summary>
        /// Calculates the size of a folder.
        /// </summary>
        /// <param name="folderName">Folder name to delete.</param>
        /// <param name="startingFolder">Starting folder location.</param>
        /// <returns>Formatted string displaying the size of the folder.</returns>
        public async Task<string> GetFolderSizeAsync(string folderName, StorageFolder startingFolder)
        {
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));

            try
            {
                // Get to the requested folder destination in the path.
                var folder = await startingFolder.GetFolderAsync(folderName);
                if (folder != null)
                {
                    long size = 0;
                    foreach(var file in await folder.GetFilesAsync())
                    {
                        var p = await file.GetBasicPropertiesAsync();
                        size += (long)p.Size;
                    }
                    return size.ToStringAsMemory();
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not get folder size for '{0}' in '{1}'", folderName, startingFolder.DisplayName);
                throw ex;
            }

            return null;
        }

        public Task<string> GetAppDataCacheFolderSizeAsync()
        {
            return this.GetFolderSizeAsync("AppDataCache", ApplicationData.Current.LocalCacheFolder);
        }

        public Task ClearAppDataCacheFolderAsync()
        {
            try
            {
                return this.DeleteFolderAsync("AppDataCache", ApplicationData.Current.LocalCacheFolder);
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error during ClearAppDataCacheFolder");
                return Task.CompletedTask;
            }
        }

        #endregion

        #region Logout

        /// <summary>
        /// When a user signs out of the app, delete their AppDataCache folder to protect sensitive data from being shown to other users potentially.
        /// </summary>
        /// <returns></returns>
        public Task SignoutAsync()
        {
            return this.ClearAppDataCacheFolderAsync();
        }

        #endregion

        #endregion Methods
    }
}
