using System.Security.Principal;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity;

public partial class Job : ISerializable
{
    public string GetFileName()
    {
        return Enum.GetName(Identifier) ?? throw new Exception();
    }

    public string GetSubDirectory()
    {
        return "jobs";
    }
}
