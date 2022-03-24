using System.Text.Json.Serialization;

namespace Service.UserTokenAccount.Domain.Models
{
	/// <summary>
	///     Тип операции
	/// </summary>
	[JsonConverter(typeof (JsonStringEnumConverter))]
	public enum TokenOperationMovement
	{
		/// <summary>
		///     Приход
		/// </summary>
		Income,

		/// <summary>
		///     Расход
		/// </summary>
		Outcome
	}
}