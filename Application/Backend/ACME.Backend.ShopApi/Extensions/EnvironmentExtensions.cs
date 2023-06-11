namespace ACME.Backend.ShopApi.Extensions;

public static class EnvironmentExtensions
{
    public const string Kubernetes = "Kubernetes";

    public static bool IsKubernetes(this IHostEnvironment env)
    {
        return env.IsEnvironment(Kubernetes);
    }
}
