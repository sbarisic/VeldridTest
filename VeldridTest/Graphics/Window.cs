using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using System.Runtime.InteropServices;

namespace VeldridTest.Graphics {
	class Window {
		bool IsDebug;

		Sdl2Window Wnd;
		public GraphicsDevice GfxDev;

		public bool ShouldClose {
			get {
				return !Wnd.Exists;
			}
		}

		public Window(string Name, int W, int H) {
#if DEBUG
			IsDebug = true;
#endif

			WindowCreateInfo WindowInfo = new WindowCreateInfo();
			WindowInfo.WindowWidth = W;
			WindowInfo.WindowHeight = H;
			WindowInfo.X = 100;
			WindowInfo.Y = 100;
			WindowInfo.WindowTitle = Name;

			Wnd = VeldridStartup.CreateWindow(WindowInfo);

			GraphicsDeviceOptions GraphicsOpt = new GraphicsDeviceOptions(IsDebug);
			GraphicsOpt.ResourceBindingModel = ResourceBindingModel.Improved;
			GfxDev = VeldridStartup.CreateGraphicsDevice(Wnd, GraphicsOpt, GraphicsBackend.Vulkan);

			UniformLayouts.Init(GfxDev);
		}

		public Pipeline CreatePipeline(ResourceLayout[] ResourceLayouts) {
			GraphicsPipelineDescription PipelineDesc = new GraphicsPipelineDescription();
			PipelineDesc.BlendState = BlendStateDescription.SingleOverrideBlend;
			PipelineDesc.DepthStencilState = new DepthStencilStateDescription(false, true, ComparisonKind.LessEqual);
			PipelineDesc.RasterizerState = new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.Clockwise, true, false);
			PipelineDesc.PrimitiveTopology = PrimitiveTopology.TriangleList;
			PipelineDesc.ResourceLayouts = ResourceLayouts;
			PipelineDesc.ShaderSet = CreateShaderSet("Default/default");
			PipelineDesc.Outputs = GfxDev.SwapchainFramebuffer.OutputDescription;

			return GfxDev.ResourceFactory.CreateGraphicsPipeline(ref PipelineDesc);
		}

		public CommandList CreateCommandList() {
			return GfxDev.ResourceFactory.CreateCommandList();
		}

		ShaderSetDescription CreateShaderSet(string Name) {
			ShaderSetDescription ShaderSet = new ShaderSetDescription();
			LoadShader(Name, out ShaderSet.Shaders, out ShaderSet.VertexLayouts);
			return ShaderSet;
		}

		void LoadShader(string Name, out Shader[] Shaders, out VertexLayoutDescription[] Layout) {
			Layout = new VertexLayoutDescription[] {
					new VertexLayoutDescription(
						new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
						new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4),
						new VertexElementDescription("UV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
					)
				};

			string VertCode = File.ReadAllText(Path.Combine("Data/Shaders", Name + ".vert"));
			string FragCode = File.ReadAllText(Path.Combine("Data/Shaders", Name + ".frag"));

			ShaderDescription VertDesc = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(VertCode), "main");
			ShaderDescription FragDesc = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragCode), "main");
			Shaders = GfxDev.ResourceFactory.CreateFromSpirv(VertDesc, FragDesc);
		}

		public Framebuffer GetFramebuffer() {
			return GfxDev.SwapchainFramebuffer;
		}

		public ResourceSet CreateResourceSet(ResourceLayout Layout, BindableResource[] Resources) {
			ResourceSetDescription Desc = new ResourceSetDescription(Layout, Resources);
			return GfxDev.ResourceFactory.CreateResourceSet(Desc);
		}

		public void SubmitCommands(CommandList List) {
			GfxDev.SubmitCommands(List);
		}

		public void WaitForIdle() {
			GfxDev.WaitForIdle();
		}

		public void SwapBuffers() {
			GfxDev.SwapBuffers();
		}

		public void PumpEvents() {
			Wnd.PumpEvents();
		}

		public T CreateGraphicsObject<T>() where T : GraphicsObject, new() {
			T Obj = new T();
			Obj.Create(GfxDev);
			return Obj;
		}
	}

	static class UniformLayouts {
		public static ResourceLayout TextureLayout;
		public static ResourceLayout UniformsLayout;

		public static void Init(GraphicsDevice Dev) {
			ResourceLayoutDescription TexDesc = new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Tex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("Smp", ResourceKind.Sampler, ShaderStages.Fragment)
				);

			TextureLayout = Dev.ResourceFactory.CreateResourceLayout(TexDesc);

			ResourceLayoutDescription UniformsDesc = new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Uniforms", ResourceKind.UniformBuffer, ShaderStages.Fragment)
				);

			UniformsLayout = Dev.ResourceFactory.CreateResourceLayout(UniformsDesc);
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct Uniforms {
		public Vector4 Clr;
	}
}
