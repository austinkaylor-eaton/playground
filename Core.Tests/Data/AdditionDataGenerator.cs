namespace Core.Tests.Data;

/// <summary>
/// A data generator attribute that provides test data for addition operations.
/// </summary>
/// <example>
/// <code>
/// [AdditionDataGenerator]
/// public async Task TestAddition(int a, int b, int expectedSum)
/// {
///     Assert.Equal(expectedSum, a + b);
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class AdditionDataGeneratorAttribute : DataSourceGeneratorAttribute<int, int, int>
{
    protected override IEnumerable<Func<(int, int, int)>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        yield return () => (1, 1, 2);
        yield return () => (4, 5, 9);
        yield return () => (-1, 1, 0);
    }
}