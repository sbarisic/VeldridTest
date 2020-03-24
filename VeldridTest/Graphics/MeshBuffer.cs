using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid;
using System.Runtime.InteropServices;

namespace VeldridTest.Graphics {
	struct Vertex3D {
		public Vector3 Position;
		public RgbaFloat Color;
		public Vector2 UV;

		public Vertex3D(Vector3 Position, RgbaFloat Color, Vector2 UV) {
			this.Position = Position;
			this.Color = Color;
			this.UV = UV;
		}
	}

	unsafe class MeshBuffer : Renderable {
		DeviceBuffer VertBuffer;
		DeviceBuffer IndBuffer;

		int IndexCount;

		public void Upload(Vertex3D[] Verts, ushort[] Inds) {
			VertBuffer?.Dispose();
			IndBuffer?.Dispose();

			VertBuffer = Fact.CreateBuffer(new BufferDescription((uint)(sizeof(Vertex3D) * Verts.Length), BufferUsage.VertexBuffer));
			VertBuffer.Name = "VertBuffer";

			IndBuffer = Fact.CreateBuffer(new BufferDescription((uint)(sizeof(ushort) * Inds.Length), BufferUsage.IndexBuffer));
			IndBuffer.Name = "IndBuffer";

			Dev.UpdateBuffer(VertBuffer, 0, Verts);
			Dev.UpdateBuffer(IndBuffer, 0, Inds);
			IndexCount = Inds.Length;
		}

		public override void Render(CommandList List) {
			List.SetVertexBuffer(0, VertBuffer);
			List.SetIndexBuffer(IndBuffer, IndexFormat.UInt16);
			List.DrawIndexed((uint)IndexCount, 1, 0, 0, 0);
		}

		public override void Dispose() {
			VertBuffer?.Dispose();
			IndBuffer?.Dispose();
		}
	}
}
