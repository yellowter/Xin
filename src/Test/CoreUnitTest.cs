using System;
using Xin.Core.Utils;
using Xunit;

namespace Test
{
    public class CoreUnitTest
    {
        [Fact]
        public void Test1()
        {
           var arr= NetworkUtil.MacToArray("1C:69:7A:07:44:48");
        }
    }
}