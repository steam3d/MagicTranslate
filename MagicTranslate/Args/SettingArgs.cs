namespace MagicTranslate.Args
{
    internal class SettingArgs
    {
        public string ContainerName { get; }
        public string SettingName { get; }
        public object NewValue { get; }
        public object OldValue { get; }

        public SettingArgs(string containerName, string settingName, object newValue, object oldValue)
        {
            ContainerName = containerName;
            SettingName = settingName;
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}
