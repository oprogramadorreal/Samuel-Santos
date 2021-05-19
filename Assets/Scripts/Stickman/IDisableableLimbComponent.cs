namespace ss
{
    /// <summary>
    /// A component to disable in a limb (body part) when it's destroyed.
    /// See HittableLimb.
    /// </summary>
    public interface IDisableableLimbComponent
    {
        void DisableLimbComponent();
    }
}
