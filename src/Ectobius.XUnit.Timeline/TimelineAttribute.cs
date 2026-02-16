using System.Reflection;
using Xunit.Sdk;

namespace Ectobius.XUnit.Timeline
{
    public class TimelineAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            var className = methodUnderTest.DeclaringType?.FullName ?? methodUnderTest.DeclaringType?.Name ?? "Unknown";
            var methodName = methodUnderTest.Name;
            var testId = $"{className}.{methodName}";

            TimelineCollector.RecordStart(testId, methodName, className);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            var className = methodUnderTest.DeclaringType?.FullName ?? methodUnderTest.DeclaringType?.Name ?? "Unknown";
            var methodName = methodUnderTest.Name;
            var testId = $"{className}.{methodName}";

            TimelineCollector.RecordEnd(testId);
        }
    }
}
