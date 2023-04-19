using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit;

/// <summary>
/// Represents an <see cref="ITestCaseOrderer"/> use to order test cases thanks to the <see cref="PriorityAttribute"/> they are marked with
/// </summary>
public class PriorityTestCaseOrderer
    : ITestCaseOrderer
{

    /// <inheritdoc/>
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        SortedDictionary<int, List<TTestCase>> sortedCases;
        int priority;
        sortedCases = new SortedDictionary<int, List<TTestCase>>();
        foreach (TTestCase testCase in testCases)
        {
            priority = 0;
            foreach (IAttributeInfo attr in testCase.TestMethod.Method.GetCustomAttributes((typeof(PriorityAttribute).AssemblyQualifiedName)))
                priority = attr.GetNamedArgument<int>(nameof(PriorityAttribute.Priority));
            GetOrCreate(sortedCases, priority).Add(testCase);
        }
        foreach (List<TTestCase> list in sortedCases.Keys.Select(p => sortedCases[p]))
        {
            list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            foreach (TTestCase testCase in list)
                yield return testCase;
        }
    }

    private static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
    {
        if (dictionary.TryGetValue(key, out var result)) return result;
        result = new TValue();
        dictionary[key] = result;
        return result;
    }

}
