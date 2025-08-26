namespace Kevin.Testing.Shared.Attributes
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; private set; }

        public TestPriorityAttribute(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Priority must be greater than 0"); // 抛出异常以指示错误
            }
            Priority = value;
        }
    }
}
