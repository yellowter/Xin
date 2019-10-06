using System;

namespace Xin.Core.Utils
{
    public class Snowflake
    {
        private static readonly object _lock = new object();

        private static readonly Snowflake _instance = new Snowflake(0);
        private readonly long _maxFlowID = 1L << 4;
        private readonly long _maxWorkID = 1L << 6;
        private readonly long _offsetTicks = 0;
        private readonly long _workId;
        private long _lastFlowId;
        private long _lastTicks;


        /// <summary>
        ///     初始化机器码,范围0-31,如果超过范围,则WorkID等于0
        /// </summary>
        /// <param name="workID"></param>
        public Snowflake(long workID)
        {
            _workId = workID >= _maxWorkID ? 0 : workID;
        }

        public static long GenerateId()
        {
            return _instance.NewID();
        }


        /// <summary>
        ///     生成新的ID
        /// </summary>
        /// <returns></returns>
        public long NewID()
        {
            lock (_lock)
            {
                var ticks = GetTicks();
                ResetFlowID(ticks);
                var flowID = GetFlowID();
                // 如果流水号溢出,重新获取时间戳
                if (flowID >= _maxFlowID)
                {
                    ticks = GetNextTicks();
                    ResetFlowID(ticks);
                    flowID = GetFlowID();
                }

                return ticks | GetWorkID() | flowID;
            }
        }

        /// <summary>
        ///     重置流水号
        /// </summary>
        /// <param name="ticks"></param>
        private void ResetFlowID(long ticks)
        {
            if (ticks > _lastTicks)
            {
                _lastTicks = ticks;
                _lastFlowId = 0;
            }
        }

        /// <summary>
        ///     获取时间戳(55位)
        /// </summary>
        /// <returns></returns>
        private long GetTicks()
        {
            return ((DateTime.UtcNow.Ticks - _offsetTicks) << 8) & long.MaxValue;
        }

        /// <summary>
        ///     流水号溢出获取下一个时间戳
        /// </summary>
        /// <returns></returns>
        private long GetNextTicks()
        {
            var ticks = 0L;
            do
            {
                ticks = GetTicks();
            } while (ticks == _lastTicks);

            return ticks;
        }

        /// <summary>
        ///     获取机器ID(5位)
        /// </summary>
        /// <returns></returns>
        private long GetWorkID()
        {
            return _workId << 3;
        }

        /// <summary>
        ///     获取流水号(3位)
        /// </summary>
        /// <returns></returns>
        private long GetFlowID()
        {
            return _lastFlowId++;
        }
    }
}
