namespace AtsPlugin.Parametrics
{
    public abstract class AtsParametricBase
    {
        public string Path { get; protected set; } = string.Empty;
        public string FileName { get => System.IO.Path.GetFileName(Path); }


        public AtsParametricBase()
        {
        }

        public AtsParametricBase(string path)
        {
            Path = path;
        }
    }
}
