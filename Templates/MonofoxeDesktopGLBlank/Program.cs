﻿using System;
// using YourSharedProjectNamespace;
// Don't forget to reference it first!

namespace $safeprojectname$
{
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			using (var game = new Game1())
			{
				game.Run();
			}
		}
	}
}
