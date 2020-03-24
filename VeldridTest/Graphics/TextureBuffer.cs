using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using DRect = System.Drawing.Rectangle;
using PixFormat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = Veldrid.PixelFormat;
using Veldrid;

namespace VeldridTest.Graphics {
	class TextureBuffer : GraphicsObject {
		Texture Tex;
		public TextureView TexView;
		public Sampler TexSampler;

		public int Width;
		public int Height;

		public void Update(ResourceLayout TexLayout, int W, int H) {
			if (Width != W || Height != H) {
				Tex?.Dispose();
				TexView?.Dispose();

				Width = W;
				Height = H;

				TextureDescription Desc = new TextureDescription((uint)W, (uint)H, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Sampled, TextureType.Texture2D);
				Tex = Fact.CreateTexture(Desc);
				TexView = Fact.CreateTextureView(Tex);

				SamplerDescription SamplerDesc = new SamplerDescription(SamplerAddressMode.Clamp, SamplerAddressMode.Clamp, SamplerAddressMode.Clamp,
					SamplerFilter.MinPoint_MagPoint_MipPoint, null, 0, 0, 0, 0, SamplerBorderColor.OpaqueBlack);
				TexSampler = Fact.CreateSampler(SamplerDesc);
			}
		}

		public void Update(ResourceLayout TexLayout, int W, int H, IntPtr Data, int Length) {
			Update(TexLayout, W, H);
			Dev.UpdateTexture(Tex, Data, (uint)Length, 0, 0, 0, (uint)W, (uint)H, 1, 0, 0);
		}

		public void Update(ResourceLayout TexLayout, Image Img) {
			using (Bitmap Bmp = new Bitmap(Img)) {
				BitmapData BmpData = Bmp.LockBits(new DRect(0, 0, Img.Width, Img.Height), ImageLockMode.ReadOnly, PixFormat.Format32bppArgb);
				Update(TexLayout, Img.Width, Img.Height, BmpData.Scan0, Bmp.Width * Bmp.Height * 4);
				Bmp.UnlockBits(BmpData);
			}
		}

		public override void Dispose() {
			Tex?.Dispose();
			TexView?.Dispose();
			TexSampler?.Dispose();
		}
	}
}
