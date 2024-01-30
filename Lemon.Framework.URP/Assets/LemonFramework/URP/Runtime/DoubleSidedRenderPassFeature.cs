using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DoubleSidedRenderPassFeature : ScriptableRendererFeature
{
    class DoubleSidedRenderPass : ScriptableRenderPass
    {
        FilteringSettings m_FilteringSettings;
        RenderStateBlock m_RenderStateBlock;
        string m_ProfilerTag;
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        ProfilingSampler m_ProfilingSampler;
        bool m_IsOpaque;
        static readonly int s_DrawObjectPassDataPropID = Shader.PropertyToID("_DrawObjectPassData");

        public DoubleSidedRenderPass(string profilerTag, bool opaque, RenderPassEvent renderPassEvent,
            RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int tencilReference)
        {
            m_ProfilerTag = profilerTag;
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            m_ShaderTagIdList.Add(new ShaderTagId("BackFace"));
            this.renderPassEvent = renderPassEvent;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_IsOpaque = opaque;
            if (stencilState.enabled)
            {
                m_RenderStateBlock.stencilReference = tencilReference;
                m_RenderStateBlock.mask = RenderStateMask.Stencil;
                m_RenderStateBlock.stencilState = stencilState;
            }
        } 
 
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor renderTextureDescriptor)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd,m_ProfilingSampler))
            {
                //Global render pass data containing various settings.
                //x,y,z are currently unused
                //w is used for knowing whether the object is opaque(1) or alpha blended(0)
                Vector4 drawObjectPassData = new Vector4(0, 0, 0, m_IsOpaque ? 1.0f : 0.0f);
                cmd.SetGlobalVector(s_DrawObjectPassDataPropID, drawObjectPassData);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                Camera camera = renderingData.cameraData.camera;
                var sortFlags = m_IsOpaque? renderingData.cameraData.defaultOpaqueSortFlags:SortingCriteria.CommonTransparent;
                var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings,
                    ref m_RenderStateBlock);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    DoubleSidedRenderPass m_DoubleSidedRenderPass;

    /// <inheritdoc/>
    public override void Create()
    {
        StencilStateData stencilStateData = new StencilStateData(); 
        StencilState stencilState = StencilState.defaultValue;
        stencilState.enabled = stencilStateData.overrideStencilState;
        stencilState.SetCompareFunction(stencilStateData.stencilCompareFunction);
        stencilState.SetPassOperation(stencilStateData.passOperation);
        stencilState.SetFailOperation(stencilStateData.failOperation);
        stencilState.SetZFailOperation(stencilStateData.zFailOperation);
        m_DoubleSidedRenderPass = new DoubleSidedRenderPass("Render Transparents",false, RenderPassEvent.AfterRenderingTransparents,
            RenderQueueRange.transparent, -1, stencilState, stencilStateData.stencilReference);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_DoubleSidedRenderPass);
    }
}


