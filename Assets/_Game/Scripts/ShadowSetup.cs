using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowSetup : ShadowShape2DProvider
{
    public override string ProviderName(string componentName)
    {
        return base.ProviderName(componentName);
    }

    public override int Priority()
    {
        return base.Priority();
    }

    public override void Enabled(Component sourceComponent, ShadowShape2D persistantShadowShape)
    {
        base.Enabled(sourceComponent, persistantShadowShape);
    }

    public override void Disabled(Component sourceComponent, ShadowShape2D persistantShadowShape)
    {
        base.Disabled(sourceComponent, persistantShadowShape);
    }

    public override void OnPersistantDataCreated(Component sourceComponent, ShadowShape2D persistantShadowShape)
    {
        base.OnPersistantDataCreated(sourceComponent, persistantShadowShape);
    }

    public override void OnBeforeRender(Component sourceComponent, Bounds worldCullingBounds, ShadowShape2D persistantShadowShape)
    {
        base.OnBeforeRender(sourceComponent, worldCullingBounds, persistantShadowShape);
    }

    public override bool IsShapeSource(Component sourceComponent)
    {
        return true;
    }
}