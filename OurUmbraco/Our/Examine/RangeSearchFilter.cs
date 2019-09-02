using Lucene.Net.Search;
using Lucene.Net.Search.Payloads;

namespace OurUmbraco.Our.Examine
{
    public class RangeSearchFilter : SearchFilter
    {
        private readonly float? _boost;
        public long To { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public RangeSearchFilter(string fieldName, long from, long to, float? boost = null) : base(fieldName, from)
        {
            _boost = boost;
            To = to;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            //see https://lucene.apache.org/core/2_9_4/queryparsersyntax.html#Range Searches
            if (_boost.HasValue)
                return string.Format("{0}:[{1} TO {2}]^{3}", FieldName, Value, To, _boost.Value);
            return string.Format("{0}:[{1} TO {2}]", FieldName, Value, To);
        }

        /// <summary>
        /// Can be used to return a true Lucene query object if the string format is not good enough
        /// </summary>
        /// <returns></returns>
        public override Query GetLuceneQuery()
        {
            var query =  NumericRangeQuery.NewLongRange(FieldName, (long)Value, To, true, true);
            if (_boost.HasValue)
            {
                query.SetBoost(_boost.Value);
            }
            return query;
        }
    }
}