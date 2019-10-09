using System;

namespace Xin.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
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
        /// <param name="workId"></param>
        public Snowflake(long workId)
        {
            _workId = workId >= _maxWorkID ? 0 : workId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GenerateId()
        {
            return _instance.NewId();
        }


        /// <summary>
        ///     生成新的ID
        /// </summary>
        /// <returns></returns>
        public long NewId()
        {
            lock (_lock)
            {
                var ticks = GetTicks();
                ResetFlowId(ticks);
                var flowId = GetFlowId();
                // 如果流水号溢出,重新获取时间戳
                if (flowId >= _maxFlowID)
                {
                    ticks = GetNextTicks();
                    ResetFlowId(ticks);
                    flowId = GetFlowId();
                }

                return ticks | GetWorkId() | flowId;
            }
        }

        /// <summary>
        ///     重置流水号
        /// </summary>
        /// <param name="ticks"></param>
        private void ResetFlowId(long ticks)
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
        private long GetWorkId()
        {
            return _workId << 3;
        }

        /// <summary>
        ///     获取流水号(3位)
        /// </summary>
        /// <returns></returns>
        private long GetFlowId()
        {
            return _lastFlowId++;
        }
    }
}
