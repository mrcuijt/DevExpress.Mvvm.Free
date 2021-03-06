using System;
using System.Diagnostics;
using System.IO;
using DevExpress.Internal.WinApi;

namespace DevExpress.Data {
    public static class ShellHelper {
        public static void TryCreateShortcut(string applicationId, string name, string iconPath = null) {
            string shortcutPath = GetShortcutPath(name);
            if(!File.Exists(shortcutPath))
                InstallShortcut(shortcutPath, applicationId, iconPath);
        }
        public static void TryCreateShortcut(string sourcePath, string applicationId, string name, string iconPath = null) {
            string shortcutPath = GetShortcutPath(name);
            if(!File.Exists(shortcutPath))
                InstallShortcut(sourcePath, shortcutPath, applicationId, iconPath);
        }
        public static void TryRemoveShortcut(string name) {
            string shortcutPath = GetShortcutPath(name);
            if(File.Exists(shortcutPath))
                File.Delete(shortcutPath);
        }
        public static bool IsApplicationShortcutExist(string name) {
            string shortcutPath = GetShortcutPath(name);
            return File.Exists(shortcutPath);
        }
        static string GetShortcutPath(string name) {
            return Path.Combine(GetProgramsFolder(), name + ".lnk");
        }
        static void InstallShortcut(string shortcutPath, string applicationId, string iconPath) {
            var process = Process.GetCurrentProcess();
            InstallShortcut(process.MainModule.FileName, shortcutPath, applicationId, iconPath);
        }
        static void InstallShortcut(string sourcePath, string shortcutPath, string applicationId, string iconPath) {
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();
            ErrorHelper.VerifySucceeded(newShortcut.SetPath(sourcePath));
            ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));
            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;
            if(iconPath != null)
                newShortcut.SetIconLocation(iconPath, 0);
            using(PropVariant appId = new PropVariant(applicationId)) {
                PropertyKey SystemProperties_System_AppUserModel_ID = new PropertyKey(new Guid("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}"), 5);
                ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties_System_AppUserModel_ID, appId));
                ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;
            ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }
        static string GetProgramsFolder() {
            return Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        }
    }
}