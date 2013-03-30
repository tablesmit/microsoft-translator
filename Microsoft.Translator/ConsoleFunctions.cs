using System;

namespace ConsoleFunctions
{
	/// <summary>
	/// Display Progress Bar in Console Application in C#
	/// http://www.bytechaser.com/en/articles/ckcwh8nsyt/display-progress-bar-in-console-application-in-c.aspx
	/// </summary>
	static class Code
	{

		/// <summary>
		/// Renders a message in the console window that overwrites
		/// any previous message in the same location
		/// </summary>
		/// <param name="message">Message to be displayed in the console</param>
		public static void OverwriteConsoleMessage(string message)
		{
			//move the cursor to the beginning of the console
			Console.CursorLeft = 0;
			//Get size of console
			int maxCharacterWidth = Console.WindowWidth - 1;
			if (message.Length > maxCharacterWidth)
			{
				//if message is longer than the console window truncate it
				message = message.Substring(0, maxCharacterWidth - 3) + "...";
			}
			//create a new message string with the end padded out with blank spaces
			message = message + new string(' ', maxCharacterWidth - message.Length);
			//update the console with the message
			Console.Write(message);
		}

		/// <summary>
		/// Draws a progress bar in a console window using a default character
		/// and maintaining the console window foreground color
		/// </summary>
		/// <param name="percentage">Percentage to be displayed on console</param>
		public static void RenderConsoleProgress(int percentage)
		{
			//other good progress bar characters \u2591 \u2592
			RenderConsoleProgress(percentage, '\u2590', Console.ForegroundColor, "");
		}

		/// <summary>
		/// Draws a progress bar in a console window using a selected character
		/// to make up the progres bar elements. A message can be displayed below
		/// the console at the same time.
		/// </summary>
		/// <param name="percentage">Percentage to be displayed on console</param>
		/// <param name="progressBarCharacter">Character used to build progress bar</param>
		/// <param name="color">Color of progress bar</param>
		/// <param name="message">Message to be displayed below console</param>
		public static void RenderConsoleProgress(int percentage, char progressBarCharacter,
				  ConsoleColor color, string message)
		{
			Console.CursorVisible = false;
			//record the original console color before changing it
			ConsoleColor originalColor = Console.ForegroundColor;
			//set the console to use the selected color
			Console.ForegroundColor = color;
			//move the cursor to the left of the console
			Console.CursorLeft = 0;
			//Determine the maximum width of the console window
			int width = Console.WindowWidth - 1;
			//Calculate the number of character required to create the
			//progress bar
			int newWidth = (int)((width * percentage) / 100d);

			//Create the progress bar text to be displayed
			string progBar = new string(progressBarCharacter, newWidth) +
				  new string(' ', width - newWidth);

			Console.Write(progBar);
			if (string.IsNullOrEmpty(message)) message = "";
			//move the cursor down one line to display the message
			Console.CursorTop++;
			//Render the message below the progress bar
			OverwriteConsoleMessage(message);
			//reset the cursor up 1 line
			Console.CursorTop--;
			//reset the console color back to the original color
			Console.ForegroundColor = originalColor;

			Console.CursorVisible = true;
		}
	}
}