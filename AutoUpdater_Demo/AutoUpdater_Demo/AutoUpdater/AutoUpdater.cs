﻿using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization /* Add System.Web.Extesnion Reference */;
using System.Windows.Forms;

public class AutoUpdater
{
    private class Properties
    {
        public string Version { get;   set; }
        public string DownloadUrl { get;  set; }
        public string ChangeLog{ get;  set; }
        public bool IsClosed { get;  set; }
        public string ClosedMessage { get;  set; }
    }
    private static string _MediaFireDirectLink(string mediaFirelink)
    {
        string content = new WebClient().DownloadString(mediaFirelink);
        return content.Contains("mediafire") ? Regex.Match(content, @"http://download.*.mediafire.com.*(.[\w])").Value : content;
    }
    private static void _Close()
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
    private static DialogResult _Msg(object msg, MessageBoxIcon icon, bool ques = false)
    {
        return MessageBox.Show(msg.ToString(), "Message", ques ? MessageBoxButtons.YesNo : MessageBoxButtons.OK, icon);
    }
    /// <summary>
    /// Check For Updates With Message Box
    /// </summary>
    /// <param name="currentVersion">Current Application Version</param>
    /// <param name="jsonContent">The url that contains your application details as JSON</param>
    /// <returns>Return Current Version</returns>
    public static decimal CheckForUpdate(decimal currentVersion, string jsonContent)
    {
        try
        {
            using (WebClient wb = new WebClient())
            {
                wb.Encoding = Encoding.UTF8;
                Properties prop = new JavaScriptSerializer().Deserialize<Properties>(wb.DownloadString(jsonContent));
                if (prop.IsClosed)
                {
                    _Msg(prop.ClosedMessage, MessageBoxIcon.Error);
                    _Close();
                }
                if ((decimal.Parse(prop.Version) > currentVersion))
                {
                    if (_Msg("New Update Available!\nCurrent Version: " + currentVersion + "\nNew Version: " + prop.Version, MessageBoxIcon.Information, true) == DialogResult.Yes)
                    {
                        using (SaveFileDialog sf = new SaveFileDialog() { Filter = "Rar File |*.rar", Title = prop.Version })
                        {
                            if (sf.ShowDialog() == DialogResult.OK)
                            {
                                wb.DownloadFile(_MediaFireDirectLink(prop.DownloadUrl), sf.FileName);
                                _Msg("File Has Been Downloaded", MessageBoxIcon.Asterisk);
                                _Close();
                            }
                            else
                            {
                                _Close();
                            }
                        }
                    }
                    else
                    {
                        _Msg("You have To use the new Version: " + prop.Version, MessageBoxIcon.Error);
                        _Close();

                    }
                }

            }
        }
        catch (Exception ex)
        {
            _Msg(ex.Message, MessageBoxIcon.Error);
            _Close();
        }

        return currentVersion;
    }
}

