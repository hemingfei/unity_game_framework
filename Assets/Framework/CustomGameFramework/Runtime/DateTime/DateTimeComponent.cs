//
//  TimeManager.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using HGT.Core.DateTime;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{

    public class DateTimeComponent : GameFrameworkComponent, IDateTimeManager
    {
        /// <summary>
        /// 获取 本地 时间 毫秒
        /// </summary>
        /// <returns>时间</returns>
        public long GetLocalTimeStampMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
        /// <summary>
        /// 获取 本地 时间戳 毫秒
        /// </summary>
        /// <returns>时间戳</returns>
        public string GetLocalTimeStampMillisecondsString()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        /// <summary>
        /// 获取 本地 时间 秒
        /// </summary>
        /// <returns>时间</returns>
        public long GetLocalTimeStampSeconds()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        /// <summary>
        /// 获取 本地 时间戳 秒
        /// </summary>
        /// <returns>时间戳</returns>
        public string GetLocalTimeStampSecondsString()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        
        /// <summary>
        /// 秒时间戳转为时间值
        /// </summary>
        /// <param name="timeStampSecondsString">秒时间戳</param>
        /// <returns>时间</returns>
        public long GetSecondsTimeFromStampSecond(string timeStampSecondsString)
        {
            return long.Parse(timeStampSecondsString);
        }
        
        /// <summary>
        /// 毫秒时间戳转为时间值
        /// </summary>
        /// <param name="timeStampMillisecondsString">毫秒时间戳</param>
        /// <returns>时间</returns>
        public long GetMilisecondsTimeFromStampMillisecond(string timeStampMillisecondsString)
        {
            return long.Parse(timeStampMillisecondsString);
        }

        public System.DateTime GetDatetimeFromSeconds(long secondsTime)
        {
            System.DateTime dtStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            long lTime = long.Parse(secondsTime + "0000000");
            System.TimeSpan toNow = new System.TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public System.DateTime GetDatetimeFromMilliseconds(long milisecondsTime)
        {
            System.DateTime dtStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            System.TimeSpan toNow = new System.TimeSpan(milisecondsTime);
            return dtStart.Add(toNow);
        }
    }
}
