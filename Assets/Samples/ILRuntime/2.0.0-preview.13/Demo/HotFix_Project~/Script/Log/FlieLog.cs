using System.IO;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class FlieLog : ILogger
    {
        #region Field
        private const string mDirectoryPath = @"/Log";
        private const string mLogName = @"/Logger.txt";

        private ILogger mILogger;
        public ILogger Logger { get { return mILogger; } }

        public string DirectoryPath { get { return Application.persistentDataPath + mDirectoryPath; } }
        public string FilePath { get { return DirectoryPath + mLogName; } }

        #endregion

        #region Method
        #region Public
        public FlieLog(bool _isClear = false)
        {
            CreateFile();

            if (_isClear)
            {
                File.Delete(FilePath);
            }
        }
        public string context;
        public void Log(string _condition, string _stackTack, LogType _logType)
        {
            context = string.Format("时间:[{0}]\n内容:{1}\n类型:{2}栈:{3}\n", GetDataTime(), _condition, _logType, _stackTack);

            using (StreamWriter sw = new StreamWriter(FilePath, true, System.Text.Encoding.UTF8))
            {
                sw.Write(context);
                sw.Close();
            }
        }


        #endregion

        #region Private
        private void CreateFile()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            if (!File.Exists(FilePath))
            {
                Debug.Log($"创建日志文本 path:{FilePath}");
                var filestream = File.Create(FilePath);
                filestream.Close();
            }


        }



        private string GetDataTime()
        {
            return System.DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        #endregion
        #endregion
    }
}
