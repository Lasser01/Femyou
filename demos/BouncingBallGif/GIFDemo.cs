using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AnimatedGif;
using Femyou;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace BouncingBallGif {
	internal static class GIFDemo {
		private static readonly string FmuFolder =
			Path.Combine(
				Tools.GetBaseFolder(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath,
					nameof(Femyou)), "FMU", "bin", "dist");

		internal static void CreateGIF() {
			var info = new SKImageInfo(400, 400);
			var r = 70;
			var h = (double) (info.Height - r * 2);
			var v = 0.0;
			var hCoord = new SKPoint(0, 20);
			var vCoord = new SKPoint(0, 40);
			var paint = new SKPaint {
				Color = SKColors.Red,
				IsAntialias = true,
				Style = SKPaintStyle.Fill,
				TextSize = 16
			};

			using IModel model = Model.Load(Path.Combine(FmuFolder, "BouncingBall.fmu"));
			using IInstance instance = Tools.CreateInstance(model, "demo");
			IVariable altitude = model.Variables["h"];
			IVariable velocity = model.Variables["v"];
			instance.WriteReal((altitude, h));
			instance.WriteReal((velocity, v));
			using var gif = new AnimatedGifCreator("BouncingBall.gif");
			instance.StartTime(0.0);
			while (h > 0 || Math.Abs(v) > 0) {
				IEnumerable<double> variables = instance.ReadReal(altitude, velocity);
				h = variables.First();
				v = variables.Last();
				AddFrame(gif, info, canvas => {
					canvas.Clear(SKColors.WhiteSmoke);
					canvas.DrawText($"h = {h:F2} m", hCoord, paint);
					canvas.DrawText($"v = {v:F2} m/s", vCoord, paint);
					canvas.DrawCircle(info.Width / 2, info.Height - (int) h - r, r, paint);
				});
				instance.AdvanceTime(0.1);
			}
		}

		private static void AddFrame(AnimatedGifCreator gif, SKImageInfo info, Action<SKCanvas> action) {
			using var surface = SKSurface.Create(info);
			action(surface.Canvas);
			using SKImage image = surface.Snapshot();
			using var bitmap = image.ToBitmap();
			gif.AddFrame(bitmap, quality: GifQuality.Bit8);
		}
	}
}