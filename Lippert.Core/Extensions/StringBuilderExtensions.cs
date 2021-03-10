using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lippert.Core.Extensions
{
	/// <summary>
	/// Extension methods that act upon <see cref="StringBuilder"/>s
	/// </summary>
	public static class StringBuilderExtensions
	{
		/// <summary>
		/// Appends copies of the specified strings followed by the default line terminator to the end of the specified <see cref="StringBuilder"/> object.
		/// </summary>
		/// <param name="lines">The strings to append.</param>
		/// <returns>A reference to the specified instance after the append operations have completed.</returns>
		public static StringBuilder AppendLines(this StringBuilder builder, params string[] lines) => AppendLines(builder, lines.AsEnumerable());
		/// <summary>
		/// Appends copies of the specified strings followed by the default line terminator to the end of the specified <see cref="StringBuilder"/> object.
		/// </summary>
		/// <param name="lines">The strings to append.</param>
		/// <returns>A reference to the specified instance after the append operations have completed.</returns>
		public static StringBuilder AppendLines(this StringBuilder builder, IEnumerable<string> lines)
		{
			foreach (var line in lines)
			{
				builder.AppendLine(line);
			}

			return builder;
		}
	}
}