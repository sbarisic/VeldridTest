using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Veldrid;

namespace VeldridTest.Graphics {
	static class GraphicsDebug {
		static RenderDoc RDoc;

		public static void Init() {
			string CapturePath = Path.GetFullPath("./RenderDocCapture/");

			if (!Directory.Exists(CapturePath))
				Directory.CreateDirectory(CapturePath);

			if (RenderDoc.Load(out RDoc))
				RDoc.SetCaptureSavePath(CapturePath);
		}

		public static void TriggerCapture() {
			RDoc?.TriggerCapture();
		}
	}
}
