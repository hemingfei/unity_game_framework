//
//  IDateTimeManager.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

namespace HGT.Core.DateTime
{
    /// <summary>
    ///     时间管理器接口。
    /// </summary>
    public interface IDateTimeManager
    {
        /// <summary>
        ///     获取 本地 时间 秒
        /// </summary>
        /// <returns>时间</returns>
        long GetLocalTimeStampSeconds();

        /// <summary>
        ///     获取 本地 时间 毫秒
        /// </summary>
        /// <returns>时间</returns>
        long GetLocalTimeStampMilliseconds();

        /// <summary>
        ///     获取 本地 时间戳 秒
        /// </summary>
        /// <returns>时间戳</returns>
        string GetLocalTimeStampSecondsString();

        /// <summary>
        ///     获取 本地 时间戳 毫秒
        /// </summary>
        /// <returns>时间戳</returns>
        string GetLocalTimeStampMillisecondsString();

        /// <summary>
        ///     秒时间戳转为时间值
        /// </summary>
        /// <param name="timeStampSecondsString">秒时间戳</param>
        /// <returns>时间</returns>
        long GetSecondsTimeFromStampSecond(string timeStampSecondsString);

        /// <summary>
        ///     毫秒时间戳转为时间值
        /// </summary>
        /// <param name="timeStampMillisecondsString">毫秒时间戳</param>
        /// <returns>时间</returns>
        long GetMilisecondsTimeFromStampMillisecond(string timeStampMillisecondsString);

        /// <summary>
        ///     秒时间 转为 日期值
        /// </summary>
        /// <param name="secondsTimeStamp"></param>
        /// <returns></returns>
        System.DateTime GetDatetimeFromSeconds(long secondsTime);

        /// <summary>
        ///     毫秒时间 转为 日期值
        /// </summary>
        /// <param name="milisecondsTimeStamp"></param>
        /// <returns></returns>
        System.DateTime GetDatetimeFromMilliseconds(long milisecondsTime);
    }
}