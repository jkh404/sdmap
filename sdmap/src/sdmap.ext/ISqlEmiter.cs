namespace sdmap.ext
{
    public interface ISdmapEmiter
    {
        string Emit(string statementId, object parameters);

        string Emit(string assemblyFullName, string statementId, object parameters);
    }
}
