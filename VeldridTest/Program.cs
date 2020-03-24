using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using Veldrid;
using VeldridTest.Graphics;

namespace VeldridTest {
	unsafe static class Program {
		static Window Wnd;

		static void Main(string[] args) {
			GraphicsDebug.Init();
			Wnd = new Window("Test", 800, 600);

			Pipeline DefaultPipeline = Wnd.CreatePipeline(new ResourceLayout[] { UniformLayouts.TextureLayout, UniformLayouts.UniformsLayout });
			CommandList List = Wnd.CreateCommandList();

			MeshBuffer Msh = Wnd.CreateGraphicsObject<MeshBuffer>();
			Msh.Upload(new Vertex3D[] {
				new Vertex3D(new Vector3(0, 0, 0), RgbaFloat.Red, new Vector2(0, 0)),
				new Vertex3D(new Vector3(0, 1, 0), RgbaFloat.Green, new Vector2(0, 1)),
				new Vertex3D(new Vector3(1, 1, 0), RgbaFloat.Blue, new Vector2(1, 1)),
				new Vertex3D(new Vector3(1, 0, 0), RgbaFloat.Yellow, new Vector2(1, 0)),
			}, new ushort[] { 2, 1, 0, 2, 0, 3 });

			TextureBuffer Tex = Wnd.CreateGraphicsObject<TextureBuffer>();
			Tex.Update(UniformLayouts.TextureLayout, Image.FromFile("Data/Textures/Test.png"));

			ResourceSet TexResource = Wnd.CreateResourceSet(UniformLayouts.TextureLayout, new BindableResource[] { Tex.TexView, Tex.TexSampler });

			DeviceBuffer UniformBuffer = Wnd.GfxDev.ResourceFactory.CreateBuffer(new BufferDescription((uint)sizeof(Uniforms), BufferUsage.UniformBuffer, 0));
			ResourceSet UniformsRes = Wnd.CreateResourceSet(UniformLayouts.UniformsLayout, new BindableResource[] { UniformBuffer });

			while (!Wnd.ShouldClose) {
				{
					List.Begin();
					List.SetFramebuffer(Wnd.GetFramebuffer());
					List.ClearColorTarget(0, RgbaFloat.CornflowerBlue);

					List.SetPipeline(DefaultPipeline);
					List.SetGraphicsResourceSet(0, TexResource);

					/*List.SetGraphicsResourceSet(1, UniformsRes);
					List.UpdateBuffer(UniformBuffer, 0, new Uniforms() {
						Clr = new Vector4(50, 143, 168, 255) / new Vector4(255),
					});*/

					Msh.Render(List);

					List.End();
				}

				Wnd.SubmitCommands(List);
				Wnd.WaitForIdle();
				Wnd.SwapBuffers();
				Wnd.PumpEvents();
			}
		}
	}
}
