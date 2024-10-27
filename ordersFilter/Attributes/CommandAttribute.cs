namespace ordersFilter.Attributes
{
    /// <summary>
    /// Обозначает команду
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class CommandAttribute(string command) : Attribute
    {
        /// <summary>
        /// Текст, которым вызывается команда
        /// </summary>
        internal string Command { get; } = command;
    }
}
