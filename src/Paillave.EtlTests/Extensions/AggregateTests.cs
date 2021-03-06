using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paillave.Etl;
using Paillave.Etl.Extensions;

namespace Paillave.EtlTests.Extensions
{
    [TestClass()]
    public class AggregateTests
    {
        [TestCategory(nameof(AggregateTests))]
        [TestMethod]
        public void GroupElements()
        {
            #region group elements
            var inputList = Enumerable.Range(0, 10).ToList();
            var outputList = new List<string>();

            StreamProcessRunner.CreateAndExecuteAsync(inputList, rootStream =>
            {
                rootStream
                    .CrossApplyEnumerable("list elements", config => config)
                    .Select("produce business object", i => new { Value = i, Category = $"CAT{i % 2}" })
                    .Aggregate("compute sum per category",
                        i => "",
                        i => i.Category,
                        (agg, elt) => $"{agg}{elt.Value}")
                    .Select("format result", i => $"{i.Key}:{i.Aggregation}")
                    .ThroughAction("collect values", outputList.Add);
            }).Wait();

            CollectionAssert.AreEquivalent(new[] { "CAT0:02468", "CAT1:13579" }.ToList(), outputList);
            #endregion
        }

        [TestCategory(nameof(AggregateTests))]
        [TestMethod]
        public void ComputeAverage()
        {
            #region compute average
            var inputList = Enumerable.Range(0, 10).ToList();
            var outputList = new List<string>();

            StreamProcessRunner.CreateAndExecuteAsync(inputList, rootStream =>
            {
                rootStream
                    .CrossApplyEnumerable("list elements", config => config)
                    .Select("produce business object", i => new { Value = i, Category = $"CAT{i % 2}" })
                    .Aggregate("compute sum per category",
                        i => new { Nb = 0, Sum = 0 },
                        i => i.Category,
                        (agg, elt) => new { Nb = agg.Nb + 1, Sum = agg.Sum + elt.Value })
                    .Select("format and compute result", i => $"{i.Key}:{i.Aggregation.Sum / i.Aggregation.Nb}")
                    .ThroughAction("collect values", outputList.Add);
            }).Wait();

            CollectionAssert.AreEquivalent(new[] { "CAT0:4", "CAT1:5" }.ToList(), outputList);
            #endregion
        }
    }
}
