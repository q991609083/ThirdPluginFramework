using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LanguageSetting
{
    public enum LanguageType
    {
        SimpleChinese,
        English,
    }
    public class LanguageManager
    {
        #region SingleInstance
        private static LanguageManager _instance = null;
        private static object _lock = new object();
        private LanguageManager() { }

        public static LanguageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance = new LanguageManager();
                        if (_instance == null)
                        {
                            _instance = new LanguageManager();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        public LanguageType languageType;
        /// <summary>
        /// 根据发布的平台，设置语言类型
        /// </summary>
        public void InitLanguageSetting()
        {
#if CHANNEL_HUAWEI
            languageType = LanguageType.English;
#else
            languageType =  LanguageType.SimpleChinese;
#endif
        }


    }
}


