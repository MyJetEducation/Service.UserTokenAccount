using System;
using Service.UserTokenAccount.Grpc.Models;
using Service.UserTokenAccount.Postgres.Models;

namespace Service.UserTokenAccount.Mappers
{
	public static class UserTokenOperationMapper
	{
		public static OperationGrpcModel ToGrpcModel(this UserTokenOperationEntity model) => new OperationGrpcModel
		{
			UserId = model.UserId,
			Date = model.Date,
			Source = model.Source,
			Movement = model.Movement,
			ProductType = model.ProductType,
			Value = model.Value,
			Info = model.Info
		};

		public static UserTokenOperationEntity ToModel(this NewOperationGrpcRequest request, DateTime now) => new UserTokenOperationEntity
		{
			UserId = request.UserId,
			Date = now,
			ProductType = request.ProductType,
			Movement = request.Movement,
			Source = request.Source,
			Value = request.Value,
			Info = request.Info
		};
	}
}