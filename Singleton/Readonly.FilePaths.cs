public partial class Readonly
{
	public static class FilePaths
	{
		public static string GetProfilePath(string profileName) => $"user://{profileName}/";
	}
}
