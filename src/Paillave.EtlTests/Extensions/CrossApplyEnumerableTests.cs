using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paillave.Etl;
using Paillave.Etl.Extensions;

namespace Paillave.EtlTests.Extensions
{
    //TODO: Make test
    [TestClass()]
    public class CrossApplyEnumerableTests
    {
        [TestCategory(nameof(CrossApplyEnumerableTests))]
        [TestMethod]
        public void NoElements()
        {
            StreamProcessRunner.Create<object>(rootStream =>
            {
                rootStream.CrossApplyEnumerable("list elements", _ => Enumerable.Range(0, 10));
            }).ExecuteAsync(null).Wait();
        }
    }
}