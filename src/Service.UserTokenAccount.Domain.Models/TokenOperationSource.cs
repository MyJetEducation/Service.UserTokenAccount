using System.Text.Json.Serialization;

namespace Service.UserTokenAccount.Domain.Models
{
	/// <summary>
	///     Исход операции
	/// </summary>
	[JsonConverter(typeof (JsonStringEnumConverter))]
	public enum TokenOperationSource
	{
		/// <summary>
		///     Покупка токена
		/// </summary>
		TokenPurchase,

		/// <summary>
		///     Покупка товара
		/// </summary>
		ProductPurchase
	}
}