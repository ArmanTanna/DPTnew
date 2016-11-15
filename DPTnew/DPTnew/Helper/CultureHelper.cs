using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Threading;
using System.Globalization;

namespace DPTnew.Helper
{
    public class CultureHelper
    {
        protected HttpSessionState session;

        //constructor   
        public CultureHelper(HttpSessionState httpSessionState)
        {
            session = httpSessionState;
        }
        // Properties  
        public static int CurrentCulture
        {
            get
            {
                switch (Thread.CurrentThread.CurrentUICulture.Name)
                {
                    case "it-IT": return 1;
                    case "ja-JP": return 2;
                    case "ko-KR": return 3;
                    case "de-DE": return 4;
                    case "fr-FR": return 5;
                    case "zh-CN": return 6;
                    case "en-US":
                    default: return 0;
                }
            }
            set
            {
                switch (value)
                {
                    case 0: Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                        break;
                    case 1: Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT");
                        break;
                    case 2: Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");
                        break;
                    case 3: Thread.CurrentThread.CurrentUICulture = new CultureInfo("ko-KR");
                        break;
                    case 4: Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
                        break;
                    case 5: Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
                        break;
                    case 6: Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                        break;
                    default: Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                        break;
                }

                //Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            }
        }
    }
}