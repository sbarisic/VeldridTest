using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace VeldridTest.Graphics {
	abstract class GraphicsObject : IDisposable {
		protected GraphicsDevice Dev;
		protected ResourceFactory Fact;

		public void Create(GraphicsDevice Dev) {
			this.Dev = Dev;
			this.Fact = Dev.ResourceFactory;
		}

		public abstract void Dispose();
	}

	abstract class Renderable : GraphicsObject {
		public abstract void Render(CommandList List);
	}
}
