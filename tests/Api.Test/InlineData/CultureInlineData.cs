using System.Collections;

namespace Api.Test.InlineData
{
    public class CultureInlineData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "en-US" };
            yield return new object[] { "pt-BR" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
